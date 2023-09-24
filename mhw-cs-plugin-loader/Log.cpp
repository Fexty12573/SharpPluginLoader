#include "Log.h"

#include <chrono>

namespace debug::log::impl {
static HANDLE s_console = nullptr;
}


void dlog::impl::log(dlog::impl::LogLevel level, const std::string& msg) {
    if (!s_console) {
        s_console = GetStdHandle(STD_OUTPUT_HANDLE);
        if (s_console == INVALID_HANDLE_VALUE) {
            loader::LOG(loader::ERR) << "[SPL] Failed to get console handle";
            return;
        }
    }

    const auto time = std::format("[ {:%T} | SPL ] ", std::chrono::system_clock::now());

    SetConsoleTextAttribute(s_console, FOREGROUND_GREEN);
    WriteConsoleA(s_console, time.data(), (UINT)time.size(), nullptr, nullptr);

    SetConsoleTextAttribute(s_console, level); // See LogLevel enum
    WriteConsoleA(s_console, msg.data(), (UINT)msg.size(), nullptr, nullptr);
    SetConsoleTextAttribute(s_console, 0);
}

extern "C" static void public_log_interface(int level, const char* msg) {
    dlog::impl::log((dlog::impl::LogLevel)level, msg);
}
