using System.Text;
using SharpPluginLoader.Core.Memory;

namespace SharpPluginLoader.Core.IO;

/// <summary>
/// Represents a file stream.
/// </summary>
public unsafe class MtFileStream : MtStream, IDisposable
{
    public MtFileStream(nint instance) : base(instance) { }
    public MtFileStream() { }

    /// <summary>
    /// Creates a new file stream from the specified file.
    /// </summary>
    /// <param name="file">The file to create the stream from.</param>
    /// <returns>The created file stream, or null if the file stream could not be created.</returns>
    public static MtFileStream? FromFile(MtFile file)
    {
        var dti = MtDti.Find("MtFileStream");
        if (dti is null)
            return null;

        var stream = dti.CreateInstance<MtFileStream>();
        stream._ownsPointer = true;
        stream._file = file;
        if (stream.Instance == 0)
            return null;

        var ctor = new NativeFunction<nint, nint, nint>(AddressRepository.Get("MtFileStream:Ctor"));
        ctor.Invoke(stream.Instance, file.Instance);

        return stream;
    }

    /// <summary>
    /// Opens a file stream from the specified path.
    /// </summary>
    /// <param name="path">The path of the file.</param>
    /// <param name="mode">The mode to open the file in.</param>
    /// <param name="createPath">Whether to create the path if it does not exist.</param>
    /// <returns>The opened file stream, or null if the file stream could not be opened.</returns>
    public static MtFileStream? FromPath(string path, OpenMode mode, bool createPath = true)
    {
        var file = MtFile.Open(path, mode, createPath);
        return file is null ? null : FromFile(file);
    }

    /// <summary>
    /// Writes the given string to the stream.
    /// </summary>
    /// <param name="str">The string to write.</param>
    /// <param name="encoding">The encoding to use. If null, UTF8 will be used.</param>
    /// <returns>The number of bytes written.</returns>
    public long Write(string str, Encoding? encoding = null)
    {
        encoding ??= Encoding.UTF8;
        return Write(encoding.GetBytes(str));
    }

    private bool _ownsPointer = false;
    private MtFile? _file = null;

    private void ReleaseUnmanagedResources()
    {
        if (_ownsPointer)
        {
            _file?.Dispose();
            Destroy(true);
        }
    }

    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    ~MtFileStream()
    {
        ReleaseUnmanagedResources();
    }
}
