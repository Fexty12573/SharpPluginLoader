
using System.Runtime.InteropServices;
using SharpPluginLoader.Core.Memory;
using SharpPluginLoader.Core.Rendering;

namespace SharpPluginLoader.Core.Resources;

public class Texture : Resource
{
    public Texture(nint instance) : base(instance) { }
    public Texture() { }

    public uint Width => Get<uint>(0xB8);

    public uint Height => Get<uint>(0xBC);

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
