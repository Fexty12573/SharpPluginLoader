#include "NativePluginFramework.h"

#include "CoreModule.h"
#include "GuiModule.h"

NativePluginFramework::NativePluginFramework(CoreClr* coreclr)
    : m_managed_functions(coreclr->get_managed_function_pointers()) {

    s_instance = this;
    m_modules.push_back(std::make_shared<CoreModule>());
    m_modules.push_back(std::make_shared<GuiModule>());

    for (const auto& module : m_modules) {
        module->initialize(coreclr);
    }
}
