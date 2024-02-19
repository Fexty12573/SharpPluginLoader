using System.Collections;
using System.Collections.Specialized;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using ImGuiNET;
using SharpPluginLoader.Core;
using SharpPluginLoader.Core.Entities;
using SharpPluginLoader.Core.Experimental;
using SharpPluginLoader.Core.Rendering;
using SharpPluginLoader.Core.Resources;

namespace ExperimentalTesting
{
    public class Plugin : IPlugin
    {
        public string Name => "Experimental Testing";

        public delegate void CreateShinyDropDelegate(MtObject a, int b, int c, nint d, long e, uint f);
        public delegate void ReleaseResourceDelegate(MtObject resourceMgr, Resource resource);
        private MarshallingHook<CreateShinyDropDelegate> _createShinyDropHook = null!;
        private MarshallingHook<ReleaseResourceDelegate> _releaseResourceHook = null!;
        private int _value = 5;
        private string _str = "Hello World!";
        private long _a, _b, _sum;

        private TextureHandle _texture = TextureHandle.Invalid;
        private uint _textureWidth, _textureHeight;
        private bool _firstRender = true;
        private string _texturePath = @"em\em001\00\mod\em001_00_BML";
        private MtDti _texDti = null!;
        private bool _showRealSize = false;

        private (Texture texture, TextureHandle handle) _loadedTexture = (null!, TextureHandle.Invalid);

        public unsafe void CreateShinyDropHook(MtObject a, int b, int c, nint d, long e, uint f)
        {
            Log.Info($"CreateShinyDropHook: {a},{b},{c},{d},{e},{f}");
            _createShinyDropHook.Original(a, b, c, d, e, f);
        }

        public unsafe void ReleaseResourceHook(MtObject resourceMgr, Resource resource)
        {
            Log.Info($"Releasing Resource: {resource.FilePath}.{resource.FileExtension}" + 
                     (resource.Get<int>(0x5C) == 1 ? " | Unloading..." : ""));
            _releaseResourceHook.Original(resourceMgr, resource);
        }

        public PluginData Initialize()
        {
            return new PluginData
            {
                OnImGuiRender = true
            };
        }

        public void OnLoad()
        {
            _texDti = MtDti.Find("rTexture")!;
            Ensure.NotNull(_texDti);

            //_createShinyDropHook = MarshallingHook.Create<CreateShinyDropDelegate>(CreateShinyDropHook, 0x1402cb1d0);
            //_releaseResourceHook = MarshallingHook.Create<ReleaseResourceDelegate>(ReleaseResourceHook, 0x142224890);
        }

        public void OnImGuiRender()
        {
            if (_firstRender)
            {
                _texture = Renderer.LoadTexture("./yodabruh.png", out _textureWidth, out _textureHeight);
                _firstRender = false;

                if (_texture == TextureHandle.Invalid)
                    Log.Error("Failed to load texture!");
                else
                    Log.Info("Texture loaded!");
            }

            ImGui.InputText("Message", ref _str, 512);
            ImGui.SameLine();
            if (ImGui.Button("Display Fatal Error"))
                InternalCalls.DisplayFatalErrorMessage(_str);

            ImGui.InputInt("HRESULT", ref _value);
            ImGui.SameLine();
            if (ImGui.Button("Display Error"))
                InternalCalls.D3DCheckResult(_value, "SPL: Test Error message");

            ImGui.Separator();

            ImGuiExtensions.InputScalar("A", ref _a);
            ImGui.SameLine();
            ImGuiExtensions.InputScalar("B", ref _b);
            ImGui.SameLine();
            ImGui.Text($"Sum: {InternalCalls.Sum(_a, _b)}");

            ImGui.Separator();

            ImGui.InputInt("Value", ref _value);
            ImGui.SameLine();
            if (ImGui.Button("Modify"))
                InternalCalls.ModifyValue(ref _value);

            if (_texture != TextureHandle.Invalid)
            {
                ImGui.Image(_texture, new Vector2(_textureWidth, _textureHeight));
            }

            ImGui.Separator();

            ImGui.InputText("Texture Path", ref _texturePath, 260);
            ImGui.SameLine();
            if (ImGui.Button("Load Texture"))
            {
                var tex = ResourceManager.GetResource<Texture>(_texturePath, _texDti);
                if (tex is null)
                {
                    Log.Error("Failed to load texture!");
                    return;
                }

                _loadedTexture = (tex, tex.GetTextureHandle());
            }

            ImGui.SameLine();
            ImGui.Checkbox("Real Size", ref _showRealSize);

            if (_loadedTexture.handle != TextureHandle.Invalid)
            {
                ImGui.Image(_loadedTexture.handle, _showRealSize
                    ? new Vector2(_loadedTexture.texture.Width, _loadedTexture.texture.Height)
                    : new Vector2(256, 256)
                );
            }
        }
    }
}
