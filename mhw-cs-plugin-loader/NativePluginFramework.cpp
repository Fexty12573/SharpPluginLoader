#include "NativePluginFramework.h"

#include "Log.h"
#include "ChunkModule.h"
#include "CoreModule.h"
#include "D3DModule.h"
#include "GuiModule.h"
#include "ImGuiModule.h"
#include "PatternScan.h"

NativePluginFramework::NativePluginFramework(CoreClr* coreclr, AddressRepository* address_repository)
    : m_managed_functions(coreclr->get_managed_function_pointers()),
      m_address_repository(address_repository) {

    s_instance = this;
    m_modules.push_back(std::make_shared<CoreModule>());
    m_modules.push_back(std::make_shared<GuiModule>());
    m_modules.push_back(std::make_shared<D3DModule>());
    m_modules.push_back(std::make_shared<ChunkModule>());
    m_modules.push_back(std::make_shared<ImGuiModule>());
    m_modules.push_back(std::make_shared<PrimitiveRenderingModule>());

    for (const auto& module : m_modules) {
        module->initialize(coreclr);
    }

    // TODO(andoryuuta): should this be a full "Module" instead?
    coreclr->add_internal_call("GetRepositoryAddress", get_repository_address);
    coreclr->add_internal_call("GetGameRevision", get_game_revision);
    coreclr->upload_internal_calls();
    coreclr->initialize_core_assembly();
}

void NativePluginFramework::trigger_on_pre_main() {
    m_managed_functions.TriggerOnPreMain();
}

void NativePluginFramework::trigger_on_win_main() {
    m_managed_functions.TriggerOnWinMain();
}

void NativePluginFramework::trigger_on_mh_main_ctor() {
    m_managed_functions.TriggerOnMhMainCtor();
}

uintptr_t NativePluginFramework::get_repository_address(const char* name) {
    return s_instance->m_address_repository->get(name);
}

const char* NativePluginFramework::get_game_revision() {
    return s_instance->m_address_repository->get_game_revision();
}
