#include <cstdio>
#include <fstream>
#include <iostream>
#include <mutex>

#include <Windows.h>
#include <intrin.h>
#include <psapi.h>

#include <safetyhook/safetyhook.hpp>
#include "game_address.h"
#include "log.h"
#include "preloader_core.h"

#pragma intrinsic(_ReturnAddress)

SafetyHookInline g_GetSystemTimeAsFileTime_hook{};
SafetyHookInline g_WinMain_hook{};
SafetyHookInline g_SCRTCommonMain_hook{};

// This is hooks the __scrt_common_main_seh MSVC function.
// This runs before all of the CRT initalization, static
// initalizers, and before WinMain.
__declspec(noinline) int64_t hookedSCRTCommonMain()
{
    // TODO(Andoryuuta): SPL callbacks - pre-init and pre-winmain.
    PRELOADER_LOG("TODO(Andoryuuta): SPL callbacks - pre-init and pre-winmain.");
    return g_SCRTCommonMain_hook.call<int64_t>();
}

__declspec(noinline) int __stdcall hookedWinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance, LPSTR lpCmdLine, int nShowCmd)
{
    // TODO(Andoryuuta): SPL callbacks - post-init, pre-winmain.
    PRELOADER_LOG("TODO(Andoryuuta): SPL callbacks - post-init, pre-winmain.");
    return g_WinMain_hook.call<int>(hInstance, hPrevInstance, lpCmdLine, nShowCmd);
}

// The hooked GetSystemTimeAsFileTime function.
// This function is called in many places, one of them being the
// `__security_init_cookie` function that is used to setup the security token(s)
// before SCRT_COMMON_MAIN_SEH is called.
void hookedGetSystemTimeAsFileTime(LPFILETIME lpSystemTimeAsFileTime) {

    // If we match the return address within the `__security_init_cookie`
    // then the client has been unpacked and we can hook the other functions now.
    uint64_t ret_address = (uint64_t)_ReturnAddress();
    if (ret_address == preloader::address::SECURITY_COOKIE_INIT_GETTIME_RET) {

        g_SCRTCommonMain_hook = safetyhook::create_inline(
            reinterpret_cast<void*>(preloader::address::SCRT_COMMON_MAIN_SEH),
            reinterpret_cast<void*>(hookedSCRTCommonMain)
        );

        g_GetSystemTimeAsFileTime_hook = safetyhook::create_inline(
            reinterpret_cast<void*>(preloader::address::WINMAIN),
            reinterpret_cast<void*>(hookedWinMain)
        );

        // Unhook this function and call the original
        g_GetSystemTimeAsFileTime_hook = {};
        GetSystemTimeAsFileTime(lpSystemTimeAsFileTime);
        return;
    }

    // Not the expected return address, just proxy to real function.
    g_GetSystemTimeAsFileTime_hook.call<LPFILETIME>(lpSystemTimeAsFileTime);
}

// This function is called from the loader-locked DllMain,
// does the bare-minimum to get control flow in the main thread
// by hooking a function called in the CRT startup (GetSystemTimeAsFileTime).
//
// This allows us to work with both the SteamDRM and Steamless unpacked
// binaries by detecting the first call to the hooked function _after_
// the executable is unpacked in memory.
void LoaderLockedInitialize() {
    // Override the process security token that that the singleton instantiaion
    // happens, causing GetSystemTimeAsFileTime to be called pre-CRT init.
    *(uint64_t*)preloader::address::PROCESS_SECURITY_COOKIE = 0x2B992DDFA232L;

    g_GetSystemTimeAsFileTime_hook = safetyhook::create_inline(
        reinterpret_cast<void*>(GetSystemTimeAsFileTime), 
        reinterpret_cast<void*>(hookedGetSystemTimeAsFileTime)
    );
}

void InitPreloader() {
    OpenConsole();
    LoaderLockedInitialize();
}

//BOOL WINAPI DllMain(HINSTANCE hinstDLL, DWORD fdwReason, LPVOID lpvReserved) {
//    if (fdwReason == DLL_PROCESS_ATTACH) {
//        preloader::log::OpenConsole();
//        LoaderLockedInitialize();
//    }
//
//    return TRUE;
//}