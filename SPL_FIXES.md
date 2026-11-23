# SPL Local Fixes

This directory contains minimal local fixes for two SharpPluginLoader issues.

## 1. Crash after changing the game language and restarting

File: `SharpPluginLoader.Core/UI/StartMenu.cs`

Cause:

- `LanguageIdentifiers` is built by iterating every `Language` enum value and calling `GetLanguageIdentifierFromId`.
- Some languages can return an empty string or null.
- After changing the language and restarting, `Gui.CurrentLanguage` may be a value outside the enum, so `LanguageIdentifiers[language]` throws `KeyNotFoundException`.
- Even when the lookup succeeds with null, the resource path becomes `common/text/cm_start_menu_`, `GetResource` returns null, and `Ensure.NotNull` throws.

Fix:

- Fall back to `"English"` when the language identifier is null.
- Use `TryGetValue` in `ApplyOptions()` and fall back to `Language.English` when the key is missing or empty.

## 2. Game chat key reopens SPL after closing the menu

File: `SharpPluginLoader.Core/Rendering/Renderer.cs`

Cause:

- The menu is drawn with `ImGui.Begin("SharpPluginLoader", ref _showMenu, ...)`.
- Closing it with the window X does not go through `_waitForRelease`.
- On the next frame `io.WantCaptureKeyboard` / `io.NavActive` can still be true, so the keyboard state is cleared and conflicts with the in-game chat key.

Fix:

- Remember whether the menu was shown at the start of the frame.
- When `wasMenuShown && !_showMenu` is detected:
  - set `_lastUpdateHadKeyboard = false`;
  - set `_waitForRelease = Key.Escape` so key handling resumes after Escape is released.

## Build

Requirements: Visual Studio 2022/2026, .NET 8 SDK, vcpkg (native dependencies).

```powershell
dotnet build .\SharpPluginLoader.Core\SharpPluginLoader.Core.csproj -c Release
```

Full loader:

```powershell
msbuild .\mhw-cs-plugin-loader.slnx /p:Configuration=Release /p:Platform=x64
```

Building the C# core alone is enough to verify these changes. The native `mhw-cs-plugin-loader` requires the vcpkg dependencies to be installed first.

## Notes

- These are local, temporary fixes, not an official release.
- The language fix only prevents the crash; it does not guarantee that every language has a matching `cm_start_menu_*` resource.
- The chat-key fix covers the window-X close path. If the menu can be closed by other means, `_lastUpdateHadKeyboard` should be reset there as well.
