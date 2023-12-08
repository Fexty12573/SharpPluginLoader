using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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

        public static bool TimelineTrack(string label, Span<float> keyframes, out int selectedKeyframe, int explicitCount = -1)
        {
            return InternalCalls.TimelineTrack(label, keyframes, out selectedKeyframe, explicitCount);
        }

        public static bool TimelineTrack(string label, Span<float> keyframes, int explicitCount = -1)
        {
            return InternalCalls.TimelineTrack(label, keyframes, out _, explicitCount);
        }

        public static void NotificationSuccess(string message, int duration = 3000)
        {
            InternalCalls.NotificationSuccess(message, duration);
        }

        public static void NotificationError(string message, int duration = 3000)
        {
            InternalCalls.NotificationError(message, duration);
        }

        public static void NotificationWarning(string message, int duration = 3000)
        {
            InternalCalls.NotificationWarning(message, duration);
        }

        public static void NotificationInfo(string message, int duration = 3000)
        {
            InternalCalls.NotificationInfo(message, duration);
        }

        public static void Notification(ImGuiToastType type, string title, string message, int duration = 3000)
        {
            InternalCalls.Notification((int)type, duration, title, message);
        }

        public static unsafe bool InputScalar(string label, ref sbyte value, sbyte step = 1, sbyte stepFast = 10,
            string format = "%d", ImGuiInputTextFlags flags = 0)
        {
            return ImGui.InputScalar(
                label,
                ImGuiDataType.S8,
                (nint)Unsafe.AsPointer(ref value),
                (nint)Unsafe.AsPointer(ref step),
                (nint)Unsafe.AsPointer(ref stepFast),
                format,
                flags
            );
        }

        public static unsafe bool InputScalar(string label, ref byte value, byte step = 1, byte stepFast = 10,
            string format = "%u", ImGuiInputTextFlags flags = 0)
        {
            return ImGui.InputScalar(
                label,
                ImGuiDataType.U8,
                (nint)Unsafe.AsPointer(ref value),
                (nint)Unsafe.AsPointer(ref step),
                (nint)Unsafe.AsPointer(ref stepFast),
                format,
                flags
            );
        }

        public static unsafe bool InputScalar(string label, ref short value, short step = 1, short stepFast = 10,
            string format = "%d", ImGuiInputTextFlags flags = 0)
        {
            return ImGui.InputScalar(
                label,
                ImGuiDataType.S16,
                (nint)Unsafe.AsPointer(ref value),
                (nint)Unsafe.AsPointer(ref step),
                (nint)Unsafe.AsPointer(ref stepFast),
                format,
                flags
            );
        }

        public static unsafe bool InputScalar(string label, ref ushort value, ushort step = 1, ushort stepFast = 10,
            string format = "%u", ImGuiInputTextFlags flags = 0)
        {
            return ImGui.InputScalar(
                label,
                ImGuiDataType.U16,
                (nint)Unsafe.AsPointer(ref value),
                (nint)Unsafe.AsPointer(ref step),
                (nint)Unsafe.AsPointer(ref stepFast),
                format,
                flags
            );
        }

        public static unsafe bool InputScalar(string label, ref int value, int step = 1, int stepFast = 10,
            string format = "%d", ImGuiInputTextFlags flags = 0)
        {
            return ImGui.InputScalar(
                label,
                ImGuiDataType.S32,
                (nint)Unsafe.AsPointer(ref value),
                (nint)Unsafe.AsPointer(ref step),
                (nint)Unsafe.AsPointer(ref stepFast),
                format,
                flags
            );
        }

        public static unsafe bool InputScalar(string label, ref uint value, uint step = 1, uint stepFast = 10,
            string format = "%u", ImGuiInputTextFlags flags = 0)
        {
            return ImGui.InputScalar(
                label,
                ImGuiDataType.U32,
                (nint)Unsafe.AsPointer(ref value),
                (nint)Unsafe.AsPointer(ref step),
                (nint)Unsafe.AsPointer(ref stepFast),
                format,
                flags
            );
        }

        public static unsafe bool InputScalar(string label, ref long value, long step = 1, long stepFast = 10,
            string format = "%d", ImGuiInputTextFlags flags = 0)
        {
            return ImGui.InputScalar(
                label,
                ImGuiDataType.S64,
                (nint)Unsafe.AsPointer(ref value),
                (nint)Unsafe.AsPointer(ref step),
                (nint)Unsafe.AsPointer(ref stepFast),
                format,
                flags
            );
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

        public enum ImGuiToastType
        {
            Success,
            Warning,
            Error,
            Info
        }
    }
