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
#include "utility/game_functions.h"

#pragma intrinsic(_ReturnAddress)

SafetyHookInline g_GetSystemTimeAsFileTime_hook{};
SafetyHookInline g_WinMain_hook{};
SafetyHookInline g_SCRTCommonMain_hook{};
SafetyHookInline g_MhMainCtor_hook{};

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


void OpenConsole() {
    AllocConsole();
    FILE* cinStream;
    FILE* coutStream;
    FILE* cerrStream;
    freopen_s(&cinStream, "CONIN$", "r", stdin);
    freopen_s(&coutStream, "CONOUT$", "w", stdout);
    freopen_s(&cerrStream, "CONOUT$", "w", stderr);

    // From: https://stackoverflow.com/a/45622802 to deal with UTF8 CP:
    SetConsoleOutputCP(CP_UTF8);
    setvbuf(stdout, nullptr, _IOFBF, 1000);
}

// This hooks the __scrt_common_main_seh MSVC function.
// This runs before all of the CRT initalization, static initalizers, and WinMain.
__declspec(noinline) int64_t hookedSCRTCommonMain()
{
    dlog::info("Initializing CLR / NativePluginFramework");
    s_coreclr = new CoreClr();
    s_framework = new NativePluginFramework(s_coreclr);
    dlog::info("Initialized");

    s_framework->TriggerOnPreMain();

    return g_SCRTCommonMain_hook.call<int64_t>();
}

__declspec(noinline) int __stdcall hookedWinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance, LPSTR lpCmdLine, int nShowCmd)
{
    s_framework->TriggerOnWinMain();

    return g_WinMain_hook.call<int>(hInstance, hPrevInstance, lpCmdLine, nShowCmd);
}

__declspec(noinline) void* hookedMhMainCtor(void* this_ptr)
{
    s_framework->TriggerOnMhMainCtor();

    return g_MhMainCtor_hook.call<void*>(this_ptr);
}

// The hooked GetSystemTimeAsFileTime function.
// This function is called in many places, one of them being the
// `__security_init_cookie` function that is used to setup the security token(s)
// before SCRT_COMMON_MAIN_SEH is called.
void hookedGetSystemTimeAsFileTime(LPFILETIME lpSystemTimeAsFileTime) {

    // If we match the return address within the `__security_init_cookie`
    // then the client has been unpacked and we can hook the other  functions now.
    uint64_t ret_address = (uint64_t)_ReturnAddress();
    if (ret_address == preloader::address::SECURITY_COOKIE_INIT_GETTIME_RET) {

        g_SCRTCommonMain_hook = safetyhook::create_inline(
            reinterpret_cast<void*>(preloader::address::SCRT_COMMON_MAIN_SEH),
            reinterpret_cast<void*>(hookedSCRTCommonMain)
        );

        g_WinMain_hook = safetyhook::create_inline(
            reinterpret_cast<void*>(preloader::address::WINMAIN),
            reinterpret_cast<void*>(hookedWinMain)
        );

        g_MhMainCtor_hook = safetyhook::create_inline(
            reinterpret_cast<void*>(MH::sMhMain::ctor),
            reinterpret_cast<void*>(hookedMhMainCtor)
        );

        // Unhook this function and call the original
        g_GetSystemTimeAsFileTime_hook = {};
        GetSystemTimeAsFileTime(lpSystemTimeAsFileTime);
        return;
    }

    // Not the expected return address, just proxy to real function.
    g_GetSystemTimeAsFileTime_hook.call<LPFILETIME>(lpSystemTimeAsFileTime);
}


// This function is called from the loader-locked DllMain.
// It does the bare-minimum to get control flow in the main thread
// by hooking a function called in the CRT startup (GetSystemTimeAsFileTime).
//
// This allows us to work with both the SteamDRM and Steamless unpacked
// binaries by detecting the first call to the hooked function _after_
// the executable is unpacked in memory.
void initialize_preloader() {
    OpenConsole();

    // Override the process security token that that the singleton instantiaion
    // happens, causing GetSystemTimeAsFileTime to be called pre-CRT init.
    *(uint64_t*)preloader::address::PROCESS_SECURITY_COOKIE = 0x2B992DDFA232L;

    g_GetSystemTimeAsFileTime_hook = safetyhook::create_inline(
        reinterpret_cast<void*>(GetSystemTimeAsFileTime),
        reinterpret_cast<void*>(hookedGetSystemTimeAsFileTime)
    );
}