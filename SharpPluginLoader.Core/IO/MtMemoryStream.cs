using SharpPluginLoader.Core.Memory;

namespace SharpPluginLoader.Core.IO;

/// <summary>
/// Represents a memory stream.
/// </summary>
public class MtMemoryStream : MtStream, IDisposable
{
    public MtMemoryStream(nint instance) : base(instance) { }
    public MtMemoryStream() { }

    /// <summary>
    /// Creates a new expandable memory stream.
    /// </summary>
    /// <returns>The new memory stream or null if it could not be created.</returns>
    public static MtMemoryStream? Create()
    {
        var dti = MtDti.Find("MtMemoryStream");
        if (dti is null)
            return null;

        var stream = dti.CreateInstance<MtMemoryStream>();
        stream._ownsPointer = true;

        return stream.Instance == 0 ? null : stream;
    }

    /// <summary>
    /// Creates a new memory stream from the specified buffer.
    /// </summary>
    /// <param name="buffer">The buffer to use</param>
    /// <returns>The new memory stream or null if it could not be created.</returns>
    /// <remarks>
    /// Note that the memory stream returned by this method is not expandable.
    /// It also does not take ownership of the buffer.
    /// </remarks>
    public static unsafe MtMemoryStream? Create(byte[] buffer)
    {
        var dti = MtDti.Find("MtMemoryStream");
        if (dti is null)
            return null;

        var stream = dti.CreateInstance<MtMemoryStream>();
        stream._ownsPointer = true;
        stream._buffer = buffer;

        var ctor = new NativeAction<nint, nint, long, MemoryStreamMode>(AddressRepository.Get("MtMemoryStream:Ctor"));
        fixed (byte* ptr = buffer)
            ctor.Invoke(stream.Instance, (nint)ptr, buffer.LongLength, MemoryStreamMode.Read | MemoryStreamMode.Write);

        return stream.Instance == 0 ? null : stream;
    }

    /// <summary>
    /// The buffer of this memory stream.
    /// </summary>
    public nint Buffer => Get<nint>(0x10);

    /// <summary>
    /// Whether this memory stream is overflowed.
    /// </summary>
    public bool IsBufferOverflowed => Get<bool>(0x2C);

    /// <summary>
    /// Gets the mode this memory stream was created with.
    /// </summary>
    public MemoryStreamMode Mode => Get<MemoryStreamMode>(0x28);


    private bool _ownsPointer = false;
    private byte[]? _buffer = null;

    private void ReleaseUnmanagedResources()
    {
        if (_ownsPointer)
        {
            _buffer = null;
            Destroy(true);
        }
    }

    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    ~MtMemoryStream()
    {
        ReleaseUnmanagedResources();
    }
}

[Flags]
public enum MemoryStreamMode
{
    Read = 1,
    Write = 2,
    Expand = 4
}
