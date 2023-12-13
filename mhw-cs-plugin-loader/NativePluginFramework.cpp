#include "NativePluginFramework.h"

#include "ChunkModule.h"
#include "CoreModule.h"
#include "D3DModule.h"
#include "GuiModule.h"
#include "ImGuiModule.h"

NativePluginFramework::NativePluginFramework(CoreClr* coreclr)
    : m_managed_functions(coreclr->get_managed_function_pointers()) {

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

    coreclr->upload_internal_calls();
    coreclr->initialize_core_assembly();
    m_managed_functions.LoadPlugins();
}
