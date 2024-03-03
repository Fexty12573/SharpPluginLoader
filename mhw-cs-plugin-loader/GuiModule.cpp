#include "GuiModule.h"
#include "CoreClr.h"
#include "Config.h"
#include "Log.h"
#include "NativePluginFramework.h"

#include <dti/dti_types.h>

struct GuiElement {
    GuiElement() : m_self_n(0) {}
    void** vft = nullptr;
    const char* m_message = nullptr;
    MtSize m_position = { 1920 / 2, 1080 / 2 };
    float m_display_time = 0.0f;
    float m_delay = 0.0f;
    float m_unknown = 0.0f;
    float m_offset_y = 0.0f;
    float m_offset_x = 0.0f;
    float m_unused = 0.0f;
    float m_size_y = 0.0f;
    float m_size_x = 0.0f;
    union {
        GuiElement* m_self = nullptr;
        u64 m_self_n;
    };
    u32 m_unknown2 = 0;
};

void GuiModule::initialize(CoreClr* coreclr) {
    m_propagate_dialog_result = coreclr->get_method<void(void*, void*, int)>(
        config::SPL_CORE_ASSEMBLY_NAME,
        L"SharpPluginLoader.Core.Gui",
        L"PropagateDialogResult"
    );
    m_get_singleton = coreclr->get_method<void* (const char*)>(
        config::SPL_CORE_ASSEMBLY_NAME,
        L"SharpPluginLoader.Core.SingletonManager",
        L"GetSingletonNative"
    );

    if (m_propagate_dialog_result == nullptr) {
        dlog::error("Failed to get method SharpPluginLoader.Core.Gui.PropagateDialogResult");
    }

    const auto load_dialog_vtable = NativePluginFramework::get_repository_address("Gui:LoadDialogVTable");
    const auto vtable_offset = *(int*)load_dialog_vtable;

    const auto real_dialog_vtable = (void**)(load_dialog_vtable + 4 + vtable_offset);
    m_dialog_vtable[3] = real_dialog_vtable[3];
    m_dialog_vtable[4] = real_dialog_vtable[4];
    m_dialog_vtable[5] = real_dialog_vtable[5];

    dlog::debug("Dialog Virtual Function [3] = {:p}", m_dialog_vtable[3]);
    dlog::debug("Dialog Virtual Function [4] = {:p}", m_dialog_vtable[4]);
    dlog::debug("Dialog Virtual Function [5] = {:p}", m_dialog_vtable[5]);

    m_dialog_vtable[2] = (void*)m_propagate_dialog_result;

    coreclr->add_internal_call("QueueYesNoDialog", display_dialog);

    m_display_dialog = (decltype(m_display_dialog))NativePluginFramework::get_repository_address("Gui:DisplayYesNoDialog");
    dlog::debug("DisplayYesNoDialog = {:p}", (void*)m_display_dialog);
}

void GuiModule::shutdown() {
    m_propagate_dialog_result = nullptr;
}

void GuiModule::display_dialog(const char* message) {
    const auto& gui = NativePluginFramework::get_module<GuiModule>();
    GuiElement elements[2];

    elements[0].m_self_n = 0x10000000B;
    elements[0].m_size_y = 0.0f;
    elements[0].m_size_x = 0.0f;
    elements[0].m_offset_x = 0.0f;
    elements[0].m_offset_y = 0.0f;
    elements[0].m_unknown2 = 0x30;
    elements[0].m_message = message;
    elements[1].vft = gui->m_dialog_vtable.data();
    elements[1].m_self = &elements[1];

    gui->m_display_dialog(gui->m_get_singleton("sMhGUI"), elements);
}

GuiElement* GuiModule::gui_element_set_vtable(const GuiElement* self, GuiElement* other) {
    other->vft = NativePluginFramework::get_module<GuiModule>()->m_dialog_vtable.data();
    other->m_message = self->m_message;
    return other;
}
