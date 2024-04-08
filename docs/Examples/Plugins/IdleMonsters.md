# Idle Monsters
This plugin locks all monsters into their idle state. This is done using the `OnMonsterAction` event, and changing the executed action to their idle action.

```cs
using SharpPluginLoader.Core;
using SharpPluginLoader.Core.Entities;

namespace IdleMonsters;

public class Plugin : IPlugin
{
    public string Name => "Idle Monsters";
    public string Author => "Fexty";

    public void OnMonsterAction(Monster monster, ref int actionId)
    {
        // Tigrex's idle action uses id 60, the other monsters use id 1
        if (monster.Type == MonsterType.Tigrex)
        {
            actionId = 60;
        }
        else
        {
            actionId = 1;
        }
    }
}
```

The below variation of the plugin lets the user toggle this idle state using the `/lock` and `/unlock` commands in the in-game chat.
```cs
using SharpPluginLoader.Core;
using SharpPluginLoader.Core.Entities;

namespace IdleMonsters;

public class Plugin : IPlugin
{
    public string Name => "Idle Monsters";
    public string Author => "Fexty";

    private bool _locked = false;

    private const string LOCK_COMMAND = "/lock";
    private const string UNLOCK_COMMAND = "/unlock";

    public void OnChatMessageSent(string message)
    {
        if (message == LOCK_COMMAND)
        {
            _locked = true;
        }
        else if (message == UNLOCK_COMMAND)
        {
            _locked = false;
        }
    }

    public void OnMonsterAction(Monster monster, ref int actionId)
    {
        if (!_locked)
        {
            return;
        }
        
        // Tigrex's idle action uses id 60, the other monsters use id 1
        if (monster.Type == MonsterType.Tigrex)
        {
            actionId = 60;
        }
        else
        {
            actionId = 1;
        }
    }
}
```
