#include "ImGuiModule.h"
#include "SharpPluginLoader.h"

#include <imgui_impl.h>
#include "Timeline.h"

#include "CoreClr.h"

void ImGuiModule::initialize(CoreClr* coreclr) {
    coreclr->add_internal_call("BeginTimeline", &ImGuiModule::begin_timeline);
    coreclr->add_internal_call("EndTimeline", &ImGuiModule::end_timeline);
    coreclr->add_internal_call("BeginTimelineGroup", &ImGuiModule::begin_timeline_group);
    coreclr->add_internal_call("EndTimelineGroup", &ImGuiModule::end_timeline_group);
    coreclr->add_internal_call("TimelineTrack", &ImGuiModule::timeline_track);
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
