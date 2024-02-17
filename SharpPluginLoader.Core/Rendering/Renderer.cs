using System.Numerics;
using System.Runtime.InteropServices;
using ImGuiNET;
using SharpPluginLoader.Core.IO;
using SharpPluginLoader.Core.Memory;
using SharpPluginLoader.Core.MtTypes;

namespace SharpPluginLoader.Core.Rendering
{
    public static class Renderer
    {
        /// <summary>
        /// Indicates whether the main SPL menu is currently shown.
        /// </summary>
        public static bool MenuShown => _showMenu;

#if DEBUG
        /// <summary>
        /// Indicates whether the ImGui demo window is currently shown.
        /// </summary>
        public static bool DemoShown => _showDemo;
#endif

        /// <summary>
        /// Loads a texture from the specified path.
        /// </summary>
        /// <param name="path">The path to the texture.</param>
        /// <returns>A handle to the loaded texture.</returns>
        /// <remarks>
        /// Textures can be any of the following formats: PNG, JPG, DDS
        /// </remarks>
        ///
        /// <example>
        /// <code>
        ///     var texture = Renderer.LoadTexture("./path/to/texture.png");
        ///     if (texture == TextureHandle.Invalid)
        ///         // handle error
        ///     ...
        ///     ImGui.Image(texture, new Vector2(100, 100));
        /// </code>
        /// </example>
        public static TextureHandle LoadTexture(string path)
        {
            return new TextureHandle(InternalCalls.LoadTexture(path));
        }

        [UnmanagedCallersOnly]
        internal static nint Initialize()
        {
            if (ImGui.GetCurrentContext() != 0)
                return ImGui.GetCurrentContext();
            
            ImGui.CreateContext();
            var io = ImGui.GetIO();
            io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;
            // Currently causes a freeze when dragging a window outside of the main window.
            // Most likely the WndProc doesn't process events anymore which causes windows to think it's frozen.
            // io.ConfigFlags |= ImGuiConfigFlags.ViewportsEnable;

            SetupImGuiStyle();

            _getCursorPositionHook = Hook.Create<GetCursorPositionDelegate>(0x1422e7010, GetCursorPositionHook);

            Log.Debug("Renderer.Initialize");

            return ImGui.GetCurrentContext();
        }

        [UnmanagedCallersOnly]
        internal static void Render()
        {
            foreach (var plugin in PluginManager.Instance.GetPlugins(p => p.OnRender))
                plugin.OnRender();
        }

        [UnmanagedCallersOnly]
        internal static unsafe nint ImGuiRender()
        {
            if (Input.IsPressed(Key.F9))
                _showMenu = !_showMenu;
            if (Input.IsPressed(Key.F10))
                _showDemo = !_showDemo;

            var io = ImGui.GetIO();
            var anyFocused = ImGui.IsWindowFocused(ImGuiFocusedFlags.AnyWindow);
            var anyHovered = ImGui.IsWindowHovered(ImGuiHoveredFlags.AnyWindow);
            io.MouseDrawCursor = anyFocused || anyHovered;

            if (_showMenu)
                ImGui.GetStyle().Alpha = anyFocused ? 1.0f : 0.5f;

            ImGui.NewFrame();
            if (_showMenu)
            {
                if (ImGui.Begin("SharpPluginLoader", ref _showMenu, ImGuiWindowFlags.MenuBar))
                {
                    if (ImGui.BeginMenuBar())
                    {
                        if (ImGui.BeginMenu("Options"))
                        {
                            ImGui.Checkbox("Draw Primitives as Wireframe",
                                                               ref MemoryUtil.AsRef(_renderingOptionPointers.DrawPrimitivesAsWireframe));

                            ImGui.SliderFloat("Line Thickness",
                                                               ref MemoryUtil.AsRef(_renderingOptionPointers.LineThickness),
                                                                                              1.0f, 10.0f);

                            ImGui.EndMenu();
                        }
                        ImGui.EndMenuBar();
                    }

                    foreach (var plugin in PluginManager.Instance.GetPlugins(pluginData => pluginData.OnImGuiRender))
                    {
                        if (plugin.PluginData.ImGuiWrappedInTreeNode)
                        {
                            if (ImGui.TreeNodeEx(plugin.Name,
                                    ImGuiTreeNodeFlags.FramePadding | ImGuiTreeNodeFlags.SpanAvailWidth))
                            {
                                plugin.OnImGuiRender();
                                ImGui.TreePop();
                            }
                        }
                        else
                        {
                            plugin.OnImGuiRender();
                        }
                    }
                }

                ImGui.End();
            }

            foreach (var plugin in PluginManager.Instance.GetPlugins(p => p.OnImGuiFreeRender))
                plugin.OnImGuiFreeRender();

#if DEBUG
            if (_showDemo)
                ImGui.ShowDemoWindow(ref _showDemo);
#endif

            ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 5f);
            InternalCalls.RenderNotifications();
            ImGui.PopStyleVar();

