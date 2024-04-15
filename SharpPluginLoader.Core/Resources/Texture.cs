
using System.Runtime.InteropServices;
using SharpPluginLoader.Core.Memory;
using SharpPluginLoader.Core.Rendering;

namespace SharpPluginLoader.Core.Resources;

/// <summary>
/// Represents a texture resource (an instance of the rTexture class).
/// </summary>
public class Texture : Resource
{
    public Texture(nint instance) : base(instance) { }
    public Texture() { }

    /// <summary>
    /// The width of the texture.
    /// </summary>
    public uint Width => Get<uint>(0xB8);

    /// <summary>
    /// The height of the texture.
    /// </summary>
    public uint Height => Get<uint>(0xBC);

    /// <summary>
    /// Obtains a handle to the texture, which can be passed to ImGui. This method actually uploads the texture to the GPU and creates the resource object.
    /// </summary>
    /// <returns>A handle to the texture, or <see cref="TextureHandle.Invalid"/> if the texture couldn't be created.</returns>
    /// <remarks>
    /// Textures are cached internally, so calling this method multiple times for the same texture will return the same handle.
    /// </remarks>
    public TextureHandle GetTextureHandle()
    {
        return Renderer.IsDirectX12 
            ? GetTextureHandle12() 
            : GetTextureHandle11();
    }

    private TextureHandle GetTextureHandle11()
    {
        var shaderRes = GetShaderResource();
        if (shaderRes is null)
            return TextureHandle.Invalid;

        if (shaderRes.Srv != TextureHandle.Invalid)
            return shaderRes.Srv;

        return InternalCalls.RegisterTexture(shaderRes.Buffer.Resource11);
    }

    private TextureHandle GetTextureHandle12()
    {
        var shaderRes = GetShaderResource();
        if (shaderRes is null)
            return TextureHandle.Invalid;

        return InternalCalls.RegisterTexture(shaderRes.Buffer.Resource12);
    }

    private ShaderResource<DrawTexture>? GetShaderResource() => GetObject<ShaderResource<DrawTexture>>(0xA8);
}

internal class ShaderResource<T> : MtObject where T : unmanaged
{
    public ShaderResource(nint instance) : base(instance) { }
    public ShaderResource() { }

    public uint Usage => Get<uint>(0x58);

    public uint Size => Get<uint>(0x5C);

    public unsafe ref T Buffer => ref *GetPtr<T>(0x60);

    public TextureHandle Srv => MemoryUtil.Read<nint>(Get<nint>(0x78) + 0x10);
}

[StructLayout(LayoutKind.Explicit, Size = 0xF0)]
internal struct DrawTexture
{
    [FieldOffset(0x18)] public nint Resource11;
    [FieldOffset(0x20)] public nint Resource12;
}
