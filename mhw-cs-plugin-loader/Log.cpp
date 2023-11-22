#include "Log.h"

#include <chrono>
#include <fstream>
#include <nlohmann/json.hpp>

namespace debug::log::impl {
static HANDLE s_console = nullptr;
static std::ofstream s_file;

static bool s_log_to_cmd = true;
static loader::LogLevel s_console_log_level = loader::INFO;

using OutputFunc = BOOL(WINAPI*)(HANDLE, const void*, DWORD, LPDWORD, LPVOID);

static loader::LogLevel to_loader_level(LogLevel level) {
    switch (level) {
    case LogLevel::Debug: return loader::DEBUG;
    case LogLevel::Info: return loader::INFO;
    case LogLevel::Warn: return loader::WARN;
    case LogLevel::Error: return loader::ERR;
    }

    return loader::INFO;
}

static void log_raw(LogLevel level, const void* msg, size_t msg_length, const void* time_msg, size_t time_msg_length, OutputFunc write) {
    if (!s_console) {
        nlohmann::json config;
        std::ifstream("loader-config.json") >> config;

        const auto log_level = config.value("logLevel", "INFO");
        s_log_to_cmd = config.value("logcmd", true);

        if (log_level == "DEBUG") {
            s_console_log_level = loader::DEBUG;
        } else if (log_level == "INFO") {
            s_console_log_level = loader::INFO;
        } else if (log_level == "WARNING") {
            s_console_log_level = loader::WARN;
        } else if (log_level == "ERROR") {
            s_console_log_level = loader::ERR;
        } else {
            loader::LOG(loader::ERR) << "[SPL] Invalid log level: " << log_level;
            return;
        }

        s_console = GetStdHandle(STD_OUTPUT_HANDLE);
        if (s_console == INVALID_HANDLE_VALUE) {
            loader::LOG(loader::ERR) << "[SPL] Failed to get console handle";
            return;
        }
    }

    if (!s_file) {
        s_file.open("nativePC/plugins/SharpPluginLoader.log", std::ios::out);
        if (!s_file) {
            loader::LOG(loader::ERR) << "[SPL] Failed to open log file";
            return;
        }
    }

    if (!s_log_to_cmd || to_loader_level(level) < s_console_log_level) {
        return;
    }

    SetConsoleTextAttribute(s_console, FOREGROUND_GREEN);
    write(s_console, time_msg, (UINT)time_msg_length, nullptr, nullptr);

    SetConsoleTextAttribute(s_console, level); // See LogLevel enum
    write(s_console, msg, (UINT)msg_length, nullptr, nullptr);
    write(s_console, "\n", 1, nullptr, nullptr);
    SetConsoleTextAttribute(s_console, 0);
}

}

void dlog::impl::log(dlog::impl::LogLevel level, const std::string& msg) {
    const auto now = []  {
        const auto time = std::chrono::system_clock::to_time_t(
            std::chrono::system_clock::now()
        );

        // Converting to std::tm to avoid the millisecond precision
        std::tm tm{};
        localtime_s(&tm, &time);
        return tm;
    };

    const auto time = std::format("[ {:%H:%M:%S} | SPL ] ", std::chrono::system_clock::now());
    log_raw(level, msg.c_str(), msg.size(), time.c_str(), time.size(), WriteConsoleA);
    
    impl::s_file << time << msg << '\n' << std::flush;
}

void debug::log::impl::log(LogLevel level, const std::wstring& msg) {
    const auto time = std::format(L"[ {:%T} | SPL ] ", std::chrono::system_clock::now());
    log_raw(level, msg.c_str(), msg.size(), time.c_str(), time.size(), WriteConsoleW);

    const std::string msg_utf8{ msg.begin(), msg.end() };
    const std::string time_utf8{ time.begin(), time.end() };
    impl::s_file << time_utf8 << msg_utf8 << '\n' << std::flush;
}