            ImGui.EndFrame();
            ImGui.Render();

            return (nint)ImGui.GetDrawData().NativePtr;
        }

        internal static void Shutdown()
        {
            ImGui.DestroyContext();
            Log.Debug("Renderer.Shutdown");
        }

        [UnmanagedCallersOnly]
        internal static unsafe void SetRenderingOptions(RenderingOptionPointers* pointers)
        {
            _renderingOptionPointers = *pointers;
        }

        private static void SetupImGuiStyle()
        {
            var style = ImGui.GetStyle();

            style.Alpha = 1.0f;
            style.DisabledAlpha = 1.0f;
            style.WindowPadding = new Vector2(12.0f, 12.0f);
            style.WindowRounding = 2.0f;
            style.WindowBorderSize = 1.0f;
            style.WindowMinSize = new Vector2(20.0f, 20.0f);
            style.WindowTitleAlign = new Vector2(0.5f, 0.5f);
            style.WindowMenuButtonPosition = ImGuiDir.None;
            style.ChildRounding = 0.0f;
            style.ChildBorderSize = 1.0f;
            style.PopupRounding = 0.0f;
            style.PopupBorderSize = 1.0f;
            style.FramePadding = new Vector2(6.0f, 6.0f);
            style.FrameRounding = 1.0f;
            style.FrameBorderSize = 0.0f;
            style.ItemSpacing = new Vector2(12.0f, 6.0f);
            style.ItemInnerSpacing = new Vector2(6.0f, 3.0f);
            style.CellPadding = new Vector2(12.0f, 6.0f);
            style.IndentSpacing = 20.0f;
            style.ColumnsMinSpacing = 6.0f;
            style.ScrollbarSize = 12.0f;
            style.ScrollbarRounding = 0.0f;
            style.GrabMinSize = 12.0f;
            style.GrabRounding = 1.0f;
            style.TabRounding = 0.0f;
            style.TabBorderSize = 0.0f;
            style.TabMinWidthForCloseButton = 0.0f;
            style.ColorButtonPosition = ImGuiDir.Right;
            style.ButtonTextAlign = new Vector2(0.5f, 0.5f);
            style.SelectableTextAlign = new Vector2(0.0f, 0.0f);

            style.Colors[(int)ImGuiCol.Text] = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
            style.Colors[(int)ImGuiCol.TextDisabled] = new Vector4(0.2745098173618317f, 0.3176470696926117f, 0.4509803950786591f, 1.0f);
            style.Colors[(int)ImGuiCol.WindowBg] = new Vector4(0.0784313753247261f, 0.08627451211214066f, 0.1019607856869698f, 1.0f);
            style.Colors[(int)ImGuiCol.ChildBg] = new Vector4(0.0784313753247261f, 0.08627451211214066f, 0.1019607856869698f, 1.0f);
            style.Colors[(int)ImGuiCol.PopupBg] = new Vector4(0.0784313753247261f, 0.08627451211214066f, 0.1019607856869698f, 1.0f);
            style.Colors[(int)ImGuiCol.Border] = new Vector4(0.1568627506494522f, 0.168627455830574f, 0.1921568661928177f, 1.0f);
            style.Colors[(int)ImGuiCol.BorderShadow] = new Vector4(0.0784313753247261f, 0.08627451211214066f, 0.1019607856869698f, 1.0f);
            style.Colors[(int)ImGuiCol.FrameBg] = new Vector4(0.1176470592617989f, 0.1333333402872086f, 0.1490196138620377f, 1.0f);
            style.Colors[(int)ImGuiCol.FrameBgHovered] = new Vector4(0.1568627506494522f, 0.168627455830574f, 0.1921568661928177f, 1.0f);
            style.Colors[(int)ImGuiCol.FrameBgActive] = new Vector4(0.2352941185235977f, 0.2156862765550613f, 0.5960784554481506f, 1.0f);
            style.Colors[(int)ImGuiCol.TitleBg] = new Vector4(0.0470588244497776f, 0.05490196123719215f, 0.07058823853731155f, 1.0f);
            style.Colors[(int)ImGuiCol.TitleBgActive] = new Vector4(0.0470588244497776f, 0.05490196123719215f, 0.07058823853731155f, 1.0f);
            style.Colors[(int)ImGuiCol.TitleBgCollapsed] = new Vector4(0.0784313753247261f, 0.08627451211214066f, 0.1019607856869698f, 1.0f);
            style.Colors[(int)ImGuiCol.MenuBarBg] = new Vector4(0.09803921729326248f, 0.105882354080677f, 0.1215686276555061f, 1.0f);
            style.Colors[(int)ImGuiCol.ScrollbarBg] = new Vector4(0.0470588244497776f, 0.05490196123719215f, 0.07058823853731155f, 1.0f);
            style.Colors[(int)ImGuiCol.ScrollbarGrab] = new Vector4(0.1176470592617989f, 0.1333333402872086f, 0.1490196138620377f, 1.0f);
            style.Colors[(int)ImGuiCol.ScrollbarGrabHovered] = new Vector4(0.1568627506494522f, 0.168627455830574f, 0.1921568661928177f, 1.0f);
            style.Colors[(int)ImGuiCol.ScrollbarGrabActive] = new Vector4(0.1176470592617989f, 0.1333333402872086f, 0.1490196138620377f, 1.0f);
            style.Colors[(int)ImGuiCol.CheckMark] = new Vector4(0.4980392158031464f, 0.5137255191802979f, 1.0f, 1.0f);
            style.Colors[(int)ImGuiCol.SliderGrab] = new Vector4(0.4980392158031464f, 0.5137255191802979f, 1.0f, 1.0f);
            style.Colors[(int)ImGuiCol.SliderGrabActive] = new Vector4(0.5372549295425415f, 0.5529412031173706f, 1.0f, 1.0f);
            style.Colors[(int)ImGuiCol.Button] = new Vector4(0.257f, 0.267f, 0.554f, 1.0f);
            style.Colors[(int)ImGuiCol.ButtonHovered] = new Vector4(0.196078434586525f, 0.1764705926179886f, 0.5450980663299561f, 1.0f);
            style.Colors[(int)ImGuiCol.ButtonActive] = new Vector4(0.2352941185235977f, 0.2156862765550613f, 0.5960784554481506f, 1.0f);
            style.Colors[(int)ImGuiCol.Header] = new Vector4(0.1176470592617989f, 0.1333333402872086f, 0.1490196138620377f, 1.0f);
            style.Colors[(int)ImGuiCol.HeaderHovered] = new Vector4(0.196078434586525f, 0.1764705926179886f, 0.5450980663299561f, 1.0f);
            style.Colors[(int)ImGuiCol.HeaderActive] = new Vector4(0.2352941185235977f, 0.2156862765550613f, 0.5960784554481506f, 1.0f);
            style.Colors[(int)ImGuiCol.Separator] = new Vector4(0.1568627506494522f, 0.1843137294054031f, 0.250980406999588f, 1.0f);
            style.Colors[(int)ImGuiCol.SeparatorHovered] = new Vector4(0.1568627506494522f, 0.1843137294054031f, 0.250980406999588f, 1.0f);
            style.Colors[(int)ImGuiCol.SeparatorActive] = new Vector4(0.1568627506494522f, 0.1843137294054031f, 0.250980406999588f, 1.0f);
            style.Colors[(int)ImGuiCol.ResizeGrip] = new Vector4(0.1176470592617989f, 0.1333333402872086f, 0.1490196138620377f, 1.0f);
            style.Colors[(int)ImGuiCol.ResizeGripHovered] = new Vector4(0.196078434586525f, 0.1764705926179886f, 0.5450980663299561f, 1.0f);
            style.Colors[(int)ImGuiCol.ResizeGripActive] = new Vector4(0.2352941185235977f, 0.2156862765550613f, 0.5960784554481506f, 1.0f);
            style.Colors[(int)ImGuiCol.Tab] = new Vector4(0.0470588244497776f, 0.05490196123719215f, 0.07058823853731155f, 1.0f);
            style.Colors[(int)ImGuiCol.TabHovered] = new Vector4(0.1176470592617989f, 0.1333333402872086f, 0.1490196138620377f, 1.0f);
            style.Colors[(int)ImGuiCol.TabActive] = new Vector4(0.09803921729326248f, 0.105882354080677f, 0.1215686276555061f, 1.0f);
            style.Colors[(int)ImGuiCol.TabUnfocused] = new Vector4(0.0470588244497776f, 0.05490196123719215f, 0.07058823853731155f, 1.0f);
            style.Colors[(int)ImGuiCol.TabUnfocusedActive] = new Vector4(0.0784313753247261f, 0.08627451211214066f, 0.1019607856869698f, 1.0f);
            style.Colors[(int)ImGuiCol.PlotLines] = new Vector4(0.5215686559677124f, 0.6000000238418579f, 0.7019608020782471f, 1.0f);
            style.Colors[(int)ImGuiCol.PlotLinesHovered] = new Vector4(0.03921568766236305f, 0.9803921580314636f, 0.9803921580314636f, 1.0f);
            style.Colors[(int)ImGuiCol.PlotHistogram] = new Vector4(1.0f, 0.2901960909366608f, 0.5960784554481506f, 1.0f);
            style.Colors[(int)ImGuiCol.PlotHistogramHovered] = new Vector4(0.9960784316062927f, 0.4745098054409027f, 0.6980392336845398f, 1.0f);
            style.Colors[(int)ImGuiCol.TableHeaderBg] = new Vector4(0.0470588244497776f, 0.05490196123719215f, 0.07058823853731155f, 1.0f);
            style.Colors[(int)ImGuiCol.TableBorderStrong] = new Vector4(0.0470588244497776f, 0.05490196123719215f, 0.07058823853731155f, 1.0f);
            style.Colors[(int)ImGuiCol.TableBorderLight] = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);
            style.Colors[(int)ImGuiCol.TableRowBg] = new Vector4(0.1176470592617989f, 0.1333333402872086f, 0.1490196138620377f, 1.0f);
            style.Colors[(int)ImGuiCol.TableRowBgAlt] = new Vector4(0.09803921729326248f, 0.105882354080677f, 0.1215686276555061f, 1.0f);
            style.Colors[(int)ImGuiCol.TextSelectedBg] = new Vector4(0.2352941185235977f, 0.2156862765550613f, 0.5960784554481506f, 1.0f);
            style.Colors[(int)ImGuiCol.DragDropTarget] = new Vector4(0.4980392158031464f, 0.5137255191802979f, 1.0f, 1.0f);
            style.Colors[(int)ImGuiCol.NavHighlight] = new Vector4(0.4980392158031464f, 0.5137255191802979f, 1.0f, 1.0f);
            style.Colors[(int)ImGuiCol.NavWindowingHighlight] = new Vector4(0.4980392158031464f, 0.5137255191802979f, 1.0f, 1.0f);
            style.Colors[(int)ImGuiCol.NavWindowingDimBg] = new Vector4(0.196078434586525f, 0.1764705926179886f, 0.5450980663299561f, 0.501960813999176f);
            style.Colors[(int)ImGuiCol.ModalWindowDimBg] = new Vector4(0.196078434586525f, 0.1764705926179886f, 0.5450980663299561f, 0.501960813999176f);
        }

        private static nint GetCursorPositionHook(nint app, out MtPoint pos)
        {
            var result = _getCursorPositionHook.Original(app, out pos);
            if (ImGui.GetCurrentContext() == 0)
                return result;

            if (ImGui.GetIO().MouseDrawCursor)
            {
                pos.X = 0;
                pos.Y = 0;
            }

            return result;
        }

        private delegate nint GetCursorPositionDelegate(nint app, out MtPoint pos);
        private static Hook<GetCursorPositionDelegate> _getCursorPositionHook = null!;
        private static bool _showMenu = false;
        private static bool _showDemo = false;
        private static RenderingOptionPointers _renderingOptionPointers;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct RenderingOptionPointers
    {
        public float* LineThickness;
        public bool* DrawPrimitivesAsWireframe;
    }
}
