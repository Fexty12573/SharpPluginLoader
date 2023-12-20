
using SharpPluginLoader.Core.Memory;

namespace SharpPluginLoader.Core.IO;

/// <summary>
/// Represents a file object.
/// </summary>
public unsafe class MtFile : MtObject, IDisposable
{
    public MtFile(nint instance) : base(instance) { _ownsPointer = false; }
    public MtFile() { _ownsPointer = false; }

    /// <summary>
    /// Opens a file.
    /// </summary>
    /// <param name="path">The path of the file.</param>
    /// <param name="mode">The mode to open the file in.</param>
    /// <returns>The opened file, or null if the file could not be opened.</returns>
    public static MtFile? Open(string path, OpenMode mode)
    {
        var dti = MtDti.Find("MtFile");
        if (dti is null) 
            return null;

        var file = new MtFile(MemoryUtil.Alloc(dti.Size))
        {
            _ownsPointer = true
        };

        var ctor = new NativeFunction<nint, string, OpenMode, uint, nint>(AddressRepository.Get("MtFile:Ctor"));
        ctor.Invoke(file.Instance, path, mode, 0);

        return file;
    }

    /// <summary>
    /// Gets the path of the file.
    /// </summary>
    public string Path => new(GetPtr<sbyte>(0x10));

    /// <summary>
    /// Gets the file pointer.
    /// </summary>
    public long Position => new NativeFunction<nint, long>(GetVirtualFunction(10)).InvokeUnsafe(Instance);

    /// <summary>
    /// Gets the size of the file.
    /// </summary>
    public long Size => new NativeFunction<nint, long>(GetVirtualFunction(11)).InvokeUnsafe(Instance);

    /// <summary>
    /// Whether the file is readable.
    /// </summary>
    public bool IsReadable => new NativeFunction<nint, bool>(GetVirtualFunction(13)).InvokeUnsafe(Instance);

    /// <summary>
    /// Whether the file is writable.
    /// </summary>
    public bool IsWritable => new NativeFunction<nint, bool>(GetVirtualFunction(14)).InvokeUnsafe(Instance);

    /// <summary>
    /// Reopens the file.
    /// </summary>
    /// <param name="path">The path of the file.</param>
    /// <param name="mode">The mode to open the file in.</param>
    /// <returns>Whether the file was reopened successfully.</returns>
    public bool Reopen(string path, OpenMode mode)
    {
        return new NativeFunction<nint, string, OpenMode, uint, bool>(GetVirtualFunction(5))
            .Invoke(Instance, path, mode, 0);
    }

    /// <summary>
    /// Closes the file.
    /// </summary>
    /// <remarks>
    /// This is called automatically when the file object is destroyed or disposed.
    /// </remarks>
    public void Close()
    {
        new NativeAction<nint>(GetVirtualFunction(6)).Invoke(Instance);
    }

    /// <summary>
    /// Reads the specified number of bytes from the file.
    /// </summary>
    /// <param name="size">The number of bytes to read.</param>
    /// <returns>The bytes read.</returns>
    /// <exception cref="Exception">Thrown if the number of bytes read does not match the number of bytes requested.</exception>
    public byte[] Read(long size)
    {
        var buffer = new byte[size];
        var read = Read(buffer);
        if (read != size)
            throw new Exception($"Failed to read {size} bytes from file. Only read {read} bytes.");

        return buffer;
    }

    /// <summary>
    /// Reads the specified number of bytes from the file.
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
            return new NativeFunction<nint, nint, long, long>(GetVirtualFunction(7))
                .Invoke(Instance, (nint)ptr, count);
        }
    }

    /// <summary>
    /// Writes the specified bytes to the file.
    /// </summary>
    /// <param name="buffer">The bytes to write.</param>
    /// <returns>The number of bytes written.</returns>
    public long Write(ReadOnlySpan<byte> buffer)
    {
        fixed (byte* ptr = buffer)
        {
            return new NativeFunction<nint, nint, long, long>(GetVirtualFunction(8))
                .Invoke(Instance, (nint)ptr, buffer.Length);
        }
    }

    /// <summary>
    /// Seeks to the specified position in the file.
    /// </summary>
    /// <param name="offset">The offset to seek to.</param>
    /// <param name="origin">The origin to seek from.</param>
    /// <returns>The new position in the file.</returns>
    public long Seek(long offset, SeekOrigin origin)
    {
        return new NativeFunction<nint, long, SeekOrigin, long>(GetVirtualFunction(9))
            .Invoke(Instance, offset, origin);
    }

    /// <summary>
    /// Sets the length of the file.
    /// </summary>
    /// <param name="length">The new length of the file.</param>
    public void SetLength(long length)
    {
        new NativeAction<nint, long>(GetVirtualFunction(12)).Invoke(Instance, length);
    }

    // MtFile VTable
    // [0] - Destructor
    // [1] - Unimplemented
    // [2] - Unimplemented
    // [3] - Unimplemented
    // [4] - GetDti
    // [5] - Open
    // [6] - Close
    // [7] - Read
    // [8] - Write
    // [9] - Seek
    // [10] - GetPosition
    // [11] - Length
    // [12] - SetLength
    // [13] - IsReadable
    // [14] - IsWritable

    private bool _ownsPointer;

    private void ReleaseUnmanagedResources()
    {
        if (_ownsPointer)
        {
            Close();
            MemoryUtil.Free(Instance);
        }
    }

    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    ~MtFile()
    {
        ReleaseUnmanagedResources();
    }
}

/// <summary>
/// Specifies how to open a file.
/// </summary>
public enum OpenMode
{
    /// <summary>
    /// Do not open the file.
    /// </summary>
    None = 0,

    /// <summary>
    /// Open the file for reading. If the file does not exist, an exception will be thrown.
    /// </summary>
    Read = 2,

    /// <summary>
    /// Open the file for writing. If the file exists, it will be truncated. If it does not exist, it will be created.
    /// </summary>
    Write = 3,

    /// <summary>
    /// Open the file for writing. If the file exists, it will be appended to. If it does not exist, it will be created.
    /// </summary>
    WriteAppend = 4,

    /// <summary>
    /// Open the file for reading and writing. If the file exists, it will be truncated. If it does not exist, it will be created.
    /// </summary>
    ReadWrite = 5,

    /// <summary>
    /// Open the file for reading and writing. If the file exists, it will be appended to. If it does not exist, it will be created.
    /// </summary>
    ReadWriteAppend = 6,

    /// <inheritdoc cref="Read"/>
    Read2 = 7
}
