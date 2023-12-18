using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SharpPluginLoader.Core;

/// <summary>
/// Represents a string in memory.
/// </summary>
[StructLayout(LayoutKind.Explicit)]
public unsafe struct MtString
{
    [FieldOffset(0x00)] public int RefCount;
    [FieldOffset(0x04)] public int Length;
    [FieldOffset(0x08)] public fixed sbyte Data[8];

    /// <summary>
    /// Gets the string from the data using the specified encoding.
    /// </summary>
    /// <param name="encoding">The encoding to interpret the string as</param>
    /// <returns>The string, interpreted with the given encoding</returns>
    public string GetString(Encoding encoding)
    {
        fixed (sbyte* ptr = Data)
        {
            return new string(ptr, 0, Length, encoding);
        }
    }

    /// <summary>
    /// Gets the string from the data using UTF-8 encoding.
    /// </summary>
    /// <returns>The string, interpreted with UTF-8 encoding</returns>
    public string GetString()
    {
        return GetString(Encoding.UTF8);
    }
}
