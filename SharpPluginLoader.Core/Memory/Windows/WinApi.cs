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

        public static bool IsManagedAssembly(string path)
        {
            using var stream = File.OpenRead(path);
            using var reader = new PEReader(stream);
            
            if (!reader.HasMetadata)
                return false;

            var metadataReader = reader.GetMetadataReader();
            return metadataReader.IsAssembly;
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
