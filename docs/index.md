---
_layout: landing
---

<div align="center">
    <h1>SharpPluginLoader</h1>
    <img src="images/SPL-256x256.png"></a>
</div>



## Introduction
SharpPluginLoader is, as the name might imply, a plugin loader for Monster Hunter World. It allows loading C# plugins into the game.

## Motivation
The main motivation behind this project is to allow for easier modding of the game. Stracker's Loader allows loading native plugins into the game.
However "plugin" is really just a nice term for "injecting a DLL". Plugins are nothing more than DLLs that modify the game's code/memory at runtime.

These DLLs are written in either C or C++. Neither of these languages are very beginner friendly. Furthermore, simply creating a DLL will not get you anywhere.
Plugins require many hours of reverse engineering the game's code to figure out how to modify it.

This project aims to simplify this process by allowing plugins to be written in C# instead, while also providing an API for interacting with the game.

## Feature Overview
- Load C# plugins into the game
    - Plugins are automatically reloaded when they are modified while the game is running
- API for interacting with the game
    - Wrappers around commonly used classes
    - Expansive list of in-game events that can be subscribed to (e.g. `OnUpdate`, `OnQuestStart`, `OnMonsterAction`, ...)
    - Raw access to the game's memory
    - Wrappers around native functions to allow calling them from C#
    - Hooking into native functions
- Plugin configuration
    - Plugins can have their own configuration files
    - Configuration files can be reloaded in-game
- Overlay Rendering
    - Plugins can render their own overlays/UIs using ImGUI
        - Both DX11 and DX12 are supported
