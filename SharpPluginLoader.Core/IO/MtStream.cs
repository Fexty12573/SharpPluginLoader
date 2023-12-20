
namespace SharpPluginLoader.Core.IO;

/// <summary>
/// Represents any kind of stream.
/// </summary>
public unsafe class MtStream : MtObject
{
    public MtStream(nint instance) : base(instance) { }
    public MtStream() { }

    /// <summary>
    /// The flags of the stream.
    /// </summary>
    public ref uint Flags => ref GetRef<uint>(0x8);

    /// <summary>
    /// Whether the stream is readable.
    /// </summary>
    public bool IsReadable => (Flags & 1) != 0;

    /// <summary>
    /// Whether the stream is writable.
    /// </summary>
    public bool IsWritable => (Flags & 2) != 0;

    /// <summary>
    /// Whether the stream is seekable.
    /// </summary>
    public bool IsSeekable => (Flags & 4) != 0;

    /// <summary>
    /// Gets the position of the stream.
    /// </summary>
    public long Position => new NativeFunction<nint, long>(GetVirtualFunction(10)).InvokeUnsafe(Instance);

    /// <summary>
    /// Gets or sets the length of the stream.
    /// </summary>
    public long Length
    {
        get => new NativeFunction<nint, long>(GetVirtualFunction(11)).InvokeUnsafe(Instance);
        set => new NativeFunction<nint, long, long>(GetVirtualFunction(10)).InvokeUnsafe(Instance, value);
    }

    /// <summary>
    /// Closes the stream.
    /// </summary>
    public void Close()
    {
        new NativeAction<nint>(GetVirtualFunction(6)).Invoke(Instance);
    }

    /// <summary>
    /// Flushes the stream.
    /// </summary>
    public void Flush()
    {
        new NativeAction<nint>(GetVirtualFunction(7)).Invoke(Instance);
    }

    /// <summary>
    /// Reads the specified number of bytes from the stream.
    /// </summary>
    /// <param name="count">The number of bytes to read.</param>
    /// <returns>The bytes read.</returns>
    /// <exception cref="Exception">Thrown if the number of bytes read does not match the number of bytes requested.</exception>
    public byte[] Read(long count)
    {
        var buffer = new byte[count];
        var read = Read(buffer);

        if (read != count)
            throw new Exception($"Failed to read {count} bytes from file. Only read {read} bytes.");

        return buffer;
    }

    /// <summary>
    /// Reads the specified number of bytes from the stream.
    /// </summary>
    /// <param name="buffer">The buffer to read into.</param>
    /// <param name="count">The number of bytes to read. If -1, it will read bytes equal to the length of the buffer.</param>
    /// <returns>The number of bytes read.</returns>
    public long Read(Span<byte> buffer, long count = -1)
    {
        if (count == -1)
            count = buffer.Length;

        fixed (byte* ptr = buffer)
        {
            return new NativeFunction<nint, nint, long, long>(GetVirtualFunction(8))
                .InvokeUnsafe(Instance, (nint)ptr, count);
        }
    }

    /// <summary>
    /// Writes the specified bytes to the stream.
    /// </summary>
    /// <param name="buffer">The bytes to write.</param>
    /// <returns>The number of bytes written.</returns>
    public long Write(ReadOnlySpan<byte> buffer)
    {
        fixed (byte* ptr = buffer)
        {
            return new NativeFunction<nint, nint, long, long>(GetVirtualFunction(9))
                .InvokeUnsafe(Instance, (nint)ptr, buffer.Length);
        }
    }

    /// <summary>
    /// Seeks to the specified position in the stream.
    /// </summary>
    /// <param name="offset">The offset to seek to.</param>
    /// <param name="origin">The origin to seek from.</param>
    /// <returns>The new position in the stream.</returns>
    public long Seek(long offset, SeekOrigin origin)
    {
        return new NativeFunction<nint, long, SeekOrigin, long>(GetVirtualFunction(12))
            .InvokeUnsafe(Instance, offset, origin);
    }

    /// <summary>
    /// Skips the specified number of bytes in the stream.
    /// </summary>
    /// <param name="count">The number of bytes to skip.</param>
    public void Skip(long count)
    {
        new NativeAction<nint, long>(GetVirtualFunction(13)).Invoke(Instance, count);
    }

    // MtStream VTable
    // [0] - Destructor
    // [1] - Unimplemented
    // [2] - Unimplemented
    // [3] - PopulateProperties
    // [4] - GetDti
    // [5] - GetPosition
    // [6] - Close
    // [7] - Flush
    // [8] - Read
    // [9] - Write
    // [10] - SetLength
    // [11] - GetLength
    // [12] - Seek
    // [13] - Skip
}
