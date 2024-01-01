#include "CoreModule.h"
#include "CoreClr.h"
#include "Config.h"
#include "Log.h"
#include "NativePluginFramework.h"

#include <utility/game_functions.h>

static void test_icall() {
    dlog::info("Hello from internal call!");
}

void CoreModule::initialize(CoreClr* coreclr) {
    m_plugin_on_update = coreclr->get_method<void(float)>(
        config::SPL_CORE_ASSEMBLY_NAME,
        L"SharpPluginLoader.Core.NativeInterface",
        L"OnUpdate"
    );

    dlog::debug("Retrieved OnUpdate: {:p}, Adding internal call...", (void*)m_plugin_on_update);
    coreclr->add_internal_call("TestInternalCall", test_icall);

    m_main_update_hook = safetyhook::create_inline(MH::sMhMain::move, main_update_hook);
}

void CoreModule::shutdown() {
    m_plugin_on_update = nullptr;
}

void CoreModule::main_update_hook(const sMain* main) {
    const auto& self = NativePluginFramework::get_module<CoreModule>();
    self->m_plugin_on_update(main->mDeltaSec);

    return self->m_main_update_hook.call(main);
}
