# Getting Started

## Introduction
This section will cover how to get a basic plugin up and running, and give a brief overview of the API.
If you want to learn more about the API, check out the [API Reference](../API/index.md) page.
You can also check some more advanced examples in the [Examples](../Examples/index.md) page.

## Creating a new project
To get started, open Visual Studio and create a new project. Select the **Class Library (.NET)** template. Make sure the target framework is set to **.NET 8.0**.
Once created, right click your project in the Solution Explorer and select **Manage NuGet Packages...**. In the **Browse** tab, search for `SharpPluginLoader.Core`.
Install the latest version of the package. 

Alternatively you can install the package from the command line using the `dotnet` tool.
```bat
dotnet add package SharpPluginLoader.Core
```

!!! info
    It is also recommended to enable unsafe code in the project properties, as it is required for some features such as native function wrappers.

## Entry point
Each plugin has an entry point class, which must implement the `IPlugin` interface inside the `SharpPluginLoader.Core` namespace.
```csharp title="ExamplePlugin.cs" linenums="1" hl_lines="7 8"
using SharpPluginLoader.Core;

namespace Example;

public class ExamplePlugin : IPlugin
{
    public string Name => "Example Plugin";
    public string Author => "Fexty";
}
```
The above is technically already a valid plugin, but it doesn't do anything yet. The `IPlugin` interface has a few methods that you can override to add functionality to your plugin.

* The `Name` property is used to identify the plugin.
* The `Author` property is used to identify the author of the plugin.

## Default methods
There are 2 methods that will always be called when the plugin is loaded/reloaded. Both of them are *optional* to implement.
```csharp
public PluginData Initialize()
{
    return new PluginData();
}

public void OnLoad()
{
}
```

In the `Initialize` method you need to return a `PluginData` object, which contains some optional details about the plugin.

!!! info
    You should not do any game-related initialization in the `Initialize` method, as it is called before the game is fully loaded. Use the `OnLoad` method for that.

The `OnLoad` method is where you will normally do any setup that you need to do when the plugin is loaded. This method is called every time the plugin is loaded, so it's a good place to initialize any resources that you need.

## Events
There is a set of events exposed by the `IPlugin` interface that you can subscribe to. You "subscribe" to an event by implementing the corresponding interface method in your plugin class. 
```csharp
public void OnUpdate(float deltaTime)
{
    Log.Info("Hello!");
}
```
The above code subscribes to the `OnUpdate` event, which is called every frame.

For a full list of events, check the [API Reference](../API/SharpPluginLoader.Core.IPlugin.md).

## Loading the plugin
Now you can compile your plugin (either in Debug or Release mode) and place the resulting DLL inside the `nativePC\plugins\CSharp` folder (or any subdirectory
inside it). The plugin will be loaded automatically when the game starts.

## Debugging
Game update 15.20 removed all anti-debug measures that the game has in place. Now you can attach the Visual Studio debugger directly to the game process.

To debug your plugin compile it in Debug mode, put the DLL inside the `nativePC\plugins\CSharp` folder and start the game. 
Then, in Visual Studio, go to `Debug > Attach to Process...` and select `MonsterHunterWorld.exe` from the list. You should now be able to place
breakpoints and debug your plugin as you would with any other project.

Additionally, for some light-weight "debugging" you can use the `Log` class to print messages to the console. 
```csharp
Log.Info("Hello!");
Log.Error("Something went wrong!");
```
