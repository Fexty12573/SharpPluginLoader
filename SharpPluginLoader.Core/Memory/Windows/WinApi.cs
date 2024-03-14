using System.Diagnostics;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;

namespace SharpPluginLoader.Core.Memory.Windows
{
    internal partial class WinApi
    {
        [LibraryImport("kernel32.dll")]
        public static partial nint GetCurrentProcess();

        [LibraryImport("kernel32.dll", EntryPoint = "GetModuleHandleW")]
        public static partial nint GetModuleHandle([MarshalAs(UnmanagedType.LPWStr)] string lpModuleName);

        [LibraryImport("kernel32.dll", EntryPoint = "K32GetModuleInformation")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool GetModuleInformation(nint hProcess, nint hModule, out ModuleInfo lpmodinfo, uint cb);

        [LibraryImport("kernel32.dll")]
        public static partial ulong VirtualQuery(nint lpAddress, out MemoryBasicInformation lpBuffer, ulong dwLength);

        [LibraryImport("kernel32.dll", EntryPoint = "LoadLibraryW")]
        public static partial nint LoadLibrary([MarshalAs(UnmanagedType.LPWStr)] string lpFileName);

        [LibraryImport("kernel32.dll", EntryPoint = "LoadLibraryExW")]
        public static partial nint LoadLibraryEx([MarshalAs(UnmanagedType.LPWStr)] string lpFileName, nint hFile, uint dwFlags);

        [LibraryImport("kernel32.dll", EntryPoint = "FreeLibrary")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool FreeLibrary(nint hModule);

        [LibraryImport("kernel32.dll", EntryPoint = "GetProcAddress")]
        public static partial nint GetProcAddress(nint hModule, [MarshalAs(UnmanagedType.LPStr)] string lpProcName);

        [LibraryImport("kernel32.dll", EntryPoint = "GetLastError")]
        public static partial uint GetLastError();

        public static bool IsManagedAssembly(string path)
        {
            // Repeatedly try to open the file for reading, with a timeout of 3 seconds.
            // This is necessary because the file may be locked due to a copy operation.
            using var stream = TryOpenFileStream(path, TimeSpan.FromSeconds(3));
            if (stream is null)
                return false;

            using var reader = new PEReader(stream);
            if (!reader.HasMetadata)
                return false;

            var metadataReader = reader.GetMetadataReader();
            return metadataReader.IsAssembly;
        }

        private static FileStream? TryOpenFileStream(string path, TimeSpan timeout)
        {
            var stopwatch = Stopwatch.StartNew();
            while (stopwatch.Elapsed < timeout)
            {
                try
                {
                    return File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                }
                catch (IOException)
                {
                    Thread.Sleep(100);
                }
            }

            return null;
        }
    }

    internal static class Constants
    {
        public const uint MemCommit = 0x1000;
        public const uint MemFree = 0x10000;
        public const uint MemReserve = 0x2000;
        public const uint PageNoAccess = 0x01;
        public const uint PageReadOnly = 0x02;
        public const uint PageReadWrite = 0x04;
        public const uint PageWriteCopy = 0x08;
        public const uint PageExecute = 0x10;
        public const uint PageExecuteRead = 0x20;
        public const uint PageExecuteReadWrite = 0x40;
        public const uint PageExecuteWriteCopy = 0x80;
        public const uint PageGuard = 0x100;
        public const uint PageNoCache = 0x200;
        public const uint PageWriteCombine = 0x400;

        public const uint LoadLibrarySearchDllLoadDir = 0x100;
        public const uint LoadLibrarySearchApplicationDir = 0x200;
        public const uint LoadLibrarySearchUserDirs = 0x400;
        public const uint LoadLibrarySearchSystem32 = 0x800;
        public const uint LoadLibrarySearchDefaultDirs = 0x1000;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct ModuleInfo
    {
        public nint BaseOfDll;
        public uint SizeOfImage;
        public nint EntryPoint;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MemoryBasicInformation
    {
        public nint BaseAddress;
        public nint AllocationBase;
        public uint AllocationProtect;
        public ushort PartitionId;
        public long RegionSize;
        public uint State;
        public uint Protect;
        public uint Type;
    }
}
