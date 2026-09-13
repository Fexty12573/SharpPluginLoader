using System.Runtime.InteropServices;

namespace SharpPluginLoader.Core.SaveData;

[StructLayout(LayoutKind.Sequential, Size = 0x10)]
public struct Item
{
    public nint Vtable;
    public int Id;
    public int Count;
}
