# SharpPluginLoader

A C# plugin loader for Monster Hunter World based on .NET 8.0.

For more detailed documentation and tutorials, visit the [wiki](https://fexty12573.github.io/SharpPluginLoader/).

## How to use
1. Install [.NET 8.0](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) (Get the **.NET Desktop Runtime 8.0.0**)
2. Download the latest release from the [releases page]() and extract it into your Monster Hunter World directory.

## Plugin Development
For more detailed instructions visit the [wiki](https://fexty12573.github.io/SharpPluginLoader/Development/).
1. Install the Visual Studio 2022 17.8 Preview 2 or later.
2. Create a new **.NET 8.0** class library project and add a reference to `SharpPluginLoader.Core.dll`. 
3. Create a class that implements the `SharpPluginLoader.Core.IPlugin` interface.
4. Implement the required methods.
5. Put the compiled assembly into `nativePC/plugins/CSharp`. Assemblies are also allowed to be in subdirectories.

## Framework Development (with Visual Studio 2022)
1. Clone the repository with submodules:
    1. `git clone --recursive git@github.com:Fexty12573/SharpPluginLoader.git`
2. Setup vcpkg IDE integration if you haven't already done so:
    1. Open `Developer PowerShell for Visual Studio`
    2. Run `vcpkg integrate install`
3. Generate cimgui VS project:
    1. Open `Developer PowerShell for Visual Studio`
    2. `cd SharpPluginLoader\dependencies\cimgui\`
    3. `cmake -B . -G 'Visual Studio 17 2022'`
4. Open `mhw-cs-plugin-loader.sln`
5. Build solution `Build -> Build Solution`

## Dependency Resolution
The native host (`SharpPluginLoader.Native.dll`) loads the Bootstrapper assembly (`SharpPluginLoader.Bootstrapper.dll`)
into the default AssemblyLoadContext (ALC). The Bootstrapper then loads the core assembly into a custom ALC (`CoreLoadContext`).

The `CoreLoadContext` resolves all dependencies either from the current directory, or via the Default ALC.

The core assembly then loads all plugins into a custom ALC (`PluginLoadContext`) and resolves all dependencies from the `CoreLoadContext`. Each plugin is loaded into a separate ALC.

## **Enabling C# Debugging**
1. Make sure all projects are compiled in **Debug** mode.
2. Open the `mhw-cs-plugin-loader` project properties, make sure the **Debug** configuration is selected and go to General > Debugging. Here set the Debugger Type to **Mixed (.NET Core)**.
3. In the Attach to Process dialog, make sure **Managed (.NET Core, .NET 5+)** and **Native** are selected.

## **Hosting Docs Locally**
1. `python -m venv venv`
2. `venv\Scripts\activate`
3. `pip install -r requirements.txt`
4. `mkdocs serve`

## **Libraries Used**
- [safetyhook](https://github.com/cursey/safetyhook) - Native hooking library
- [Reloaded.Hooks](https://github.com/Reloaded-Project/Reloaded.Hooks) - Managed hooking library
- [cimgui](https://github.com/cimgui/cimgui) - C wrapper for Dear ImGui
- [Dear ImGui](https://github.com/ocornut/imgui) - GUI library
- [ImGui.NET](https://github.com/ImGuiNET/ImGui.NET) - C# wrapper for Dear ImGui
- [tinyobjloader](https://github.com/tinyobjloader/tinyobjloader) - Wavefront OBJ loader
- [nlohmann-json](https://github.com/nlohmann/json) - C++ JSON library
- [zlib](https://www.zlib.net/) - Compression library
- [DirectXMath](https://github.com/microsoft/DirectXMath) - DirectX Math library
- [PicoSHA2](https://github.com/okdshin/PicoSHA2) - header-only SHA256 library