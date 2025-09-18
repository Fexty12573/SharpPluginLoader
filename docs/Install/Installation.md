# Installation
## Windows
The mod is based on .NET 8. You will need to install the [.NET 8.0 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/8.0/runtime) to use it.
Grab the x64 Desktop Runtime and install it.

Once you have the .NET runtime installed you can grab the latest release from the [Releases Page](https://github.com/Fexty12573/SharpPluginLoader/releases).
Extract the contents of the archive into the game's root directory (where `MonsterHunterWorld.exe` is located).

If you installed everything correctly you should now find `winmm.dll` in the same directory as `MonsterHunterWorld.exe`, and a `CSharp` directory in `nativePC\plugins`.

## Linux (Proton/Wine)
As of version 0.0.7.2, SPL officially supports Linux through Proton/Wine. Below are the steps to install and run SPL on Linux.

1. Install .NET Desktop Runtime 8.0 and Direct 3D Shader Compiler using [protontricks](https://github.com/Matoking/protontricks):
```bash
protontricks 582010 dotnetdesktop8 d3dcompiler_47
```
2. Download the latest linux release of SPL (`SharpPluginLoader-<version>-linux.zip`) from the [Releases Page](https://github.com/Fexty12573/SharpPluginLoader/releases) and extract it into the game's root directory. After doing so you should have a `msvcrt.dll` file in the same directory as `MonsterHunterWorld.exe`.
3. Set the steam launch options for MHW as follows:
```bash
# Use this for SPL only
WINEDLLOVERRIDES="msvcrt=n,b" %command%

# Or this for SPL together with Stracker's Loader
WINEDLLOVERRIDES="msvcrt,dinput8=n,b" %command%
```

## Usage
### Config
SPL configuration is stored in `loader-config.json` in the same directory as `MonsterHunterWorld.exe`. It will be automatically generated when using SPL.

### Menu
Press `F9` to access the SPL menu in-game. Plugins may add various UI in this menu.

### Plugins
Any C# plugins will be placed directly into the `CSharp` directory. The plugin loader will automatically load all DLLs in this directory.
Subdirectories are also supported, so you can organize your plugins however you want.

> [!NOTE]
> Putting plugins inside the `nativePC\plugins\CSharp\Loader` directory will **not** work. The plugin loader will not load them.
> See [below](#directory-structure-examples) for an example of a valid directory structure.

Once you have all your plugins installed you can simply start the game. The plugin loader will automatically load all plugins.
Depending on the plugins you have installed you might also see an overlay/UI appear on the screen.

### Directory Structure Examples
```
<Root game directory>
└── winmm.dll/msvcrt.dll
└── nativePC
    └── plugins
        └── CSharp
            ├── Plugin1
            │   └── Plugin1.dll
            ├── Plugin2
            │   └── Plugin2.dll
            └── Plugin3.dll
```
Conversely, the following is **not** valid, as `Plugin1.dll` is inside the `Loader` directory. The plugin loader will not load it.
```
<Root game directory>
└── winmm.dll/msvcrt.dll
└── nativePC
    └── plugins
        └── CSharp
            └── Loader
                └── Plugin1.dll
```
