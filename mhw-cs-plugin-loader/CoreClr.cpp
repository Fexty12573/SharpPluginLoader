#include "CoreClr.h"
#include "Log.h"
#include "hostfxr.h"

#include <Windows.h>
#include <nethost.h>

#include <filesystem>

#define GET_HOSTFXR_FUNCTION(var, func) const auto var = (func##_fn)GetProcAddress(m_hostfxr, #func)

extern "C" static void public_log_interface(int level, const char* msg) {
    dlog::impl::log((dlog::impl::LogLevel)level, msg);
}

struct ManagedFunctionPointers {
    void(*Initialize)();
    void(*OnUpdate)(float);
};

CoreClr::CoreClr() {
    char_t buffer[MAX_PATH];
    size_t buffer_size = MAX_PATH;
    if (get_hostfxr_path(buffer, &buffer_size, nullptr) != 0) {
        dlog::error("Failed to get hostfxr path");
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
    HRESULT hr = initialize(L"nativePC/plugins/SharpPluginLoader.runtimeconfig.json", nullptr, &ctx);
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

#ifdef _DEBUG
    const auto bootstrapper_path = std::filesystem::absolute("nativePC/plugins/CSharp/Loader/SharpPluginLoader.Bootstrapper.Debug.dll");
#else
    const auto bootstrapper_path = std::filesystem::absolute("nativePC/plugins/CSharp/Loader/SharpPluginLoader.Bootstrapper.dll");
#endif

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

    ManagedFunctionPointers managed_function_pointers{};
    m_bootstrapper_initialize(public_log_interface, &managed_function_pointers);

    managed_function_pointers.OnUpdate(0.1f);
    managed_function_pointers.OnUpdate(0.3f);
    managed_function_pointers.OnUpdate(0.05f);
}
