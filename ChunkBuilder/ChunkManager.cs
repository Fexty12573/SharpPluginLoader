namespace ChunkBuilder
{
    internal static class ChunkManager
    {
        private static string DefaultChunkPath => "nativePC/plugins/CSharp/Loader/Data.bin";

        private static readonly Chunk DefaultChunk;
        private static readonly Dictionary<string, Chunk> Chunks = new();

        static ChunkManager()
        {
            DefaultChunk = new Chunk(DefaultChunkPath);
        }

        public static Chunk GetDefaultChunk() => DefaultChunk;

        public static Chunk? RequestChunk(string chunkName)
        {
            if (chunkName == "Default")
                return DefaultChunk;

            return Chunks.ContainsKey(chunkName) ? Chunks[chunkName] : null;
        }

        private static void LoadChunk(string filePath)
        {
            var chunkName = Path.GetFileNameWithoutExtension(filePath);
            if (Chunks.ContainsKey(chunkName))
                return;

            Chunks.Add(chunkName, new Chunk(filePath));
        }
    }
}
