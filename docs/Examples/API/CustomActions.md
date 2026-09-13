# Custom Actions

The framework provides a way to register custom actions for any monster. (Note: an action is not the same as an animation. Actions *call* animations.)

To create a custom action, an appropriate class must be created first:

```cs
using SharpPluginLoader.Core;
using SharpPluginLoader.Core.Actions;

[CustomAction("MY_ACTION", BaseActionId = 123)]
public class MyAction : CustomAction
{
    public MyAction() { }

    public override void OnInitialize()
    {
        Log.Info("Created instance of MY_ACTION");
        ParentOnInitialize();
    }

    public override void OnExecute()
    {
        Log.Info("Executing MY_ACTION");
        ParentOnExecute();
    }

    public override bool OnUpdate()
    {
        Log.Debug("Updating MY_ACTION");
        return ParentOnUpdate();
    }

    public override void OnEnd()
    {
        Log.Info("Finishing MY_ACTION");
        ParentOnEnd();
    }
}
```

Few things to digest here.

## Base Requirements
Every custom action must do 3 things to be eligible for registration.

### The `CustomAction` Attribute

```cs
[CustomAction("MY_ACTION", BaseActionId = 123)]
```

Custom actions must be annotated with the `CustomAction` attribute. This attribute
lets you define

1. The name of your action
2. The ID or DTI the action is based on
3. Flags for your action

### Inherit from `CustomAction`

```cs
public class MyAction : CustomAction
```

To get access to the appropriate methods, each custom action must inherit from the `CustomAction` class. It provides the following overridable methods:

1. `OnInitialize`: Called right after the action object is created
2. `OnExecute`: Called every time the action is started
3. `OnUpdate`: Called once per frame while the action is running. Can return `false` to stop it early.
4. `OnEnd`: Called when the action is done

You do not *have* to implement all of them. You can only implement what you need. Most actions will only need `OnExecute` and `OnUpdate`.

### Public parameterless constructor

```cs
public MyAction() { }
```

This is a requirement from the `NativeWrapper` base class. Your action can't be registered if it doesn't have a `public` parameterless constructor.

## Registration

Once the action is declared, it can be registered to the `ActionCloner`.

```cs
using SharpPluginLoader.Core.Actions;

// ...

ActionCloner.RegisterAction<MyAction>(MonsterType.Rathian, 0);
```

This registers the action for the given monster. If you wanted to register your action for an Anjanath subspecies with sub-id 50, you would do the following:

```cs
ActionCloner.RegisterAction<MyAnjaAction>(MonsterType.Anjanath, 50);
```

This function also takes an optional `virtualFunctions` parameter, which allows you to replace any virtual function of the base action as you see fit. You will generally not need this unless you're doing some very in-depth logic changes.
