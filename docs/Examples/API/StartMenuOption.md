# Custom Start Menu Entry
Using the `SharpPluginLoader.Core.UI.StartMenu` API, new entries can be added to the start menu. For example, to add a new option to the "Quest" page:

```cs
using SharpPluginLoader.Core;
using SharpPluginLoader.Core.UI;

// Inside Plugin Class
public void OnLoad()
{
    StartMenu.AddOption(new StartMenuOption
    {
        Page = StartMenuPage.Quest,
        AddTo = StartMenuType.Both,
        GetName = lang => lang switch
        {
            Language.English => "My Option",
            Language.German => "Meine Option",
            Language.French => "Mon Option",
            _ => "My Option"
        },
        GetDesc = _ => "This is my custom option",
        IsClickable = () => Area.CurrentStage == Stage.SelianaHub,
        OnClick = _ =>
        {
            Gui.DisplayPopup("Something Happened");
            StartMenu.Close();
        }
    });
}
```

A few things here:
```cs
GetName = lang => lang switch
{
    Language.English => "My Option",
    Language.German => "Meine Option",
    Language.French => "Mon Option",
    _ => "My Option"
},
```
`GetName`, `GetDesc` and `GetDisabledDesc` are all functions which take a `Language` parameter and should return the corresponding info for the option for the given language. You may also ignore the language argument entirely and simply return a string in one language (as seen with `GetDesc` in the example).

Next, the `IsClickable` property:
```cs
IsClickable = () => Area.CurrentStage == Stage.SelianaHub,
```
makes it so this entry can only be clicked when the player is in the Seliana Gathering Hub. `IsClickable` is of type `Action<bool>` and should return `true` if the entry should be clickable at the current moment in time. For example, you could gate this on a specific type of weapon being equipped.

Similarly, there also exists an `IsVisible` property which, like `IsClickable`, should return `true` if the option should be visible at the current moment in time.

Next, you can add your option to either the in-quest menu, the hub menu, or both.
```cs
AddTo = StartMenuType.Both,
```
This example adds the entry to both.

Finally, the `OnClick` property is called whenever the user actually clicks your entry.
```cs
OnClick = _ =>
{
    Gui.DisplayPopup("Something Happened");
    StartMenu.Close();
}
```
In here you can do whatever you want. If you want to close the start menu once your option is clicked you can use `StartMenu.Close()`, but this is optional.
