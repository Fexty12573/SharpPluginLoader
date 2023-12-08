#include "ImGuiModule.h"
#include "SharpPluginLoader.h"

#include <imgui_impl.h>
#include <imgui_notify.h>
#include "Timeline.h"

#include "CoreClr.h"

void ImGuiModule::initialize(CoreClr* coreclr) {
    coreclr->add_internal_call("BeginTimeline", &ImGuiModule::begin_timeline);
    coreclr->add_internal_call("EndTimeline", &ImGuiModule::end_timeline);
    coreclr->add_internal_call("BeginTimelineGroup", &ImGuiModule::begin_timeline_group);
    coreclr->add_internal_call("EndTimelineGroup", &ImGuiModule::end_timeline_group);
    coreclr->add_internal_call("TimelineTrack", &ImGuiModule::timeline_track);

    coreclr->add_internal_call("NotificationSuccess", &ImGuiModule::notification_success);
    coreclr->add_internal_call("NotificationError", &ImGuiModule::notification_error);
    coreclr->add_internal_call("NotificationWarning", &ImGuiModule::notification_warning);
    coreclr->add_internal_call("NotificationInfo", &ImGuiModule::notification_info);
    coreclr->add_internal_call("Notification", &ImGuiModule::notification);
    coreclr->add_internal_call("RenderNotifications", &ImGui::RenderNotifications);
}

void ImGuiModule::shutdown() {
}

bool ImGuiModule::begin_timeline(const char* label, float start_frame, float end_frame, float* p_current_frame, ImGuiTimelineFlags flags) {
    return ImGui::BeginTimeline(label, start_frame, end_frame, p_current_frame, flags);
}

void ImGuiModule::end_timeline() {
    ImGui::EndTimeline();
}

bool ImGuiModule::begin_timeline_group(const char* label, bool* open) {
    return ImGui::BeginTimelineGroup(label, open);
}

void ImGuiModule::end_timeline_group() {
    ImGui::EndTimelineGroup();
}

bool ImGuiModule::timeline_track(const char* label, float* keyframes, int keyframe_count, int* out_selected_keyframe) {
    return ImGui::TimelineTrack(label, keyframes, keyframe_count, 0, out_selected_keyframe);
}

void ImGuiModule::notification_success(const char* text, int display_time) {
    ImGui::InsertNotification({ ImGuiToastType_Success, display_time, text });
}

void ImGuiModule::notification_error(const char* text, int display_time) {
    ImGui::InsertNotification({ ImGuiToastType_Error, display_time, text });
}

void ImGuiModule::notification_warning(const char* text, int display_time) {
    ImGui::InsertNotification({ ImGuiToastType_Warning, display_time, text });
}

void ImGuiModule::notification_info(const char* text, int display_time) {
    ImGui::InsertNotification({ ImGuiToastType_Info, display_time, text });
}

void ImGuiModule::notification(int type, int duration, const char* title, const char* text) {
    ImGuiToast toast(type, duration);
    toast.set_title(title);
    toast.set_content(text);
    ImGui::InsertNotification(toast);
}
