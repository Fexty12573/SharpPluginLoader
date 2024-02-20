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
    if (s_instance->m_game_revision != nullptr) {
        return s_instance->m_game_revision;
    }
    
    const auto pattern = Pattern::from_string("48 83 EC 48 48 8B 05 ? ? ? ? 4C 8D 0D ? ? ? ? BA 0A 00 00 00");
    const auto func = PatternScanner::find_first(pattern);

    if (func == 0) {
        dlog::error("Failed to find game revision function");
        return nullptr;
    }

    const auto constant_offset = *reinterpret_cast<i32*>(func + 7);
    const uintptr_t offset_base = func + 11;
    s_instance->m_game_revision = *reinterpret_cast<const char**>(offset_base + constant_offset);

    dlog::debug("Game revision: {}", s_instance->m_game_revision);

    return s_instance->m_game_revision;
}
