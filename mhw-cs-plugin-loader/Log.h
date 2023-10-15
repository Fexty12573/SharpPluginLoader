#pragma once

#include <loader.h>
#include <format>

namespace debug::log {
namespace impl {

enum LogLevel {
    Debug = FOREGROUND_GREEN | FOREGROUND_INTENSITY,
    Info = FOREGROUND_RED | FOREGROUND_GREEN | FOREGROUND_BLUE,
    Warn = FOREGROUND_RED | FOREGROUND_GREEN | FOREGROUND_INTENSITY,
    Error = FOREGROUND_RED | FOREGROUND_INTENSITY
};

void log(LogLevel level, const std::string& msg);
void log(LogLevel level, const std::wstring& msg);

}

template<typename ...Args>
void debug(const std::format_string<Args...>& fmt, Args... args) {
    impl::log(impl::LogLevel::Debug, std::vformat(fmt.get(), std::make_format_args(args...)));
}

template<typename ...Args>
void info(const std::format_string<Args...>& fmt, Args... args) {
    impl::log(impl::LogLevel::Info, std::vformat(fmt.get(), std::make_format_args(args...)));
}

template<typename ...Args>
void warn(const std::format_string<Args...>& fmt, Args... args) {
    impl::log(impl::LogLevel::Warn, std::vformat(fmt.get(), std::make_format_args(args...)));
}

template<typename ...Args>
void error(const std::format_string<Args...>& fmt, Args&&... args) {
    impl::log(impl::LogLevel::Error, std::vformat(fmt.get(), std::make_format_args(args...)));
}

template<typename ...Args>
void debug(const std::wformat_string<Args...>& fmt, Args... args) {
    impl::log(impl::LogLevel::Debug, std::vformat(fmt.get(), std::make_wformat_args(args...)));
}

template<typename ...Args>
void info(const std::wformat_string<Args...>& fmt, Args... args) {
    impl::log(impl::LogLevel::Info, std::vformat(fmt.get(), std::make_wformat_args(args...)));
}

template<typename ...Args>
void warn(const std::wformat_string<Args...>& fmt, Args... args) {
    impl::log(impl::LogLevel::Warn, std::vformat(fmt.get(), std::make_wformat_args(args...)));
}

template<typename ...Args>
void error(const std::wformat_string<Args...>& fmt, Args&&... args) {
    impl::log(impl::LogLevel::Error, std::vformat(fmt.get(), std::make_wformat_args(args...)));
}

}

namespace dlog = debug::log;
