using System;
using SharpPluginLoader.Core;
using ImGuiNET;

namespace DtiHierarchyVisualizer;

public class Plugin : IPlugin
{
    public string Name => "DTI Hierarchy Visualizer";
    public string Author => "Fexty";

    private string _className = "MtObject";
    private MtDti? _dti;

    public PluginData Initialize()
    {
        return new PluginData
        {
            OnImGuiRender = true
        };
    }

    public void OnLoad()
    {
        _dti = MtDti.Find(_className);
    }

    public void OnImGuiRender()
    {
        if (ImGui.InputText("Root Class", ref _className, 260))
            _dti = MtDti.Find(_className);
        ImGui.Separator();

        if (_dti is null)
        {
            ImGui.Text("Class not found!");
            return;
        }

        DisplayDti(_dti);
    }

    private static void DisplayDti(MtDti dti)
    {
        if (ImGui.TreeNode(dti.Name))
        {
            foreach (var child in dti.Children)
            {
                DisplayDti(child);
            }

            ImGui.TreePop();
        }
    }
}
