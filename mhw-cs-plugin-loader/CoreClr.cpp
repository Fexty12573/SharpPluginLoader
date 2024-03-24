#include "CoreClr.h"
#include "Config.h"
#include "Log.h"
#include "hostfxr.h"

#include <Windows.h>
#include <nethost.h>

#include <filesystem>
#include <format>

#define GET_HOSTFXR_FUNCTION(var, func) const auto var = (func##_fn)GetProcAddress(m_hostfxr, #func)

extern "C" static void public_log_interface(i32 level, const char* msg) {
    dlog::impl::log((dlog::impl::LogLevel)level, msg);
}

struct ManagedFunctionPointersInternal {
    ManagedFunctionPointers PublicFunctions;
    void(*UploadInternalCalls)(void*, u32);
    void*(*FindCoreMethod)(const char*, const char*);
    void(*Initialize)();
};

CoreClr::CoreClr() {
    using namespace config;

    constexpr size_t BUFFER_SIZE = 1024;

    char_t buffer[BUFFER_SIZE];
    size_t buffer_size = BUFFER_SIZE;
    const int result = get_hostfxr_path(buffer, &buffer_size, nullptr);
    if (result != 0) {
        dlog::error("Failed to get hostfxr path: {}", result);
        return;
    }

    m_hostfxr = LoadLibraryW(buffer);
    if (m_hostfxr == nullptr) {
        dlog::error("Failed to load hostfxr: {}", GetLastError());
        return;
    }

    GET_HOSTFXR_FUNCTION(initialize, hostfxr_initialize_for_runtime_config);
    GET_HOSTFXR_FUNCTION(get_delegate, hostfxr_get_runtime_delegate);
    GET_HOSTFXR_FUNCTION(close, hostfxr_close);

    if (!initialize || !get_delegate || !close) {
        dlog::error("Failed to get hostfxr functions");
        return;
    }

    hostfxr_handle ctx = nullptr;
    HRESULT hr = initialize(SPL_RUNTIME_CONFIG.data(), nullptr, &ctx);
    if (FAILED(hr)) {
        dlog::error("Failed to initialize hostfxr: {}", hr);
        return;
    }

    hr = get_delegate(ctx, hdt_load_assembly_and_get_function_pointer, (void**)&m_load_assembly_and_get_function_pointer);
    if (FAILED(hr)) {
        dlog::error("Failed to get load_assembly_and_get_function_pointer delegate: {}", hr);
        return;
    }

    hr = get_delegate(ctx, hdt_load_assembly, (void**)&m_load_assembly);
    if (FAILED(hr)) {
        dlog::error("Failed to get load_assembly delegate: {}", hr);
        return;
    }

    hr = get_delegate(ctx, hdt_get_function_pointer, (void**)&m_get_function_pointer);
    if (FAILED(hr)) {
        dlog::error("Failed to get get_function_pointer delegate: {}", hr);
        return;
    }

    close(ctx);

    const auto bootstrapper_path = std::filesystem::absolute(SPL_BOOTSTRAPPER_ASSEMBLY);

    dlog::debug("Loading bootstrapper from {}", bootstrapper_path.string());

    // Load the bootstrapper assembly into the default ALC
    hr = m_load_assembly(bootstrapper_path.c_str(), nullptr, nullptr);
    if (FAILED(hr)) {
        dlog::error("Failed to load bootstrapper assembly: {}", hr);
        return;
    }

    hr = m_get_function_pointer(
        L"SharpPluginLoader.Bootstrapper.EntryPoint, SharpPluginLoader.Bootstrapper",
        L"Initialize",
        UNMANAGEDCALLERSONLY_METHOD,
        nullptr, nullptr, (void**)&m_bootstrapper_initialize
    );
    if (FAILED(hr)) {
        dlog::error("Failed to get bootstrapper initialize function: {}", hr);
        return;
    }

    hr = m_get_function_pointer(
        L"SharpPluginLoader.Bootstrapper.EntryPoint, SharpPluginLoader.Bootstrapper",
        L"Shutdown",
        UNMANAGEDCALLERSONLY_METHOD,
        nullptr, nullptr, (void**)&m_bootstrapper_shutdown
    );
    if (FAILED(hr)) {
        dlog::error("Failed to get bootstrapper shutdown function: {}", hr);
        return;
    }

    ManagedFunctionPointersInternal managed_function_pointers_internal{};
    m_bootstrapper_initialize(public_log_interface, &managed_function_pointers_internal);

    m_managed_function_pointers = managed_function_pointers_internal.PublicFunctions;
    m_upload_internal_calls = managed_function_pointers_internal.UploadInternalCalls;
    m_find_core_method = managed_function_pointers_internal.FindCoreMethod;
    m_core_initialize = managed_function_pointers_internal.Initialize;
}

void CoreClr::add_internal_call(std::string_view name, void* method) {
    m_internal_calls.emplace_back(name.data(), method);
}

void CoreClr::upload_internal_calls() {
    m_upload_internal_calls(m_internal_calls.data(), static_cast<u32>(m_internal_calls.size()));
    m_internal_calls.clear();
}

void CoreClr::initialize_core_assembly() const {
    m_core_initialize();
}

void* CoreClr::get_method_internal(std::wstring_view assembly, std::wstring_view type, std::wstring_view method) const {
    void* function_pointer = nullptr;

    if (assembly.starts_with(L"SharpPluginLoader.Core")) {
        const std::string type_utf8{ type.begin(), type.end() };
        const std::string method_utf8{ method.begin(), method.end() };
        return m_find_core_method(type_utf8.c_str(), method_utf8.c_str());
    }

    const auto qualified_name = std::format(L"{}, {}", type, assembly);
    dlog::debug(L"Getting function pointer for {} -> {}", qualified_name, method);
    const auto hr = m_get_function_pointer(qualified_name.c_str(), method.data(), UNMANAGEDCALLERSONLY_METHOD, nullptr, nullptr, &function_pointer);
    if (FAILED(hr)) {
        dlog::debug(L"Failed to get function pointer for {}.{}: {}", type, method, hr);
        return nullptr;
    }
    
    return function_pointer;
}
