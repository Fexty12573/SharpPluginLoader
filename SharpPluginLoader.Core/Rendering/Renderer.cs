using System.Numerics;
using System.Runtime.InteropServices;
using ImGuiNET;

namespace SharpPluginLoader.Core.Rendering
{
    internal static class Renderer
    {
        [UnmanagedCallersOnly]
        public static nint Initialize()
        {
            ImGui.CreateContext();

            ImGui.StyleColorsDark();
            var io = ImGui.GetIO();
            io.Fonts.AddFontDefault();
            io.Fonts.GetTexDataAsRGBA32(out nint _, out _, out _);
            unsafe { io.NativePtr->IniFilename = null; }

            Log.Debug("Renderer.Initialize");

            return ImGui.GetCurrentContext();
        }

        [UnmanagedCallersOnly]
        public static unsafe nint Render()
        {
            ImGui.NewFrame();

            foreach (var plugin in PluginManager.Instance.GetPlugins(p => p.OnRender))
            {
                if (ImGui.TreeNode(plugin.Name))
                {
                    plugin.OnRender();
                    ImGui.TreePop();
                }
            }

            ImGui.EndFrame();
            ImGui.Render();

            return (nint)ImGui.GetDrawData().NativePtr;
        }

        private static void Shutdown()
        {
            ImGui.DestroyContext();
            Log.Debug("Renderer.Shutdown");
        }

        private static void SetStyleColors()
        {
            var style = ImGui.GetStyle();
            style.WindowRounding = 0.0f;
            style.Colors[(int)ImGuiCol.WindowBg] = new Vector4(0.06f, 0.06f, 0.06f, 1.0f);
        }
    }
}
