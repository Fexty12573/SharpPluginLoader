
namespace SharpPluginLoader.Core.Rendering;

/// <summary>
/// Represents a texture handle that can be passed to ImGui.
/// </summary>
/// <param name="handle">The handle</param>
/// <remarks>
/// You should generally not have to create instances of this struct yourself.
/// Use <see cref="Renderer.LoadTexture"/> to load a texture and get a handle.
/// <para/>
/// If for some reason you <i>do</i> need to create a handle manually, the handle should be:
/// <list type="bullet">
/// <item>For D3D11: A <c>ID3D11ShaderResourceView*</c></item>
/// <item>For D3D12: A <c>D3D12_GPU_DESCRIPTOR_HANDLE</c></item>
/// </list>
/// </remarks>
public readonly struct TextureHandle(nint handle) : IEquatable<TextureHandle>, IDisposable
{
    private readonly nint _handle = handle;

    public static implicit operator TextureHandle(nint handle) => new(handle);
    public static implicit operator nint(TextureHandle handle) => handle._handle;

    public static TextureHandle Invalid => new(nint.Zero);

    #region Equality Operators

    public override bool Equals(object? obj) => obj is TextureHandle h && Equals(h);
    public bool Equals(TextureHandle other) => _handle == other._handle;

    public static bool operator ==(TextureHandle left, TextureHandle right) => left._handle == right._handle;
    public static bool operator !=(TextureHandle left, TextureHandle right) => left._handle != right._handle;
    public static bool operator ==(TextureHandle left, nint right) => left._handle == right;
    public static bool operator !=(TextureHandle left, nint right) => left._handle != right;
    public static bool operator ==(nint left, TextureHandle right) => left == right._handle;
    public static bool operator !=(nint left, TextureHandle right) => left != right._handle;

    #endregion

    public override int GetHashCode() => _handle.GetHashCode();

    public void Dispose()
    {
        InternalCalls.UnloadTexture(_handle);
    }
}
