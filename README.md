# SharpPluginLoader

A C# plugin loader for Monster Hunter World based on .NET 8.0.

## How to use
1. Install [.NET 8.0](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) (Get the **.NET Desktop Runtime 8.0.0-rc.1**)
2. Download the latest release from the [releases page]() and extract it into your Monster Hunter World directory.

## Plugin Development
1. Install the Visual Studio 2022 17.8 Preview 2 or later.
2. Create a new **.NET 8.0** class library project and add a reference to `SharpPluginLoader.Core.dll`. 
3. Create a class that implements the `SharpPluginLoader.Core.IPlugin` interface.
4. Implement the required methods.
5. Put the compiled assembly into `nativePC/plugins/CSharp`. Assemblies are also allowed to be in subdirectories.

## Dependency Resolution
The native host (`SharpPluginLoader.Native.dll`) loads the Bootstrapper assembly (`SharpPluginLoader.Bootstrapper.dll`)
into the default AssemblyLoadContext (ALC). The Bootstrapper then loads the core assembly into a custom ALC (`CoreLoadContext`).

The `CoreLoadContext` resolves all dependencies either from the current directory, or via the Default ALC.

The core assembly then loads all plugins into a custom ALC (`PluginLoadContext`) and resolves all dependencies from the `CoreLoadContext`. Each plugin is loaded into a separate ALC.
