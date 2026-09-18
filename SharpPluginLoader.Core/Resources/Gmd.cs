using System.Runtime.InteropServices;

namespace SharpPluginLoader.Core.Resources;

/// <summary>
/// Represents a GMD (Game Message Data) resource, which contains localized message strings for the game.
/// </summary>
public class Gmd : Resource
{
    public Gmd(nint instance, bool weakRef = false) : base(instance, weakRef) { }
    public Gmd() { }

    /// <summary>
    /// The number of messages in the GMD resource.
    /// </summary>
    public ref int MessageCount => ref GetRef<int>(0xD8);

    /// <summary>
    /// The pointer to the string block of the GMD resource, which contains all the message strings.
    /// </summary>
    public unsafe byte* StringBlock => GetPtr<byte>(0xF0);

    /// <summary>
    /// The pointer to the array of message pointers in the GMD resource, where each pointer points to a message string in the string block.
    /// </summary>
    public unsafe byte** Messages
    {
        get => (byte**)GetPtr(0xF8);
        set => SetPtr(0xF8, (byte*)value);
    }

    /// <summary>
    /// Gets the message string at the specified index in the GMD resource.
    /// </summary>
    /// <param name="id">The id/index of the message</param>
    /// <returns>The string</returns>
    /// <remarks>This method assumes the message is UTF-8 encoded.</remarks>
    public unsafe string GetMessage(int id)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(id, MessageCount);
        ArgumentOutOfRangeException.ThrowIfNegative(id);

        return Marshal.PtrToStringUTF8((nint)Messages[id]) ?? string.Empty;
    }

    /// <inheritdoc cref="GetMessage"/>
    public unsafe byte* GetMessagePtr(int id)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(id, MessageCount);
        ArgumentOutOfRangeException.ThrowIfNegative(id);

        return Messages[id];
    }
}
