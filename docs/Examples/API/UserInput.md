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

## Input Blocking
If you want to handle input but also keep it from reaching the game, you can use `Input.Block()` functions.

To ensure consistency, input blocking has to be done from `OnPadUpdate()`, `OnMouseUpdate()`, and `OnKeyboardUpdate()` respectively.

```csharp
public void OnPadUpdate()
{
    if (!Input.PadCaptured)
    {
        if (wantJoystick)
        {
            int PadRx = Input.GetPadRx(), PadRy = Input.GetPadRy();
            const int stickDeadzone = 4750;
            if (Math.Abs(PadRx) < stickDeadzone) PadRx = 0;
            if (Math.Abs(PadRy) < stickDeadzone) PadRy = 0;
            // Process right stick.

            Input.BlockPadRx();
            Input.BlockPadRy();
        }

        if (wantPad)
        {
            if (Input.IsPressed(Button.Cross))
            {
                // Process Cross/A.
            }

            if (Input.GetPadLz() > 200)
            {
                // Process left trigger mostly down.
            }

            if (Input.IsDown(Button.R3) && Input.IsPressed(Button.Up))
            {
                // Process R3/RStick + D-Pad Up.
            }

            Input.BlockPad();
        }
    }
}

public void OnMouseUpdate()
{
    if (wantMouse && !Input.MouseCaptured)
    {
        if (Input.IsPressed(Mouse.Left))
        {
            // Process left click.
        }

        if (Input.IsPressed(Mouse.Right))
        {
            // Process right click.
        }

        Input.BlockMouseClicks();
    }
}

public void OnKeyboardUpdate()
{
    if (wantKeyboard && !Input.KeyboardCaptured)
    {
        if (Input.IsPressed(Key.Up))
        {
            // Process up.
        }

        if (Input.IsPressed(Key.Down))
        {
            // Process down.
        }

        Input.Block(Key.Up);
        Input.Block(Key.Down);
    }
}
```
