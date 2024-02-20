# Rendering with ImGui

## Introduction
The framework also provides a way to render your own UI to the screen using [Dear ImGui](https://github.com/ocornut/imgui/tree/master). ImGui is a very powerful UI library that is very easy to use and has a lot of features.

To get started add the `SharpPluginLoader.ImGui` NuGet package to your project. You can do this by right-clicking your project in the solution explorer and selecting "Manage NuGet Packages". Then search for `SharpPluginLoader.ImGui` and install it.

Next add an event handler for the `OnImGuiRender` event to your plugin. This event is called every frame and is where you will do all your rendering.
```csharp
public void OnImGuiRender()
{
    // Render your UI here
}
```

## ImGui
Now you are ready to draw your own UI. 
```csharp
using ImGuiNET;

// ...

public void OnImGuiRender()
{
    ImGui.Text("Hello World!");
    if (ImGui.Button("Click me!"))
    {
        Log.Info("Button clicked!");
    }
}
```
By default all your ImGui Draw-calls will be contained in a collapsing header inside the main SPL ImGui window. You can disable this by setting `ImGuiWrappedInTreeNode` to false in your `PluginData` object.
```csharp
public PluginData Inititalize()
{
    return new PluginData
    {
        ImGuiWrappedInTreeNode = false
    };
}
```

If you want your UI to be contained in its own window, you can instead subscribe to the `OnImGuiFreeRender` event. This event is called after the main ImGui window has been rendered, and is where you can render your own ImGui windows.
```csharp
public void OnImGuiFreeRender()
{
    ImGui.Begin("My Window");
    ImGui.Text("Hello World!");
    ImGui.End();
}
```

For more information on how to use ImGui, you can check out the [ImGui Demo](https://github.com/ocornut/imgui/blob/docking/imgui_demo.cpp).
