namespace ChunkBuilder
{
    internal class FileSystemFile : IFileSystemItem
    {
        public string Name { get; }
        public byte[] Contents { get; }

        public string Extension => Name.Split('.').Last();

        public bool IsEmpty => Contents.Length == 0;

        public FileSystemFile(string name, byte[] contents)
        {
            Name = name;
            Contents = contents;
        }
    }
}
