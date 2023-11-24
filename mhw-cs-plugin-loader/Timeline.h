#pragma once

#include <string_view>
#include <vector>
#include <utility>

typedef unsigned ImGuiTimelineFlags;
typedef unsigned ImGuiTimelineTrackFlags;

enum ImGuiTimelineFlags_ {
    ImGuiTimelineFlags_None = 0,
    ImGuiTimelineFlags_EnableFramePointerSnapping = 1 << 0, // Snaps the frame pointer to the closest frame
    ImGuiTimelineFlags_EnableKeyframeSnapping = 1 << 1, // Snaps keyframes to the closest frame
    ImGuiTimelineFlags_ExtendFramePointer = 1 << 2, // Extends frame pointers to the bottom of the timeline
    ImGuiTimelineFlags_ExtendFrameMarkers = 1 << 3, // Extends each 10th frame marker to the bottom of the timeline
    ImGuiTimelineFlags_ShowSelectedKeyframeMarkers = 1 << 4, // Shows a marker on the selected keyframe

    // Amalgamated flags

    ImGuiTimelineFlags_EnableSnapping = ImGuiTimelineFlags_EnableFramePointerSnapping | ImGuiTimelineFlags_EnableKeyframeSnapping,
    ImGuiTimelineFlags_ExtendMarkers =  ImGuiTimelineFlags_ExtendFrameMarkers | ImGuiTimelineFlags_ExtendFramePointer | ImGuiTimelineFlags_ShowSelectedKeyframeMarkers
};

enum ImGuiTimelineTrackFlags_ {
    ImGuiTimelineTrackFlags_None = 0,
    ImGuiTimelineTrackFlags_Group = 1 << 0,
};

namespace ImGui {

bool BeginTimeline(std::string_view label, float start_frame, float end_frame, float* p_current_frame, ImGuiTimelineFlags flags = 0);
void EndTimeline();

bool BeginTimelineGroup(std::string_view label, bool* open = nullptr);
void EndTimelineGroup();

bool TimelineTrack(std::string_view label, float* keyframes, int keyframe_count = 1, ImGuiTimelineTrackFlags flags = 0, int* out_selected_keyframe = nullptr);
bool TimelineTrack(std::string_view label, std::vector<float>& keyframes, ImGuiTimelineTrackFlags flags = 0, int* out_selected_keyframe = nullptr);

// Refers to the last timeline created with BeginTimeline
bool TimelineIsAnyKeyframeSelected();
std::pair<int, int> TimelineGetSelectedKeyframe();


}
