using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpPluginLoader.Bootstrapper.Chunk
{
    public class FileSystemFolder : IFileSystemItem
    {
        public string Name { get; }

        public List<IFileSystemItem> Children { get; }

        public bool IsEmpty => Children.Count == 0;

        public IEnumerable<FileSystemFolder> Folders => Children.OfType<FileSystemFolder>();
        public IEnumerable<FileSystemFile> Files => Children.OfType<FileSystemFile>();

        public bool Contains(string name) => Children.Any(x => x.Name == name);
        public bool ContainsFile(string name) => Children.Any(x => x.Name == name && x is FileSystemFile);
        public bool ContainsFolder(string name) => Children.Any(x => x.Name == name && x is FileSystemFolder);

        public FileSystemFile? GetFile(string name)
        {
            return Children.Find(x => x.Name == name && x is FileSystemFile) as FileSystemFile;
        }

        public FileSystemFolder? GetFolder(string name)
        {
            return Children.Find(x => x.Name == name && x is FileSystemFolder) as FileSystemFolder;
        }

        public void Add(IFileSystemItem item)
        {
            Debug.Assert(!Contains(item.Name));
            Children.Add(item);
        }

        public FileSystemFolder(string name, IEnumerable<IFileSystemItem>? children = null)
        {
            Name = name;
            Children = children?.ToList() ?? new List<IFileSystemItem>();
        }
    }
}
