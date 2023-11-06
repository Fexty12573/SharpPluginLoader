using System;

namespace ChunkBuilder
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var root = new FileSystemFolder("/");
            var assemblies = new FileSystemFolder("Assemblies");

            assemblies.Add(CreateFile("./Data/Reloaded.Hooks.dll"));
            assemblies.Add(CreateFile("./Data/Reloaded.Hooks.Definitions.dll"));
            assemblies.Add(CreateFile("./Data/Reloaded.Assembler.dll"));
            assemblies.Add(CreateFile("./Data/Reloaded.Memory.dll"));
            assemblies.Add(CreateFile("./Data/Reloaded.Memory.Buffers.dll"));
            assemblies.Add(CreateFile("./Data/Iced.dll"));

            root.Add(assemblies);
            var chunk = new Chunk(root);

#if DEBUG
            chunk.WriteToFile("./Data/Default.Debug.bin");
#else
            chunk.WriteToFile("./Data/Default.bin");
#endif
        }

        internal static FileSystemFile CreateFile(string path)
        {
            return new FileSystemFile(Path.GetFileName(path), File.ReadAllBytes(path));
        }
    }
}
