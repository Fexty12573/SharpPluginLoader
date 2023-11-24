using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;

namespace SharpPluginLoader.Core.Rendering
{
    public static class ImGuiExtensions
    {
        public static bool BeginTimeline(string label, float startFrame, float endFrame, ref float currentFrame, ImGuiTimelineFlags flags = 0)
        {
            return InternalCalls.BeginTimeline(label, startFrame, endFrame, ref currentFrame, (int)flags);
        }

        public static void EndTimeline() => InternalCalls.EndTimeline();

        public static bool BeginTimelineGroup(string label, ref bool expanded)
        {
            return InternalCalls.BeginTimelineGroup(label, ref expanded);
        }

        public static bool BeginTimelineGroup(string label)
        {
            var expanded = true;
            return InternalCalls.BeginTimelineGroup(label, ref expanded);
        }

        public static void EndTimelineGroup() => InternalCalls.EndTimelineGroup();

        public static bool TimelineTrack(string label, Span<float> keyframes, out int selectedKeyframe)
        {
            return InternalCalls.TimelineTrack(label, keyframes, out selectedKeyframe);
        }

        public static bool TimelineTrack(string label, Span<float> keyframes)
        {
            return InternalCalls.TimelineTrack(label, keyframes, out _);
        }
    }

    [Flags]
    public enum ImGuiTimelineFlags
    {
        None = 0,
        EnableFramePointerSnapping = 1 << 0,
        EnableKeyframeSnapping = 1 << 1,
        ExtendFramePointer = 1 << 2,
        ExtendFrameMarkers = 1 << 3,
        ShowSelectedKeyframeMarkers = 1 << 4,

        EnableSnapping = EnableFramePointerSnapping | EnableKeyframeSnapping,
        ExtendMarkers = ExtendFramePointer | ExtendFrameMarkers | ShowSelectedKeyframeMarkers
    }
}
