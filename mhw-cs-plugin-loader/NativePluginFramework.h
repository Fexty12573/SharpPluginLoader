#pragma once

#include "CoreClr.h"
#include "NativeModule.h"

#include <vector>
#include <concepts>
#include <memory>


class NativePluginFramework {
public:
    static NativePluginFramework* get() {
        return s_instance;
    }

    template<class T> requires std::derived_from<T, NativeModule>
    static std::shared_ptr<T> get_module() {
        for (auto& module : get()->m_modules) {
            if (auto instance = std::dynamic_pointer_cast<T>(module)) {
                return instance;
            }
        }

        return nullptr;
    }

    explicit NativePluginFramework(CoreClr* coreclr);

private:
    std::vector<std::shared_ptr<NativeModule>> m_modules;
    ManagedFunctionPointers m_managed_functions;

    static inline NativePluginFramework* s_instance = nullptr;
};

