using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SharpPluginLoader.Bootstrapper.Chunk
{
    public static class ChunkManager
    {
#if DEBUG
        private static string DefaultChunkPath => "nativePC/plugins/CSharp/Loader/Default.Debug.bin";
#else
        private static string DefaultChunkPath => "nativePC/plugins/CSharp/Loader/Default.bin";
#endif

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
