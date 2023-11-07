#pragma once
#include "NativeModule.h"

#include <array>

struct GuiElement;

class GuiModule final : public NativeModule {
public:
    void initialize(CoreClr* coreclr) override;
    void shutdown() override;

private:
    static void display_dialog(const char* message);
    static GuiElement* gui_element_set_vtable(const GuiElement* self, GuiElement* other);

private:
    void(*m_propagate_dialog_result)(void*, void*, int) = nullptr;

    std::array<void*, 6> m_dialog_vtable = {
        (void*)gui_element_set_vtable,
        (void*)gui_element_set_vtable,
        nullptr, // Assigned in initialize
        (void*)0x141ec9790,
        (void*)0x1402405c0,
        (void*)0x1402406b0
    };
};

