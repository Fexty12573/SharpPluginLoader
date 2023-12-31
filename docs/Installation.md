# Installation
The mod is based on .NET 8. You will need to install the [.NET 8.0 Runtime](https://dotnet.microsoft.com/download/dotnet/8.0/runtime) to use it.
Grab the x64 Desktop Runtime and install it.

Once you have the .NET runtime installed you can grab the latest release from the [Releases Page](https://github.com/Fexty12573/SharpPluginLoader/releases).
Extract the contents of the archive into the game's root directory (where `MonsterHunterWorld.exe` is located).

If you installed everything correctly you should now find `winmm.dll` in the same folder as `MonsterHunterWorld.exe`, and a `CSharp` directory in `nativePC\plugins`.

## Usage
Any C# plugins will be placed directly into the `CSharp` directory. The plugin loader will automatically load all DLLs in this directory.
Subdirectories are also supported, so you can organize your plugins however you want.

!!! warning
    Putting plugins inside the `nativePC\plugins\CSharp\Loader` directory will **not** work. The plugin loader will not load them.
    See [below](#directory-structure-examples) for an example of a valid directory structure.

Once you have all your plugins installed you can simply start the game. The plugin loader will automatically load all plugins.
Depending on the plugins you have installed you might also see an overlay/UI appear on the screen.

## Directory Structure Examples
```
<Root game directory>
└── winmm.dll
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
└── winmm.dll
└── nativePC
    └── plugins
        └── CSharp
            └── Loader
                └── Plugin1.dll
```
