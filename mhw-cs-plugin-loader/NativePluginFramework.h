#pragma once

#include "CoreClr.h"
#include "NativeModule.h"
#include "AddressRepository.h"

#include <vector>
#include <concepts>
#include <memory>
#include <stdexcept>


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

        throw std::runtime_error("Module not found");
    }

    explicit NativePluginFramework(CoreClr* coreclr, AddressRepository* address_repository);

    void trigger_on_pre_main();
    void trigger_on_win_main();
    void trigger_on_mh_main_ctor();

    static uintptr_t get_repository_address(const char* name);
    static const char* get_game_revision();

private:
    std::vector<std::shared_ptr<NativeModule>> m_modules;
    ManagedFunctionPointers m_managed_functions;
    const char* m_game_revision = nullptr;
    AddressRepository* m_address_repository = nullptr;

    static inline NativePluginFramework* s_instance = nullptr;
};

