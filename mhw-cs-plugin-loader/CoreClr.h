#pragma once

#include <Windows.h>
#include "coreclr_delegates.h"
#include "SharpPluginLoader.h"

#include <string_view>
#include <vector>

#ifdef _DEBUG
#define ASSEMBLY_NAME(name) name L".Debug"
#else
#define ASSEMBLY_NAME(name) name
#endif // _DEBUG


struct ManagedFunctionPointers {
    void(*Shutdown)();
    void(*TriggerOnPreMain)();
    void(*TriggerOnWinMain)();
    void(*TriggerOnMhMainCtor)();
    void(*ReloadPlugins)();
    void(*ReloadPlugin)(const char*);
};

struct InternalCall {
    const char* name;
    void* method;
};

class CoreClr {
    using BootstrapperFn = void(*)();
public:
    CoreClr();

    template<typename TFunc>
    TFunc* get_method(std::wstring_view assembly, std::wstring_view type, std::wstring_view method) const {
        return static_cast<TFunc*>(get_method_internal(assembly, type, method));
    }

    void add_internal_call(std::string_view name, void* method);
    void upload_internal_calls();

    ManagedFunctionPointers get_managed_function_pointers() const {
        return m_managed_function_pointers;
    }

    void initialize_core_assembly() const;

private:
    void* get_method_internal(std::wstring_view assembly, std::wstring_view type, std::wstring_view method) const;


private:
    HMODULE m_hostfxr = nullptr;

    load_assembly_and_get_function_pointer_fn m_load_assembly_and_get_function_pointer = nullptr;
    load_assembly_fn m_load_assembly = nullptr;
    get_function_pointer_fn m_get_function_pointer = nullptr;

    void(*m_bootstrapper_initialize)(void(*)(i32, const char*), void*) = nullptr;
    void(*m_core_initialize)() = nullptr;
    BootstrapperFn m_bootstrapper_shutdown = nullptr;
    ManagedFunctionPointers m_managed_function_pointers{};

    void(*m_upload_internal_calls)(void*, u32) = nullptr;
    void*(*m_find_core_method)(const char*, const char*) = nullptr;

    std::vector<InternalCall> m_internal_calls{};
};

