// cl.exe Launcher.c kernel32.lib user32.lib /link /SUBSYSTEM:windows /NODEFAULTLIB /ENTRY:WinMainCRTStartup

#include <windows.h>
#include <tlhelp32.h>

#define TARGET_EXE "MonsterHunterWorld.exe"
#define TARGET_DLL "winmm.dll"

static DWORD (*K32GetModuleBaseNameA)(IN HANDLE hProcess, IN HMODULE hModule, OUT LPSTR lpBaseName, IN DWORD nSize);
static BOOL (*K32EnumProcessModules)(IN HANDLE hProcess, OUT HMODULE *lphModule, IN DWORD cb, OUT LPDWORD lpcbNeeded);

// MonsterHunterWorld.exe -> ntdll.dll -> KERNEL32.dll -> KERNELBASE.dll -> (apphelp.dll) -> TARGET_DLL + dependencies.
// () = Present on Windows and not Wine (Windows 11, Proton 11.0).
// Total modules varies between Windows and Wine, 64 is plenty in the case of MHW and likely most scenarios.
static HMODULE moduleHandles[64];
static CHAR moduleName[MAX_MODULE_NAME32 + 1];
static CHAR dllPath[MAX_PATH];

// https://git.musl-libc.org/cgit/musl/tree/src/string/strncmp.c
static int Strncmp(const char *_l, const char *_r, size_t n)
{
	const unsigned char *l=(void *)_l, *r=(void *)_r;
	if (!n--) return 0;
	for (; *l && *r && n && *l == *r ; l++, r++, n--);
	return *l - *r;
}

static BOOL DllWasInjected(HANDLE process)
{
    DWORD cbNeeded;
    if (K32EnumProcessModules(process, moduleHandles, sizeof(moduleHandles), &cbNeeded)) {
        DWORD processCount = cbNeeded / sizeof(HMODULE);
        for (DWORD i = 0; i < processCount; i++) {
            K32GetModuleBaseNameA(process, moduleHandles[i], moduleName, sizeof(moduleName));
            if (Strncmp(TARGET_DLL, moduleName, sizeof(TARGET_DLL)) == 0) {
                return TRUE;
            }
        }
    }
    return FALSE;
}

void __stdcall WinMainCRTStartup()
{
    HMODULE k32 = GetModuleHandleA("kernel32.dll");
    LPVOID LoadLibraryAddr = (LPVOID)GetProcAddress(k32, "LoadLibraryA");
    K32GetModuleBaseNameA = (LPVOID)GetProcAddress(k32, "K32GetModuleBaseNameA");
    K32EnumProcessModules = (LPVOID)GetProcAddress(k32, "K32EnumProcessModules");

    STARTUPINFOA si = { .cb = sizeof(si) };
    PROCESS_INFORMATION pi = { 0 };
    if (CreateProcessA(NULL, TARGET_EXE, NULL, NULL, FALSE, CREATE_SUSPENDED, NULL, NULL, &si, &pi) == 0) {
        MessageBoxA(NULL,
            "Failed to create process. Is '" TARGET_EXE "' contained within this folder?",
            "Launch Error",
            MB_ICONERROR | MB_OK);
        ExitProcess(1);
    }

    DWORD arglen = GetFullPathNameA(TARGET_DLL, sizeof(dllPath), dllPath, NULL);
    LPVOID buf = VirtualAllocEx(pi.hProcess, NULL, arglen, MEM_RESERVE | MEM_COMMIT, PAGE_EXECUTE_READWRITE);
    WriteProcessMemory(pi.hProcess, buf, dllPath, arglen, NULL);

    HANDLE hThread = CreateRemoteThread(pi.hProcess, NULL, 0, (LPTHREAD_START_ROUTINE)LoadLibraryAddr, buf, 0, NULL);
    if (!hThread) {
        MessageBoxA(NULL,
            "Failed to CreateRemoteThread(), make sure the architecture of this exe matches " TARGET_EXE ".",
            "Injection Failed",
            MB_ICONWARNING | MB_OK);
    } else {
        WaitForSingleObject(hThread, INFINITE);
        // The value of GetExitCodeThread() isn't usable, so try to manually check
        // if the DLL got loaded (LoadLibrary() succeeded).
        if (!DllWasInjected(pi.hProcess)) {
            MessageBoxA(NULL,
                "Failed to inject DLL. Is '" TARGET_DLL "' contained within this folder?",
                "Injection Failed",
                MB_ICONWARNING | MB_OK);
        }
        CloseHandle(hThread);
    }

    VirtualFreeEx(pi.hProcess, buf, 0, MEM_RELEASE);

    ResumeThread(pi.hThread);

    WaitForSingleObject(pi.hProcess, INFINITE);
    CloseHandle(pi.hProcess);
    CloseHandle(pi.hThread);

    FreeLibrary(k32);

    ExitProcess(0);
}
