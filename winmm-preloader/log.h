#pragma once

#include <Windows.h>
#include <cstdio>
#include <mutex>

#define SPL_OWNED_CONSOLE 1

#ifdef SPL_OWNED_CONSOLE
    static inline void OpenConsole() {
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

    #define PRELOADER_LOG(fmt, ...) fprintf(stderr, "[SPL-Preloader]: " fmt "\n", ##__VA_ARGS__)   
#else
    static inline void OpenConsole() {}
    #define PRELOADER_LOG(...)
#endif // SPL_OWNED_CONSOLE
