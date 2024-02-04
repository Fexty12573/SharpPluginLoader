using System.Collections;
using System.Collections.Specialized;
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
            //_createShinyDropHook = MarshallingHook.Create<CreateShinyDropDelegate>(CreateShinyDropHook, 0x1402cb1d0);
            //_releaseResourceHook = MarshallingHook.Create<ReleaseResourceDelegate>(ReleaseResourceHook, 0x142224890);
        }

        public void OnImGuiRender()
        {
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
        }
    }
}
