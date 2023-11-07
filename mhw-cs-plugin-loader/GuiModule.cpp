#include "GuiModule.h"
#include "CoreClr.h"
#include "Log.h"
#include "NativePluginFramework.h"

#include <game_functions.h>
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
        ASSEMBLY_NAME(L"SharpPluginLoader.Core"),
        L"SharpPluginLoader.Core.Gui",
        L"PropagateDialogResult"
    );

    if (m_propagate_dialog_result == nullptr) {
        dlog::error("Failed to get method SharpPluginLoader.Core.Gui.PropagateDialogResult");
    }

    m_dialog_vtable[2] = (void*)m_propagate_dialog_result;

    coreclr->add_internal_call("QueueYesNoDialog", display_dialog);
    coreclr->upload_internal_calls();
}

void GuiModule::shutdown() {
    m_propagate_dialog_result = nullptr;
}

void GuiModule::display_dialog(const char* message) {
    GuiElement elements[2];

    elements[0].m_self_n = 0x10000000B;
    elements[0].m_size_y = 0.0f;
    elements[0].m_size_x = 0.0f;
    elements[0].m_offset_x = 0.0f;
    elements[0].m_offset_y = 0.0f;
    elements[0].m_unknown2 = 0x30;
    elements[0].m_message = message;
    elements[1].vft = NativePluginFramework::get_module<GuiModule>()->m_dialog_vtable.data();
    elements[1].m_self = &elements[1];

    MH::sMhGUI::DisplayYesNoDialog(MH::sMhGUI::GetInstance(), elements);
}

GuiElement* GuiModule::gui_element_set_vtable(const GuiElement* self, GuiElement* other) {
    other->vft = NativePluginFramework::get_module<GuiModule>()->m_dialog_vtable.data();
    other->m_message = self->m_message;
    return other;
}
