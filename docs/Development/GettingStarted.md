# Getting Started

## Introduction
This section will cover how to get a basic plugin up and running, and give a brief overview of the API.
If you want to learn more about the API, check out the [API Reference](../API/index.md) page.
You can also check some more advanced examples in the [Examples](../Examples/index.md) page.

## Creating a new project
To get started, open Visual Studio and create a new project. Select the **Class Library (.NET)** template.
Once created, right click the project and select `Add > Project Reference...` then `Browse` and select the `SharpPluginLoader.Core.dll`,
found inside `nativePC\plugins\CSharp\Loader`.

!!! info
    It is also recommended to enable unsafe code in the project properties, as it is required for some features such as native function wrappers.

## Entry point
Each plugin has an entry point class, which must implement the `IPlugin` interface inside the `SharpPluginLoader.Core` namespace.
```csharp title="ExamplePlugin.cs" linenums="1" hl_lines="7 9"
using SharpPluginLoader.Core;

namespace Example
{
    public class ExamplePlugin : IPlugin
    {
        public string Name => "Example Plugin";

        public PluginData OnLoad()
        {
            // ...
        }
    }
}
```
Take a look at the highlighted lines. These are the only required methods and properties for a plugin to work.
The `Name` property is used to identify the plugin, and the `OnLoad` method is called when the plugin is loaded.

## Events
You need to return a `PluginData` object from the `OnLoad` method. This object will contain information about which events the plugin subscribes to.
```csharp 
public PluginData OnLoad()
{
    return new PluginData
    {
        OnUpdate = true
    }
}
```
In this example, the plugin will subscribe to the `OnUpdate` event, which is called every frame. For a list of all available events, check the [API Reference](../API/index.md).

### Event handlers
Once you subscribe to an event by setting its value to `true`, you need to create a method with the same name as the event, inside your plugin class.
```csharp
public void OnUpdate(float deltaTime)
{
    Log.Info("Hello!");
}
```
All event methods implement an interface method provided by the `IPlugin` interface. You can check reference on `IPlugin` for details on each event.
If you subscribe to an event and forget to implement that method your plugin will throw an exception when that event is called.

The above code simply prints "Hello!" to the console every frame.

## Loading the plugin
Now you can compile your plugin (either in Debug or Release mode) and place the resulting DLL inside the `nativePC\plugins\CSharp` folder (or any subdirectory
inside it). The plugin will be loaded automatically when the game starts.
