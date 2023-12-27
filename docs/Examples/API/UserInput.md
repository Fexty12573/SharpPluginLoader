# User Input
To get user input, you can use the `Input` class. It has methods for both controller and keyboard input.

```csharp
// Check if the user is pressing the "Circle" button
if (Input.IsDown(Button.Circle))
    Log.Info("Circle is pressed!");

// Check if the user pressed the "T" in the last frame
if (Input.IsPressed(Key.T))
    Log.Info("T was pressed!");

// Key combinations
if (Input.IsDown(Key.LeftControl) && Input.IsPressed(Key.R))
    Log.Info("Ctrl+R was pressed!");
```

## Hotkeys
The framework also provides a `KeyBindings` class, which allows you to register hotkeys whose states can be queried by name.

```csharp
// Register a hotkey for Ctrl+Shift+L
KeyBindings.AddKeybind("MyHotkey", Key.L, [ Key.LeftControl, Key.LeftShift ]);

// Query the hotkey's state
if (KeyBindings.IsPressed("MyHotkey"))
    Log.Info("My hotkey was pressed!");
```