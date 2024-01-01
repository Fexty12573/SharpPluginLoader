#pragma once

#include <array>
#include <string_view>

namespace config {
using namespace std::literals;
namespace detail {
// Taken from https://stackoverflow.com/questions/38955940/how-to-concatenate-static-strings-at-compile-time/62823211#62823211
template <std::wstring_view const&... Strs>
struct Join
{
    // Join all strings into a single std::array of chars
    static constexpr auto impl() noexcept
    {
        constexpr std::size_t len = (Strs.size() + ... + 0);
        std::array<wchar_t, len + 1> array{};
        auto append = [i = 0, &array](auto const& s) mutable {
            for (auto c : s) array[i++] = c;
        };
        (append(Strs), ...);
        array[len] = 0;
        return array;
    }
    // Give the joined string static storage
    static constexpr auto arr = impl();
    // View as a std::string_view
    static constexpr std::wstring_view value{ arr.data(), arr.size() - 1 };
};
template <std::wstring_view const&... Strs>
static constexpr auto concat = Join<Strs...>::value;
}

// The current version of the loader, not used for anything yet
constexpr inline auto SPL_VERSION = L"0.0.2"sv;

// The path to the loader directory
constexpr inline auto SPL_LOADER_DIR = L"nativePC/plugins/CSharp/Loader/"sv;


// The name of the log file
constexpr inline auto SPL_LOG_FILE_NAME = L"SharpPluginLoader.log"sv;

// The path to the log file
constexpr inline auto SPL_LOG_FILE = detail::concat<SPL_LOADER_DIR, SPL_LOG_FILE_NAME>;


// The name of the loader config file
constexpr inline auto SPL_RUNTIME_CONFIG_FILE_NAME = L"SharpPluginLoader.runtimeconfig.json"sv;

// The path to the .NET runtime config file
constexpr inline auto SPL_RUNTIME_CONFIG = detail::concat<SPL_LOADER_DIR, SPL_RUNTIME_CONFIG_FILE_NAME>;


#ifdef _DEBUG
// The filename of the bootstrapper assembly
constexpr inline auto SPL_BOOTSTRAPPER_ASSEMBLY_FILE_NAME = L"SharpPluginLoader.Bootstrapper.Debug.dll"sv;
#else
// The filename of the bootstrapper assembly
constexpr inline auto SPL_BOOTSTRAPPER_ASSEMBLY_FILE_NAME = L"SharpPluginLoader.Bootstrapper.dll"sv;
#endif

// The path to the loader bootstrapper
constexpr inline auto SPL_BOOTSTRAPPER_ASSEMBLY = detail::concat<SPL_LOADER_DIR, SPL_BOOTSTRAPPER_ASSEMBLY_FILE_NAME>;


#ifdef _DEBUG
// The filename of the core assembly
constexpr inline auto SPL_CORE_ASSEMBLY_FILE_NAME = L"SharpPluginLoader.Core.Debug.dll"sv;

// The name of the core assembly
constexpr inline auto SPL_CORE_ASSEMBLY_NAME = L"SharpPluginLoader.Core.Debug"sv;
#else
// The filename of the core assembly
constexpr inline auto SPL_CORE_ASSEMBLY_FILE_NAME = L"SharpPluginLoader.Core.dll"sv;

// The name of the core assembly
constexpr inline auto SPL_CORE_ASSEMBLY_NAME = L"SharpPluginLoader.Core"sv;
#endif

// The path to the loader core assembly
constexpr inline auto SPL_CORE_ASSEMBLY = detail::concat<SPL_LOADER_DIR, SPL_CORE_ASSEMBLY_FILE_NAME>;


namespace detail {

}
}