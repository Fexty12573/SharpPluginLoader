#pragma once

#include <Windows.h>
#include "coreclr_delegates.h"

class CoreClr {
    using BootstrapperFn = void(*)();
public:
    CoreClr();

private:
    HMODULE m_hostfxr = nullptr;

    load_assembly_and_get_function_pointer_fn m_load_assembly_and_get_function_pointer = nullptr;
    load_assembly_fn m_load_assembly = nullptr;
    get_function_pointer_fn m_get_function_pointer = nullptr;

    void(*m_bootstrapper_initialize)(void(*)(int, const char*), void*) = nullptr;
    BootstrapperFn m_bootstrapper_shutdown = nullptr;
};

