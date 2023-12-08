
using SharpPluginLoader.Core.Resources;
using SharpPluginLoader.Core.Resources.Animation;

namespace SharpPluginLoader.Core
{
    internal static unsafe class InternalCalls
    {
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
        public static delegate* unmanaged<void> TestInternalCallPtr;
        public static delegate* unmanaged<nint, void> QueueYesNoDialogPtr;

        public static delegate* unmanaged<string, void> LoadChunkPtr;
        public static delegate* unmanaged<string, nint> RequestChunkPtr;
        public static delegate* unmanaged<nint> GetDefaultChunkPtr;
        public static delegate* unmanaged<nint, string, nint> ChunkGetFilePtr;
        public static delegate* unmanaged<nint, string, nint> ChunkGetFolderPtr;
        public static delegate* unmanaged<nint, nint> FileGetContentsPtr;
        public static delegate* unmanaged<nint, long> FileGetSizePtr;

        public static delegate* unmanaged<string, float, float, ref float, int, bool> BeginTimelinePtr;
        public static delegate* unmanaged<void> EndTimelinePtr;
        public static delegate* unmanaged<string, ref bool, bool> BeginTimelineGroupPtr;
        public static delegate* unmanaged<void> EndTimelineGroupPtr;
        public static delegate* unmanaged<string, float*, int, out int, bool> TimelineTrackPtr;

        public static delegate* unmanaged<string, int, void> NotificationSuccessPtr;
        public static delegate* unmanaged<string, int, void> NotificationErrorPtr;
        public static delegate* unmanaged<string, int, void> NotificationWarningPtr;
        public static delegate* unmanaged<string, int, void> NotificationInfoPtr;
        public static delegate* unmanaged<int, int, string, string, void> NotificationPtr;
        public static delegate* unmanaged<void> RenderNotificationsPtr;
#pragma warning restore CS0649

        public static void TestInternalCall() => TestInternalCallPtr();
        public static void QueueYesNoDialog(nint messagePtr) => QueueYesNoDialogPtr(messagePtr);

        public static void LoadChunk(string name) => LoadChunkPtr(name);
        public static nint GetDefaultChunk() => GetDefaultChunkPtr();
        public static nint RequestChunk(string name) => RequestChunkPtr(name);
        public static nint ChunkGetFile(nint chunk, string name) => ChunkGetFilePtr(chunk, name);
        public static nint ChunkGetFolder(nint chunk, string name) => ChunkGetFolderPtr(chunk, name);
        public static nint FileGetContents(nint file) => FileGetContentsPtr(file);
        public static long FileGetSize(nint file) => FileGetSizePtr(file);

        public static bool BeginTimeline(string label, float startFrame, float endFrame, ref float currentFrame, int flags)
        {
            return BeginTimelinePtr(label, startFrame, endFrame, ref currentFrame, flags);
        }

        public static void EndTimeline() => EndTimelinePtr();

        public static bool BeginTimelineGroup(string label, ref bool expanded)
        {
            return BeginTimelineGroupPtr(label, ref expanded);
        }

        public static void EndTimelineGroup() => EndTimelineGroupPtr();

        public static bool TimelineTrack(string label, float* keyFrames, int keyframeCount, out int selectedKeyframe)
        {
            return TimelineTrackPtr(label, keyFrames, keyframeCount, out selectedKeyframe);
        }

        public static bool TimelineTrack(string label, float[] keyFrames, out int selectedKeyframe, int explicitCount = -1)
        {
            fixed (float* ptr = keyFrames)
            {
                return TimelineTrack(label, ptr, explicitCount == -1 ? keyFrames.Length : explicitCount, out selectedKeyframe);
            }
        }

        public static bool TimelineTrack(string label, Span<float> keyFrames, out int selectedKeyframe, int explicitCount = -1)
        {
            fixed (float* ptr = keyFrames)
            {
                return TimelineTrack(label, ptr, explicitCount == -1 ? keyFrames.Length : explicitCount, out selectedKeyframe);
            }
        }

        public static void NotificationSuccess(string message, int duration) => NotificationSuccessPtr(message, duration);

        public static void NotificationError(string message, int duration) => NotificationErrorPtr(message, duration);

        public static void NotificationWarning(string message, int duration) => NotificationWarningPtr(message, duration);

        public static void NotificationInfo(string message, int duration) => NotificationInfoPtr(message, duration);

        public static void Notification(int type, int duration, string title, string message) => NotificationPtr(type, duration, title, message);

        public static void RenderNotifications() => RenderNotificationsPtr();
    }
}
