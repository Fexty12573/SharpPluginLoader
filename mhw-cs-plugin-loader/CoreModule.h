#pragma once

#include "NativeModule.h"

#include <safetyhook/safetyhook.hpp>
#include <dti/sMain.h>


class CoreModule final : public NativeModule {
public:
    void initialize(CoreClr* coreclr) override;
    void shutdown() override;

private:
    static void main_update_hook(const sMain* main);

private:
    void(*m_plugin_on_update)(float) = nullptr;

    safetyhook::InlineHook m_main_update_hook;
};

