#include "Bitfield.h"

#include <imgui_impl.h>


// Taken from, and slightly modified:
// https://gist.github.com/JSandusky/af0e94011aee31f7b05ed2257d347637
bool ImGui::Bitfield(const char* label, u32* bits, u32* hover_index) {
    ImGuiWindow* window = igGetCurrentWindow();
    if (window->SkipItems)
        return false;

    const ImGuiWindowFlags old_flags = window->Flags;
    const ImGuiStyle& style = *igGetStyle();
    const ImGuiID id = ImGuiWindow_GetID_Str(window, label, nullptr);
    ImVec2 label_size;
    igCalcTextSize(&label_size, label, nullptr, true, -1.0f);
    const ImVec2 small_label_size = { label_size.x * 0.5f, label_size.y * 0.5f };

    constexpr float spacing_unit = 2.0f;

    bool any_pressed = false;
    ImVec2 current_pos = window->DC.CursorPos;
    for (unsigned i = 0; i < 32; ++i) {
        const void* lbl = (void*)(label + i);
        const ImGuiID local_id = ImGuiWindow_GetID_Ptr(window, lbl);
        if (i == 16) {
            current_pos.x = window->DC.CursorPos.x;
            current_pos.y += small_label_size.y + style.FramePadding.y * 2 + spacing_unit /*little bit of space*/;
        }
        if (i == 8 || i == 24)
            current_pos.x += small_label_size.y;

        const ImRect check_bb(current_pos, {
            current_pos.x + small_label_size.y + style.FramePadding.y * 2,
            current_pos.y + small_label_size.y + style.FramePadding.y * 2
        });

        bool hovered, held;
        const bool pressed = igButtonBehavior(check_bb, local_id, &hovered, &held, ImGuiButtonFlags_PressedOnClick);
        if (pressed)
            *bits ^= 1 << i;

        if (hovered && hover_index)
            *hover_index = i;
        
        igRenderFrame(
            check_bb.Min, 
            check_bb.Max, 
            igGetColorU32_Col(
                held && hovered 
                ? ImGuiCol_FrameBgActive 
                : hovered ? ImGuiCol_FrameBgHovered : ImGuiCol_FrameBg, 1.0f), 
            true, 
            0.0f
        );

        if (*bits & (1 << i)) {
            const float check_sz = ImMin(check_bb.GetWidth(), check_bb.GetHeight());
            const float pad = ImMax(spacing_unit, (float)(int)(check_sz / 4.0f));
            ImDrawList_AddRectFilled(
                window->DrawList,
                { check_bb.Min.x + pad, check_bb.Min.y + pad },
                { check_bb.Max.x - pad, check_bb.Max.y - pad },
                igGetColorU32_Col(ImGuiCol_CheckMark, 1.0f),
                0.0f, 0
            );
        }

        any_pressed |= pressed;
        current_pos.x = check_bb.Max.x + spacing_unit;
    }

    const ImRect matrix_bb(window->DC.CursorPos,
        { window->DC.CursorPos.x + (small_label_size.y + style.FramePadding.y * 2) * 16 /*# of checks in a row*/ + small_label_size.y /*space between sets of 8*/ + 15 * spacing_unit /*spacing between each check*/,
          window->DC.CursorPos.y + ((small_label_size.y + style.FramePadding.y * 2) * 2 /*# of rows*/ + spacing_unit /*spacing between rows*/) });

    igItemSize_Rect(matrix_bb, style.FramePadding.y);

    ImRect total_bb = matrix_bb;

    if (label_size.x > 0)
        igSameLine(0, style.ItemInnerSpacing.x);

    const ImRect text_bb({ window->DC.CursorPos.x, window->DC.CursorPos.y + style.FramePadding.y }, { window->DC.CursorPos.x + label_size.x, window->DC.CursorPos.y + style.FramePadding.y + label_size.y });
    if (label_size.x > 0) {
        igItemSize_Vec2(ImVec2(text_bb.GetWidth(), matrix_bb.GetHeight()), style.FramePadding.y);
        total_bb = ImRect(ImMin(matrix_bb.Min, text_bb.Min), ImMax(matrix_bb.Max, text_bb.Max));
    }

    if (!igItemAdd(total_bb, id, nullptr, 0))
        return false;

    if (label_size.x > 0.0f)
        igRenderText(text_bb.GetTL(), label, nullptr, true);

    window->Flags = old_flags;
    return any_pressed;
}
