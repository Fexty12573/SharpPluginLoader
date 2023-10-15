#include "Log.h"

#include <chrono>

namespace debug::log::impl {
static HANDLE s_console = nullptr;

using WriteConsoleFunc = BOOL(WINAPI*)(HANDLE, const void*, DWORD, LPDWORD, LPVOID);

static void log_raw(LogLevel level, const void* msg, size_t msg_length, WriteConsoleFunc write_console) {
    if (!s_console) {
        s_console = GetStdHandle(STD_OUTPUT_HANDLE);
        if (s_console == INVALID_HANDLE_VALUE) {
            loader::LOG(loader::ERR) << "[SPL] Failed to get console handle";
            return;
        }
    }

    const auto time = std::format("[ {:%T} | SPL ] ", std::chrono::system_clock::now());

    SetConsoleTextAttribute(s_console, FOREGROUND_GREEN);
    write_console(s_console, time.data(), (UINT)time.size(), nullptr, nullptr);

    SetConsoleTextAttribute(s_console, level); // See LogLevel enum
    write_console(s_console, msg, (UINT)msg_length, nullptr, nullptr);
    write_console(s_console, "\n", 1, nullptr, nullptr);
    SetConsoleTextAttribute(s_console, 0);
}

}

void dlog::impl::log(dlog::impl::LogLevel level, const std::string& msg) {
    log_raw(level, msg.data(), msg.size(), WriteConsoleA);
}

void debug::log::impl::log(LogLevel level, const std::wstring& msg) {
    log_raw(level, msg.data(), msg.size(), WriteConsoleW);
}
