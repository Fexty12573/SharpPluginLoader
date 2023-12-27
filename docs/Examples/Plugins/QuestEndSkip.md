# Quest End Skip
This plugin skips the quest end timer with the hotkey `Ctrl+Alt+S`.

## Code
```cs
using SharpPluginLoader.Core;
using SharpPluginLoader.Core.IO;

namespace QuestEndSkip;

public class Plugin : IPlugin
{
    public string Name => "QuestEndSkip";
    public string Author => "Fexty";

    public PluginData OnLoad()
    {
        // Register the `Ctrl+Alt+S` keybind
        KeyBindings.AddKeybind("SkipQuestEnd", new Keybind<Key>(Key.S, [Key.LeftControl, Key.LeftAlt]));

        return new PluginData
        {
            // Subscribe to the `OnUpdate` event
            OnUpdate = true
        };
    }

    public void OnUpdate(float dt)
    {
        // Each frame, check if the quest end timer is running and the `Ctrl+Alt+S` keybind is pressed
        if (Quest.QuestEndTimer.Time > 0f && KeyBindings.IsPressed("SkipQuestEnd"))
        {
            // If so, set the quest end timer to be over instantly
            Quest.QuestEndTimer.SetToEnd();
        }
    }
}
```
