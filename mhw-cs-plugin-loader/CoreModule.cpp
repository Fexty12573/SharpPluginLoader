#include "CoreModule.h"
#include "CoreClr.h"
#include "Config.h"
#include "Log.h"
#include "NativePluginFramework.h"

void CoreModule::initialize(CoreClr* coreclr) {
    m_plugin_on_update = coreclr->get_method<void(float)>(
        config::SPL_CORE_ASSEMBLY_NAME,
        L"SharpPluginLoader.Core.NativeInterface",
        L"OnUpdate"
    );

    const auto update = (void*)NativePluginFramework::get_repository_address("Main:Update");
    m_main_update_hook = safetyhook::create_inline(update, main_update_hook);

    dlog::debug("sMhMain::move: {:p}", update);
}

void CoreModule::shutdown() {
    m_plugin_on_update = nullptr;
}

void CoreModule::main_update_hook(const sMain* main) {
    const auto& self = NativePluginFramework::get_module<CoreModule>();
    self->m_plugin_on_update(main->mDeltaSec);

    return self->m_main_update_hook.call(main);
}
