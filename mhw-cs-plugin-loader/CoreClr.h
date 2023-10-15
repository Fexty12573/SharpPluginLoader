#pragma once

#include <Windows.h>
#include "coreclr_delegates.h"

#include <string_view>

class CoreClr {
    using BootstrapperFn = void(*)();
public:
    CoreClr();

    template<typename TFunc>
    TFunc* get_method(std::wstring_view assembly, std::wstring_view type, std::wstring_view method) const {
        return static_cast<TFunc*>(get_method_internal(assembly, type, method));
    }

private:
    void* get_method_internal(std::wstring_view assembly, std::wstring_view type, std::wstring_view method) const;

private:
    HMODULE m_hostfxr = nullptr;

    load_assembly_and_get_function_pointer_fn m_load_assembly_and_get_function_pointer = nullptr;
    load_assembly_fn m_load_assembly = nullptr;
    get_function_pointer_fn m_get_function_pointer = nullptr;

    void(*m_bootstrapper_initialize)(void(*)(int, const char*), void*) = nullptr;
    BootstrapperFn m_bootstrapper_shutdown = nullptr;
};

