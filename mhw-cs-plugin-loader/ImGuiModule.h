#pragma once
#include "NativeModule.h"
#include "Timeline.h"

struct MOTION_INFO;

class ImGuiModule final : public NativeModule {
public:
    void initialize(CoreClr* coreclr) override;
    void shutdown() override;

private:
    static bool begin_timeline(const char* label, float start_frame, float end_frame, float* p_current_frame, ImGuiTimelineFlags flags);
    static void end_timeline();

    static bool begin_timeline_group(const char* label, bool* open);
    static void end_timeline_group();

    static bool timeline_track(const char* label, float* keyframes, int keyframe_count, int* out_selected_keyframe);

    static void notification_success(const char* text, int display_time);
    static void notification_error(const char* text, int display_time);
    static void notification_warning(const char* text, int display_time);
    static void notification_info(const char* text, int display_time);

    static void notification(int type, int duration, const char* title, const char* text);
};

