
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
    }
}
