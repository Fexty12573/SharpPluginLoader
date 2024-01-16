#include <cstdint>
#include <string>
#include <thread>

#define WIN32_LEAN_AND_MEAN
#include <wil/resource.h>
#include <wil/stl.h>
#include <wil/win32_helpers.h>
#include <windows.h>

#include <safetyhook/safetyhook.hpp>

#include "NativePluginFramework.h"
#include "CoreClr.h"
#include "Log.h"
#include "Preloader.h"
#include "LoaderConfig.h"
#include "utility/game_functions.h"

#pragma intrinsic(_ReturnAddress)

SafetyHookInline g_get_system_time_as_file_time_hook{};
SafetyHookInline g_win_main_hook{};
SafetyHookInline g_scrt_common_main_hook{};
SafetyHookInline g_mh_main_ctor_hook{};

CoreClr* s_coreclr = nullptr;
NativePluginFramework* s_framework = nullptr;

// TODO(Andoryuuta): AOB scan for these.
namespace preloader::address {
    const uint64_t IMAGE_BASE = 0x140000000;
    const uint64_t PROCESS_SECURITY_COOKIE = IMAGE_BASE + 0x4bf4be8;
    const uint64_t SECURITY_COOKIE_INIT_GETTIME_RET = IMAGE_BASE + 0x27422e2;
    const uint64_t SCRT_COMMON_MAIN_SEH = IMAGE_BASE + 0x27414f4;
    const uint64_t WINMAIN = IMAGE_BASE + 0x13a4c00;

}  // namespace preloader::address


void open_console() {
    AllocConsole();
    FILE* cin_stream;
    FILE* cout_stream;
    FILE* cerr_stream;
    freopen_s(&cin_stream, "CONIN$", "r", stdin);
    freopen_s(&cout_stream, "CONOUT$", "w", stdout);
    freopen_s(&cerr_stream, "CONOUT$", "w", stderr);

    // From: https://stackoverflow.com/a/45622802 to deal with UTF8 CP:
    SetConsoleOutputCP(CP_UTF8);
    setvbuf(stdout, nullptr, _IOFBF, 1000);
}

// This hooks the __scrt_common_main_seh MSVC function.
// This runs before all of the CRT initalization, static initalizers, and WinMain.
__declspec(noinline) int64_t hooked_scrt_common_main()
{
    dlog::info("Initializing CLR / NativePluginFramework");
    s_coreclr = new CoreClr();
    s_framework = new NativePluginFramework(s_coreclr);
    dlog::info("Initialized");

    s_framework->trigger_on_pre_main();

    return g_scrt_common_main_hook.call<int64_t>();
}

__declspec(noinline) int __stdcall hooked_win_main(HINSTANCE hInstance, HINSTANCE hPrevInstance, LPSTR lpCmdLine, int nShowCmd)
{
    s_framework->trigger_on_win_main();
    return g_win_main_hook.call<int>(hInstance, hPrevInstance, lpCmdLine, nShowCmd);
}

__declspec(noinline) void* hooked_mh_main_ctor(void* this_ptr)
{
    auto result = g_mh_main_ctor_hook.call<void*>(this_ptr);
    s_framework->trigger_on_mh_main_ctor();
    return result;
}

// The hooked GetSystemTimeAsFileTime function.
// This function is called in many places, one of them being the
// `__security_init_cookie` function that is used to setup the security token(s)
// before SCRT_COMMON_MAIN_SEH is called.
void hooked_get_system_time_as_file_time(LPFILETIME lpSystemTimeAsFileTime) {

    // If we match the return address within the `__security_init_cookie`
    // then the client has been unpacked and we can hook the other  functions now.
    uint64_t ret_address = (uint64_t)_ReturnAddress();
    if (ret_address == preloader::address::SECURITY_COOKIE_INIT_GETTIME_RET) {

        g_scrt_common_main_hook = safetyhook::create_inline(
            reinterpret_cast<void*>(preloader::address::SCRT_COMMON_MAIN_SEH),
            reinterpret_cast<void*>(hooked_scrt_common_main)
        );

        g_win_main_hook = safetyhook::create_inline(
            reinterpret_cast<void*>(preloader::address::WINMAIN),
            reinterpret_cast<void*>(hooked_win_main)
        );

        g_mh_main_ctor_hook = safetyhook::create_inline(
            reinterpret_cast<void*>(MH::sMhMain::ctor),
            reinterpret_cast<void*>(hooked_mh_main_ctor)
        );

        // Unhook this function and call the original
        g_get_system_time_as_file_time_hook = {};
        GetSystemTimeAsFileTime(lpSystemTimeAsFileTime);
        return;
    }

    // Not the expected return address, just proxy to real function.
    g_get_system_time_as_file_time_hook.call<LPFILETIME>(lpSystemTimeAsFileTime);
}


// This function is called from the loader-locked DllMain.
// It does the bare-minimum to get control flow in the main thread
// by hooking a function called in the CRT startup (GetSystemTimeAsFileTime).
//
// This allows us to work with both the SteamDRM and Steamless unpacked
// binaries by detecting the first call to the hooked function _after_
// the executable is unpacked in memory.
void initialize_preloader() {
    auto& loader_config = preloader::LoaderConfig::get();
    if (loader_config.get_log_cmd())
    {
        open_console();
    }

    // Override the process security token that that the singleton instantiaion
    // happens, causing GetSystemTimeAsFileTime to be called pre-CRT init.
    *(uint64_t*)preloader::address::PROCESS_SECURITY_COOKIE = 0x2B992DDFA232L;

    g_get_system_time_as_file_time_hook = safetyhook::create_inline(
        reinterpret_cast<void*>(GetSystemTimeAsFileTime),
        reinterpret_cast<void*>(hooked_get_system_time_as_file_time)
    );
}