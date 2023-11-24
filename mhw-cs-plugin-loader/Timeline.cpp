#include "Timeline.h"

#include <imgui_impl.h>
#include <unordered_map>

#include <format>
#include <string>

struct ImTimelineKeyframe {
    int TrackIndex = 0;
    float Frame = 0.0f;
    // The track that this keyframe belongs to
    ImGuiID TrackId = 0;
    // The index of the keyframe in the track
    int Index = 0;
};

struct ImTimelineContext {
    ImTimelineKeyframe SelectedKeyframe;

    // The scroll position of the timeline, normalized to [0, 1]
    float Scroll = 0.0f;
    float Zoom = 1.0f;
    float YPos = 0.0f;
    float HeaderYPos = 0.0f;
    float CurrentTrackYPos = 0.0f;
    float FirstTrackYPos = 0.0f;

    // The index of the track in the current group
    int CurrentTrackIndex = 0;
    int TrackCount = 0;

    // Initial scroll position when dragging
    float InitialScroll = 0.0f;
    float InitialFrame = 0.0f;
    ImGuiTimelineFlags Flags = 0;

    // The frame range passed to BeginTimeline
    float StartFrame = 0.0f;
    float EndFrame = 0.0f;

    int FirstVisibleFrame = 0;
    int LastVisibleFrame = 0;
    int VisibleFrameCount = 0;
    float FrameWidth = 0.0f;
    float TimelineXPos = 0.0f;
    float TimelineWidth = 0.0f;

    float FramePointerXPos = 0.0f;
    float FramePointerYPos = 0.0f;

    float CurrentFrame = 0.0f;

    // Indent level for labels, always at least 1. BeginGroup increments this, EndGroup decrements it.
    int IndentLevel = 1;

    ImVec2 MouseDragDelta;
    ImVec2 MouseClickStartPos;
};

static constexpr float TIMELINE_SCROLL_BAR_HEIGHT = 20.0f;
static constexpr float TIMELINE_HEADER_BASE_HEIGHT = 20.0f;
static constexpr float TIMELINE_HEADER_TOP_PADDING = 5.0f;
static constexpr float TIMELINE_TRACK_BASE_HEIGHT = 20.0f;

static constexpr float TIMELINE_TRACK_RIGHT_EXTRA_PADDING = 3.0f;
static constexpr float TIMELINE_TRACK_LABEL_TO_TRACK_WIDTH_RATIO = 0.2f;
static constexpr float TIMELINE_TRACK_GROUP_ARROW_SIZE = 10.0f;
static constexpr float TIMELINE_TRACK_LABEL_INDENT = TIMELINE_TRACK_GROUP_ARROW_SIZE;

static constexpr float FRAME_POINTER_WIDTH = 9.0f;
static constexpr float FRAME_POINTER_HEIGHT = 15.0f;

static constexpr ImU32 FRAME_POINTER_COLOR = IM_COL32(204, 160, 29, 255);
static constexpr ImU32 FRAME_POINTER_HOVERED_COLOR = IM_COL32(232, 188, 56, 255);
static constexpr ImU32 FRAME_POINTER_ACTIVE_COLOR = IM_COL32(194, 145, 0, 255);

