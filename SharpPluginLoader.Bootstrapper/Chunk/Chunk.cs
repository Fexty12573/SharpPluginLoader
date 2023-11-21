using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SharpPluginLoader.Bootstrapper.Chunk
{
    public class Chunk
    {
        private static string Magic => "bin\x00";
        private static uint Version => 0x20230611;

        private readonly FileSystemFolder _root;

        public Chunk(string fileName)
        {
            var reader = new BinaryReader(new FileStream(fileName, new FileStreamOptions
            {
                Mode = FileMode.Open,
                Access = FileAccess.Read,
                Share = FileShare.Read
            }));

            // Header
            var magic = Encoding.UTF8.GetString(reader.ReadBytes(4));
            var version = reader.ReadUInt32();
            var rootOffset = reader.ReadInt64();

            if (magic != Magic)
                throw new Exception($"Invalid magic: {magic}");

            if (version != Version)
                throw new Exception($"Invalid version: {version}, should be {Version}");

            reader.BaseStream.Position = rootOffset;

            _root = ReadFolder(reader);
        }

        public Chunk(FileSystemFolder root)
        {
            _root = root;
        }

        public FileSystemFile GetFile(string path)
        {
            var folderPath = path[..path.LastIndexOf('/')];
            var fileName = path[(path.LastIndexOf('/') + 1)..];
            var folder = GetFolder(folderPath);

            return folder.GetFile(fileName) ?? throw new FileNotFoundException($"File not found: {fileName}");
        }

        public FileSystemFolder GetFolder(string path)
        {
            var parts = path.TrimStart('/').Split('/');
            var current = _root;

            foreach (var part in parts)
            {
                current = current.GetFolder(part);
                if (current == null)
                    throw new DirectoryNotFoundException($"Folder not found: {part}");
            }

            return current;
        }

        public void WriteToFile(string fileName)
        {
            var writer = new BinaryWriter(new FileStream(fileName, new FileStreamOptions
            {
                Mode = FileMode.Create,
                Access = FileAccess.Write,
                Share = FileShare.None
            }));

            // Header
            writer.Write(Encoding.UTF8.GetBytes(Magic));
            writer.Write(Version);
            writer.Write(writer.BaseStream.Position + 8);

            WriteFolder(writer, _root);
        }

        private static IFileSystemItem ReadItem(BinaryReader reader)
        {
            var type = (ChunkItemType)reader.ReadByte();
            return type switch
            {
                ChunkItemType.File => ReadFile(reader),
                ChunkItemType.Folder => ReadFolder(reader),
                _ => throw new Exception($"Invalid item type: {type}")
            };
        }

        private static FileSystemFile ReadFile(BinaryReader reader)
        {
            var contentsLength = reader.ReadInt32();
            var nameLength = reader.ReadInt16();
            var name = Encoding.UTF8.GetString(reader.ReadBytes(nameLength));

            return new FileSystemFile(name, reader.ReadBytes(contentsLength));
        }

        private static FileSystemFolder ReadFolder(BinaryReader reader)
        {
            var childrenCount = reader.ReadInt16();
            var nameLength = reader.ReadInt16();
            var name = Encoding.UTF8.GetString(reader.ReadBytes(nameLength));

            var folder = new FileSystemFolder(name);

            for (var i = 0; i < childrenCount; i++)
                folder.Add(ReadItem(reader));

            return folder;
        }

        private static void WriteItem(BinaryWriter writer, IFileSystemItem item)
        {
            switch (item)
            {
                case FileSystemFile file:
                    WriteFile(writer, file);
                    break;
                case FileSystemFolder folder:
                    WriteFolder(writer, folder);
                    break;
                default:
                    throw new Exception($"Invalid item type: {item.GetType()}");
            }
        }

        private static void WriteFile(BinaryWriter writer, FileSystemFile file)
        {
            writer.Write((byte)ChunkItemType.File);
            writer.Write(file.Contents.Length);
            writer.Write((short)file.Name.Length);
            writer.Write(Encoding.UTF8.GetBytes(file.Name));
            writer.Write(file.Contents);
        }

        private static void WriteFolder(BinaryWriter writer, FileSystemFolder folder)
        {
            writer.Write((byte)ChunkItemType.Folder);
            writer.Write((short)folder.Children.Count);
            writer.Write((short)folder.Name.Length);
            writer.Write(Encoding.UTF8.GetBytes(folder.Name));

            foreach (var child in folder.Children)
                WriteItem(writer, child);
        }
    }

    internal enum ChunkItemType : byte
    {
        File,
        Folder
    }
}
