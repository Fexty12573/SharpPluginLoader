
using System.Runtime.InteropServices;
using System.Text;
using SharpPluginLoader.Core.Memory;

namespace SharpPluginLoader.Core.IO;

/// <summary>
/// Represents a cipher stream. Can be used to encrypt/decrypt data from a stream.
/// </summary>
public class MtCipherStream : MtStream, IDisposable
{
    public MtCipherStream(nint instance) : base(instance) { }
    public MtCipherStream() { }

    /// <summary>
    /// Creates a new cipher stream from the specified stream and key.
    /// </summary>
    /// <param name="stream">The stream to encrypt/decrypt.</param>
    /// <param name="mode">The mode to use.</param>
    /// <param name="key">The key to use.</param>
    /// <returns>The cipher stream, or null if it could not be created.</returns>
    public unsafe MtCipherStream? FromStream(MtStream stream, CipherStreamMode mode, string key)
    {
        var dti = MtDti.Find("MtCipherStream");
        if (dti is null)
            return null;

        var cstream = dti.CreateInstance<MtCipherStream>();
        cstream._ownsPointer = true;
        if (cstream.Instance == 0)
            return null;
        
        byte[] keyBytes = [..Encoding.UTF8.GetBytes(key), 0];
        cstream._keyPointer = MemoryUtil.Alloc(keyBytes.LongLength);
        MemoryUtil.WriteBytes(cstream._keyPointer, keyBytes);

        var ctor = new NativeAction<nint, CipherStreamMode, nint, nint, uint>(
            AddressRepository.Get("MtCipherStream:Ctor"));
        ctor.Invoke(cstream.Instance, mode, stream.Instance, cstream._keyPointer, 0x400);

        return cstream;
    }

    private bool _ownsPointer = false;
    private nint _keyPointer = 0;

    private void ReleaseUnmanagedResources()
    {
        if (_ownsPointer)
        {
            Destroy(true);
            MemoryUtil.Free(_keyPointer);
        }
    }

    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    ~MtCipherStream()
    {
        ReleaseUnmanagedResources();
    }
}

public enum CipherStreamMode
{
    Read = 1,
    Write = 2
}