#define GET_ACTIVE_HOVERED_COLOR(active, hovered, color) \
    ((active) ? color##Active : (hovered) ? color##Hovered : (color))

static std::unordered_map<ImGuiID, ImTimelineContext> g_TimelineContexts;
static ImGuiID g_CurrentTimelineId = 0;
static bool g_InTimeline = false;

static float GetScrollBarWidth(float avail_width, float zoom) {
    return avail_width * zoom;
}

namespace ImGui {
inline ImVec2 CalcTextSize(const char* text, const char* text_end = nullptr, bool hide_text_after_double_hash = false, float wrap_width = -1.0f) {
    ImVec2 out;
    igCalcTextSize(&out, text, text_end, hide_text_after_double_hash, wrap_width);
    return out;
}
inline ImVec2 GetContentRegionAvail() {
    ImVec2 out;
    igGetContentRegionAvail(&out);
    return out;
}
inline ImVec2 GetMouseDragDelta(ImGuiMouseButton button = 0, float lock_threshold = -1.0f) {
    ImVec2 out;
    igGetMouseDragDelta(&out, button, lock_threshold);
    return out;
}
inline ImVec2 GetMousePos() {
    ImVec2 out;
    igGetMousePos(&out);
    return out;
}
}

// Returns the size of the text after cropping it to fit in max_width
// and a pointer to the last character+1 that can be displayed (or nullptr if the entire text fits)
static std::pair<ImVec2, const char*> TrimTextAndGetSize(std::string_view in_text, float max_width) {
    const auto& style = *igGetStyle();
    const ImVec2 text_size = ImGui::CalcTextSize(in_text.data());
    const float text_width = text_size.x;

    if (text_width <= max_width) {
        return { text_size, nullptr };
    }

    constexpr auto ellipsis = "...";
    const ImVec2 ellipsis_size = ImGui::CalcTextSize(ellipsis);
    const float ellipsis_width = ellipsis_size.x;

    const float max_text_width = max_width - ellipsis_width;
    const ImVec2 max_text_size = ImGui::CalcTextSize(in_text.data(), in_text.data() + in_text.size(),
        false, max_text_width);
    const char* max_text_end = in_text.data() + (int)(max_text_size.x / text_width * in_text.size());

    return { ImVec2{ max_text_size.x, 0.0f } + ellipsis_size, max_text_end };
}

bool ImGui::BeginTimeline(std::string_view label, float start_frame, float end_frame, float* p_current_frame, ImGuiTimelineFlags flags) {
    IM_ASSERT(p_current_frame != nullptr && "A pointer to the current frame value must be provided");
    IM_ASSERT(start_frame < end_frame && "Start frame must be less that end frame");
    IM_ASSERT(!g_InTimeline && "Timeline Begin/End mismatch. You probably forgot a call to EndTimeline");

    const bool child_open = igBeginChild_Str(std::format("##{}_child", label).c_str(), {}, 0, 0);
    if (!child_open) {
        return false;
    }

    const ImGuiID id = igGetID_Str(label.data());
    igPushOverrideID(id);

    g_InTimeline = true;
    g_CurrentTimelineId = id;
    ImTimelineContext& ctx = g_TimelineContexts[id];
    ctx.StartFrame = start_frame;
    ctx.EndFrame = end_frame;
    ctx.Flags = flags;
    ctx.CurrentTrackIndex = 0;
    ctx.TrackCount = 0;
    ctx.IndentLevel = 1;

    const auto& io = *igGetIO();
    const auto& style = *igGetStyle();
    const auto window = igGetCurrentWindow();
    const auto draw_list = window->DrawList;
    const ImVec2 cursor_pos = window->DC.CursorPos;

    const ImVec2 avail = GetContentRegionAvail();
    const float scrollbar_width = GetScrollBarWidth(avail.x, ctx.Zoom);
    const float scroll_x = cursor_pos.x + ctx.Scroll * (avail.x - scrollbar_width);

    // Allocate extra space for the frame labels
    const ImVec2 frame_label_size = CalcTextSize("100");
    const float timeline_track_right_padding = frame_label_size.x + TIMELINE_TRACK_RIGHT_EXTRA_PADDING;
    const float timeline_header_height = TIMELINE_HEADER_BASE_HEIGHT + frame_label_size.y;

    ctx.YPos = cursor_pos.y;
    ctx.HeaderYPos = ctx.YPos + TIMELINE_SCROLL_BAR_HEIGHT;
    ctx.CurrentTrackYPos = ctx.FirstTrackYPos = ctx.HeaderYPos + timeline_header_height;

    // Draw header
    ImDrawList_AddRectFilled(draw_list,
        ImVec2(cursor_pos.x, ctx.YPos),
        ImVec2(cursor_pos.x + avail.x, ctx.YPos + TIMELINE_SCROLL_BAR_HEIGHT),
        igGetColorU32_Col(ImGuiCol_ScrollbarBg, 1.0f),
        0.0f, 0
    );

    const ImRectExt header_rect = {
        ImVec2(cursor_pos.x, ctx.YPos),
        ImVec2(cursor_pos.x + avail.x, ctx.YPos + TIMELINE_SCROLL_BAR_HEIGHT)
    };
    const ImRect scrollbar_rect = {
        ImVec2(scroll_x, ctx.YPos),
        ImVec2(scroll_x + scrollbar_width, ctx.YPos + TIMELINE_SCROLL_BAR_HEIGHT)
    };

    // Scrollbar Item
    igItemSize_Rect(header_rect, 0.0f);
    if (!igItemAdd(scrollbar_rect, id, nullptr, 0)) {
        return false;
    }

    const auto scrollbar_hovered = igIsItemHovered(0);
    const auto scrollbar_active = igIsItemActive();
    const auto scrollbar_clicked = scrollbar_hovered && igIsMouseClicked_Bool(0, false);

    if (scrollbar_clicked) {
        igSetActiveID(id, window);
        igFocusWindow(window, 0);
    } else if (igIsMouseReleased_Nil(0) && scrollbar_active) {
        igClearActiveID();
    }

    const auto scrollbar_color = GET_ACTIVE_HOVERED_COLOR(scrollbar_active, scrollbar_hovered, ImGuiCol_ScrollbarGrab);

    ImDrawList_AddRectFilled(draw_list,
        ImVec2(scroll_x, ctx.YPos),
        ImVec2(scroll_x + scrollbar_width, ctx.YPos + TIMELINE_SCROLL_BAR_HEIGHT),
        igGetColorU32_Col(scrollbar_color, 1.0f),
        style.ScrollbarRounding, 0
    );

    // Border
    //draw_list->AddRect(
    //    ImVec2(cursor_pos.x, ctx.YPos),
    //    ImVec2(cursor_pos.x + avail.x, ctx.YPos + TIMELINE_SCROLL_BAR_HEIGHT),
    //    GetColorU32(ImGuiCol_Border)
    //);

    // Handle Zoom
    const auto mouse_pos = io.MousePos;
    if (header_rect.Contains(mouse_pos)) {
        if (io.MouseWheel != 0.0f) {
            ctx.Zoom += io.MouseWheel * 0.1f;
            ctx.Zoom = ImClamp(ctx.Zoom, 0.1f, 1.0f);
        }
    }

    // Handle Scroll
    if (igIsItemClicked(0)) {
        ctx.InitialScroll = ctx.Scroll;
    }

    const auto mouse_drag_delta = GetMouseDragDelta(0, 0.0f);
    ctx.MouseDragDelta = mouse_drag_delta;
    if (scrollbar_active && ctx.Zoom < 1.0f) {
        const auto mouse_delta = mouse_drag_delta.x / (avail.x - scrollbar_width);
        ctx.Scroll = ctx.InitialScroll + mouse_delta;
        ctx.Scroll = ImClamp(ctx.Scroll, 0.0f, 1.0f);
    }

    // Draw Timeline Header
    // Background
    const ImRect timeline_header_rect = {
        ImVec2(cursor_pos.x, ctx.HeaderYPos),
        ImVec2(cursor_pos.x + avail.x, ctx.HeaderYPos + timeline_header_height)
    };
    igItemSize_Rect(timeline_header_rect, 0.0f);
    ImDrawList_AddRectFilled(draw_list,
        timeline_header_rect.Min,
        timeline_header_rect.Max,
        igGetColorU32_Col(ImGuiCol_ScrollbarBg, 1.0f),
        0.0f, 0
    );

    // Markers
    const float timeline_x_pos = cursor_pos.x + TIMELINE_TRACK_LABEL_TO_TRACK_WIDTH_RATIO * avail.x;
    const float timeline_avail_x = (cursor_pos.x + avail.x) - timeline_x_pos - timeline_track_right_padding;
    const float frame_count = end_frame - start_frame;
    const int frame_count_visible = frame_count * ctx.Zoom;
    const int first_frame = start_frame + (frame_count - frame_count_visible) * ctx.Scroll;
    const int last_frame = first_frame + frame_count_visible;
    const float frame_width = timeline_avail_x / frame_count_visible;

    // -5 because frame markers shouldn't touch the scrollbar
    constexpr float frame_marker_height = TIMELINE_HEADER_BASE_HEIGHT - TIMELINE_HEADER_TOP_PADDING;
    const float frame_marker_y_pos = ctx.HeaderYPos + TIMELINE_HEADER_BASE_HEIGHT;

    for (int frame = first_frame; frame <= last_frame; ++frame) {
        const float frame_x_pos = timeline_x_pos + (frame - first_frame) * frame_width;

        // Draw frame marker
        const float line_end_y_pos = frame % 10 == 0
            ? frame_marker_y_pos - frame_marker_height
            : frame_marker_y_pos - frame_marker_height * 0.75f;

        ImDrawList_AddLine(draw_list,
            ImVec2(frame_x_pos, frame_marker_y_pos),
            ImVec2(frame_x_pos, line_end_y_pos),
            igGetColorU32_Col(ImGuiCol_Border, 1.0f),
            1.0f
        );

        // Draw frame number
        if (frame == first_frame || frame == last_frame || frame % 10 == 0) {
            const std::string frame_number = std::to_string(frame);
            const ImVec2 frame_number_size = CalcTextSize(frame_number.c_str());
            const ImVec2 frame_number_pos = { frame_x_pos - frame_number_size.x * 0.5f, frame_marker_y_pos };
            ImDrawList_AddText_Vec2(draw_list, frame_number_pos, igGetColorU32_Col(ImGuiCol_Text, 1.0f), frame_number.c_str(), nullptr);
        }
    }

    ctx.FirstVisibleFrame = first_frame;
    ctx.LastVisibleFrame = last_frame;
    ctx.VisibleFrameCount = frame_count_visible;
    ctx.FrameWidth = frame_width;
    ctx.TimelineXPos = timeline_x_pos;
    ctx.TimelineWidth = timeline_avail_x;

    // Draw Current Frame Pointer
    const float current_frame = *p_current_frame;
    if (current_frame >= (float)first_frame && current_frame <= (float)last_frame) {
        // Visuals
        const float current_frame_x_pos = timeline_x_pos + (current_frame - first_frame) * frame_width;
        const float current_frame_y_pos = ctx.HeaderYPos + TIMELINE_HEADER_TOP_PADDING;

        constexpr float frame_pointer_rect_height = FRAME_POINTER_HEIGHT - 4.0f;
        constexpr float frame_pointer_triangle_height = FRAME_POINTER_HEIGHT - frame_pointer_rect_height;
        const float frame_pointer_triangle_y_pos = current_frame_y_pos + frame_pointer_rect_height;

        ctx.FramePointerXPos = current_frame_x_pos;
        ctx.FramePointerYPos = frame_pointer_triangle_y_pos + frame_pointer_triangle_height;

        const float frame_pointer_x_start = current_frame_x_pos - FRAME_POINTER_WIDTH * 0.5f;
        const float frame_pointer_x_end = current_frame_x_pos + FRAME_POINTER_WIDTH * 0.5f;

        const ImGuiID frame_pointer_id = igGetID_Str("##frame_pointer");
        const ImRect frame_pointer_rect = {
            ImVec2(frame_pointer_x_start, current_frame_y_pos),
            ImVec2(frame_pointer_x_end, frame_pointer_triangle_y_pos + frame_pointer_triangle_height)
        };

        if (!igItemAdd(frame_pointer_rect, frame_pointer_id, nullptr, 0)) {
            return false;
        }

        const bool frame_pointer_hovered = igIsItemHovered(0);
        const bool frame_pointer_active = igIsItemActive();
        const bool frame_pointer_clicked = frame_pointer_hovered && igIsMouseClicked_Bool(0, false);
        const ImU32 frame_pointer_color = frame_pointer_active
            ? FRAME_POINTER_ACTIVE_COLOR
            : frame_pointer_hovered
            ? FRAME_POINTER_HOVERED_COLOR
            : FRAME_POINTER_COLOR;

        ImDrawList_AddRectFilled(draw_list,
            ImVec2(frame_pointer_x_start, current_frame_y_pos),
            ImVec2(frame_pointer_x_end, current_frame_y_pos + frame_pointer_rect_height),
            frame_pointer_color,
            0.0f, 0
        );
        ImDrawList_AddTriangleFilled(draw_list,
            ImVec2(frame_pointer_x_start, frame_pointer_triangle_y_pos),
            ImVec2(frame_pointer_x_end, frame_pointer_triangle_y_pos),
            ImVec2(current_frame_x_pos, frame_pointer_triangle_y_pos + frame_pointer_triangle_height),
            frame_pointer_color
        );

        // Behavior
        if (frame_pointer_clicked) {
            igSetActiveID(frame_pointer_id, window);
            igFocusWindow(window, 0);
            ctx.InitialFrame = current_frame;
        }
        if (igIsItemActive()) {
            if (igIsMouseReleased_Nil(0)) {
                igClearActiveID();
            }

            ctx.CurrentFrame = ctx.InitialFrame + mouse_drag_delta.x / frame_width;
            ctx.CurrentFrame = ImClamp(ctx.CurrentFrame, (float)first_frame, (float)last_frame);
            if (flags & ImGuiTimelineFlags_EnableFramePointerSnapping) {
                ctx.CurrentFrame = std::roundf(ctx.CurrentFrame);
            }

            *p_current_frame = ctx.CurrentFrame;
        }
    }

    return true;
}

void ImGui::EndTimeline() {
    const auto& ctx = g_TimelineContexts[g_CurrentTimelineId];
    const auto draw_list = igGetCurrentWindow()->DrawList;

    if (ctx.Flags & ImGuiTimelineFlags_ExtendFramePointer) {
        ImDrawList_AddLine(draw_list,
            ImVec2(ctx.FramePointerXPos, ctx.FramePointerYPos),
            ImVec2(ctx.FramePointerXPos, ctx.CurrentTrackYPos),
            igGetColorU32_Col(ImGuiCol_Border, 1.0f),
            1.0f
        );
    }

    if (ctx.Flags & ImGuiTimelineFlags_ShowSelectedKeyframeMarkers) {
        const float frame_x_pos = ctx.TimelineXPos + (ctx.SelectedKeyframe.Frame - (float)ctx.FirstVisibleFrame) * ctx.FrameWidth;
        ImDrawList_AddLine(draw_list,
            ImVec2(frame_x_pos, ctx.FramePointerYPos),
            ImVec2(frame_x_pos, ctx.CurrentTrackYPos),
            igGetColorU32_Col(ImGuiCol_Border, 1.0f),
            1.0f
        );
    }
    if (ctx.Flags & ImGuiTimelineFlags_ExtendFrameMarkers) {
        const int first_frame = ctx.FirstVisibleFrame + (10 - ctx.FirstVisibleFrame % 10);
        for (int frame = first_frame; frame <= ctx.LastVisibleFrame; frame += 10) {
            const float frame_x_pos = ctx.TimelineXPos + (float)(frame - ctx.FirstVisibleFrame) * ctx.FrameWidth;
            ImDrawList_AddLine(draw_list,
                ImVec2(frame_x_pos, ctx.FramePointerYPos),
                ImVec2(frame_x_pos, ctx.CurrentTrackYPos),
                igGetColorU32_Col(ImGuiCol_Border, 1.0f),
                1.0f
            );
        }
    }

    igPopID();
    igEndChild();

    g_InTimeline = false;
}

bool ImGui::BeginTimelineGroup(std::string_view label, bool* open) {
    return TimelineTrack(label, (float*)open, 0, ImGuiTimelineTrackFlags_Group);
}

void ImGui::EndTimelineGroup() {
    auto& ctx = g_TimelineContexts[g_CurrentTimelineId];
    ctx.IndentLevel--;
}

bool ImGui::TimelineTrack(std::string_view label, float* keyframes, int keyframe_count, ImGuiTimelineTrackFlags flags, int* out_selected_keyframe) {
    const bool is_group = flags & ImGuiTimelineTrackFlags_Group;

    IM_ASSERT(is_group || keyframes != nullptr);
    IM_ASSERT(is_group || keyframe_count > 0);

    bool modified = false;
    const ImGuiID track_id = igGetID_Str(label.data());
    const auto window = igGetCurrentWindow();
    const auto draw_list = window->DrawList;
    auto& ctx = g_TimelineContexts[g_CurrentTimelineId];

    const ImVec2 cursor_pos = window->DC.CursorPos;
    const ImVec2 avail = GetContentRegionAvail();

    const float track_y_pos = ctx.CurrentTrackYPos;
    const float track_height = ImMax(TIMELINE_TRACK_BASE_HEIGHT, CalcTextSize("Text").y + igGetStyle()->FramePadding.y * 2.0f);
    const float label_width = avail.x * TIMELINE_TRACK_LABEL_TO_TRACK_WIDTH_RATIO;
    ctx.CurrentTrackYPos += track_height;
    ctx.TrackCount++;

    // Essentially this: PushID(label.data()), but since we already have the ID,
    // we can just push it directly.
    igPushOverrideID(track_id);

    // The entire track (label + track)
    const ImRect full_track_rect = {
        ImVec2(cursor_pos.x, track_y_pos),
        ImVec2(cursor_pos.x + avail.x, track_y_pos + track_height)
    };

    const auto label_bg_color = ctx.TrackCount % 2 == 0
        ? ImGuiCol_FrameBgActive
        : ImGuiCol_FrameBgHovered;

    const ImRect label_rect = {
        ImVec2(cursor_pos.x, track_y_pos),
        ImVec2(cursor_pos.x + label_width, track_y_pos + track_height)
    };

    bool group_toggled = false;
    bool group_open = false;
    if (is_group) {
        const ImGuiID group_id = igGetID_Str("##group_toggle");
        if (!igItemAdd(label_rect, group_id, nullptr, 0)) {
            igPopID();
            return false;
        }

        group_toggled = igButtonBehavior(label_rect, group_id, nullptr, nullptr,
            ImGuiButtonFlags_PressedOnClickRelease);

        // Group Expand/Collapse behavior
        // If no open bool was passed, the group is always open
        auto p_open = (bool*)keyframes;
        if (group_toggled && p_open) {
            *p_open = !*p_open;
        }
        
        group_open = !p_open || *p_open;
    }

    igItemSize_Rect(full_track_rect, 0.0f);

    // Draw track label section
    ImDrawList_AddRectFilled(draw_list, label_rect.Min, label_rect.Max, igGetColorU32_Col(label_bg_color, 1.0f), 0.0f, 0);

    // The extra 3.0f is the padding between the arrow and the label
    const float regular_label_indent = TIMELINE_TRACK_LABEL_INDENT * (float)ctx.IndentLevel + 3.0f;
    const float label_indent = is_group
        ? regular_label_indent + TIMELINE_TRACK_GROUP_ARROW_SIZE
        : regular_label_indent;

    const float max_label_width = label_width - label_indent;
    const auto [label_size, text_end] = TrimTextAndGetSize(label, max_label_width);
    const ImVec2 label_pos = {
        cursor_pos.x + label_indent,
        track_y_pos + track_height * 0.5f - label_size.y * 0.5f
    };

    // Draw group arrow
    if (is_group) {
        if (group_open) {
            const float arrow_x_pos = cursor_pos.x + regular_label_indent;
            const float arrow_y_pos = track_y_pos + track_height * 0.5f;
            constexpr float arrow_size = TIMELINE_TRACK_GROUP_ARROW_SIZE * 0.5f;

            ImDrawList_AddTriangleFilled(draw_list,
                ImVec2(arrow_x_pos - arrow_size, arrow_y_pos - arrow_size),
                ImVec2(arrow_x_pos + arrow_size, arrow_y_pos - arrow_size),
                ImVec2(arrow_x_pos, arrow_y_pos + arrow_size),
                igGetColorU32_Col(ImGuiCol_Text, 1.0f)
            );
        } else {
            const float arrow_x_pos = cursor_pos.x + regular_label_indent;
            const float arrow_y_pos = track_y_pos + track_height * 0.5f;
            constexpr float arrow_size = TIMELINE_TRACK_GROUP_ARROW_SIZE * 0.5f;

            ImDrawList_AddTriangleFilled(draw_list,
                ImVec2(arrow_x_pos - arrow_size, arrow_y_pos - arrow_size),
                ImVec2(arrow_x_pos + arrow_size, arrow_y_pos),
                ImVec2(arrow_x_pos - arrow_size, arrow_y_pos + arrow_size),
                igGetColorU32_Col(ImGuiCol_Text, 1.0f)
            );
        }
    }

    if (text_end) {
        const std::string corrected_label = std::string(label.data(), text_end) + "...";
        ImDrawList_AddText_Vec2(draw_list, label_pos, igGetColorU32_Col(ImGuiCol_Text, 1.0f), corrected_label.c_str(), nullptr);
    } else {
        ImDrawList_AddText_Vec2(draw_list, label_pos, igGetColorU32_Col(ImGuiCol_Text, 1.0f), label.data(), label.data() + label.size());
    }

    // Draw track section
    const float track_x_pos = cursor_pos.x + label_width;
    const float track_draw_width = avail.x - label_width;
    const float track_interactable_width = track_draw_width - TIMELINE_TRACK_RIGHT_EXTRA_PADDING;
    const float track_x_end = track_x_pos + track_interactable_width;

    const ImRect track_rect = {
        ImVec2(track_x_pos, track_y_pos),
        ImVec2(track_x_pos + track_draw_width, track_y_pos + track_height)
    };

    igItemSize_Rect(track_rect, 0.0f);

    ImDrawList_AddRectFilled(draw_list, track_rect.Min, track_rect.Max, igGetColorU32_Col(is_group ? label_bg_color : ImGuiCol_FrameBg, 1.0f), 0.0f, 0);
    if (ctx.TrackCount > 1) {
        ImDrawList_AddLine(draw_list,
            ImVec2(track_x_pos, track_y_pos),
            ImVec2(track_x_end, track_y_pos),
            igGetColorU32_Col(ImGuiCol_Border, 1.0f),
            1.0f
        );
    }

    if (is_group) {
        ctx.CurrentTrackIndex = 0;
        ctx.IndentLevel += group_open;
        igPopID(); // track
        return group_open;
    }

    // Draw keyframes
    const float keyframe_radius = (track_height - 6.0f) * 0.5f;
    const bool mouse_released = igIsMouseReleased_Nil(0);
    const ImVec2 mouse_pos = GetMousePos();

    float distance_to_edge = ImMin(
        ImAbs(mouse_pos.x - track_x_pos),
        ImAbs(mouse_pos.x - track_x_end)
    );
    if (mouse_pos.x <= track_x_pos || mouse_pos.x >= track_x_end) {
        distance_to_edge = 0.0f;
    }
    constexpr float scroll_threshold = 0.1f;
    const float scroll_amount = (1.0f - (distance_to_edge / scroll_threshold) / track_interactable_width) * 0.02f;

    if (out_selected_keyframe) *out_selected_keyframe = -1;

    for (int i = 0; i < keyframe_count; ++i) {
        float keyframe = keyframes[i];

        if (keyframe < (float)ctx.FirstVisibleFrame || keyframe > (float)ctx.LastVisibleFrame) {
            continue;
        }

        const float keyframe_x_pos = track_x_pos + (keyframe - (float)ctx.FirstVisibleFrame) * ctx.FrameWidth;
        const float keyframe_y_pos = track_y_pos + track_height * 0.5f;

        const ImRect keyframe_rect = {
            ImVec2(keyframe_x_pos - keyframe_radius, keyframe_y_pos - keyframe_radius),
            ImVec2(keyframe_x_pos + keyframe_radius, keyframe_y_pos + keyframe_radius)
        };

        const ImGuiID keyframe_id = igGetID_Ptr((void*)i);
        if (!igItemAdd(keyframe_rect, keyframe_id, nullptr, 0)) {
            // TODO: Could technically change this to a break, as this means that the rest of the keyframes are not visible
            continue;
        }

        const bool keyframe_hovered = igIsItemHovered(0);
        const bool keyframe_active = igIsItemActive();
        const bool keyframe_clicked = keyframe_hovered && igIsMouseClicked_Bool(0, false);

        const auto keyframe_color = GET_ACTIVE_HOVERED_COLOR(keyframe_active, keyframe_hovered, ImGuiCol_Button);
        
        ImDrawList_AddCircleFilled(draw_list,
            ImVec2(keyframe_x_pos, keyframe_y_pos),
            keyframe_radius,
            igGetColorU32_Col(keyframe_color, 1.0f),
            4 // Diamond shape
        );

        if (ctx.SelectedKeyframe.TrackId == track_id && ctx.SelectedKeyframe.Index == i) {
            if (out_selected_keyframe) *out_selected_keyframe = i;
            ctx.SelectedKeyframe.Frame = keyframe;
            ImDrawList_AddCircle(draw_list,
                ImVec2(keyframe_x_pos, keyframe_y_pos),
                keyframe_radius,
                igGetColorU32_Col(ImGuiCol_NavHighlight, 1.0f),
                4, // Diamond shape
                2.0f
            );
        }

        if (keyframe_active) {
            igBeginTooltip();
            igText("Frame: %.2f", (double)keyframe);
            igEndTooltip();
        }

        // Behavior
        if (keyframe_clicked) {
            igSetActiveID(keyframe_id, window);
            igFocusWindow(window, 0);
            ctx.SelectedKeyframe = { ctx.CurrentTrackIndex, keyframe, track_id, i };
            ctx.MouseClickStartPos = mouse_pos;
            if (out_selected_keyframe) *out_selected_keyframe = i;
        }

        if (keyframe_active) {
            if (mouse_released) {
                igClearActiveID();
            }

            if (ctx.MouseDragDelta.x != 0.0f || ctx.MouseDragDelta.y != 0.0f) {
                const float mouse_frame = (float)ctx.FirstVisibleFrame + (mouse_pos.x - track_x_pos) / ctx.FrameWidth;
                const float mouse_frame_clamped = ImClamp(mouse_frame, (float)ctx.FirstVisibleFrame, (float)ctx.LastVisibleFrame);
                keyframe = ctx.Flags & ImGuiTimelineFlags_EnableKeyframeSnapping
                    ? std::roundf(mouse_frame_clamped)
                    : mouse_frame_clamped;
                modified = true;

                // Adjust scroll if we're dragging a keyframe close to the edge
                if (mouse_pos.x < track_x_pos + track_interactable_width * scroll_threshold) {
                    ctx.Scroll -= scroll_amount;
                    ctx.Scroll = ImClamp(ctx.Scroll, 0.0f, 1.0f);
                } else if (mouse_pos.x > track_x_pos + track_interactable_width * (1.0f - scroll_threshold)) {
                    ctx.Scroll += scroll_amount;
                    ctx.Scroll = ImClamp(ctx.Scroll, 0.0f, 1.0f);
                }
            }
        }

        keyframes[i] = keyframe;
    }

    igPopID();

    ctx.CurrentTrackIndex++;
    return modified;
}

bool ImGui::TimelineTrack(std::string_view label, std::vector<float>& keyframes, ImGuiTimelineTrackFlags flags, int* out_selected_keyframe) {
    return TimelineTrack(label, keyframes.data(), (int)keyframes.size(), flags);
}

bool ImGui::TimelineIsAnyKeyframeSelected() {
    return g_TimelineContexts[g_CurrentTimelineId].SelectedKeyframe.TrackId != 0;
}

std::pair<int, int> ImGui::TimelineGetSelectedKeyframe() {
    const auto& kf = g_TimelineContexts[g_CurrentTimelineId].SelectedKeyframe;
    return { kf.TrackIndex, kf.Index };
}
