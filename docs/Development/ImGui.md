# Rendering with ImGui

## Introduction
The framework also provides a way to render your own UI to the screen using [Dear ImGui](https://github.com/ocornut/imgui/tree/master). ImGui is a very powerful UI library that is very easy to use and has a lot of features.

To get started add the `ImGui.NET` NuGet package to your project. You can do this by right-clicking your project in the solution explorer and selecting "Manage NuGet Packages". Then search for `ImGui.NET` and install it.

Next add an event handler for the `OnRender` event to your plugin. This event is called every frame and is where you will do all your rendering.
```csharp
public PluginData OnLoad()
{
    return new PluginData
    {
        OnRender = true
    };
}

public void OnRender()
{
    // Render your UI here
}
```

## ImGui
Now you are ready to draw your own UI. 
```csharp
using ImGuiNET;

// ...

public void OnRender()
{
    ImGui.Text("Hello World!");
    if (ImGui.Button("Click me!"))
    {
        Log.Info("Button clicked!");
    }
}
```

By default all your ImGui Draw-calls will be rendered to a main window. You can create your own windows by using the `ImGui.Begin` and `ImGui.End` methods.
```csharp
public void OnRender()
{
    ImGui.Begin("My Window");
    ImGui.Text("Hello World!");
    ImGui.End();
}
```

For more information on how to use ImGui, you can check out the [ImGui Demo](https://github.com/ocornut/imgui/blob/docking/imgui_demo.cpp).
