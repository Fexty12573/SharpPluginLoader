# Function Hooking

## Introduction
A hook (also sometimes known as a detour) is a way to intercept a function call to inject custom logic.
The framework provides a way for plugins to hook into native functions, allowing them to modify the game's behavior in various ways.

If you want to learn more about the specifics of function hooking, in the context of MHW, check out [this page](https://github.com/Ezekial711/MonsterHunterWorldModding/wiki/Function-Hooking).

## Hooking a function
Let's say you want to be notified each time a file is unloaded from the game. To do this you could hook the `sMhResource::releaseResource` function which,
in game version 15.21, is located at address `0x142224890`.

Unfortunately, due to the way C# generics work, there is a bit of boilerplate code required to hook a function. This is because the framework needs to know the function's signature at compile time.
```csharp linenums="1"
public class Example : IPlugin
{
    private delegate void ReleaseResourceDelegate(nint resourceManager, nint resourcePtr);
    private Hook<ReleaseResourceDelegate> _releaseResourceHook;

    public void OnLoad()
    {
        _releaseResourceHook = Hook.Create<ReleaseResourceDelegate>(0x142224890, ReleaseResourceDetour);
    }

    private void ReleaseResourceHook(nint resourceManager, nint resourcePtr)
    {
        var resource = new Resource(resourcePtr);
        if (resource.RefCount == 1)
            Log.Info($"Resource {resource.Name} was unloaded!");

        _releaseResourceHook.Original(resourceManager, resourcePtr);
    }
}
```
Let's break this down. First, we declare a delegate type with the same signature as the function we want to hook. This is required for the hook to work.
```csharp
private delegate void ReleaseResourceDelegate(nint resourceManager, nint resourcePtr);
```
> [!NOTE]
> Type marshalling is very limited for function hooks. Generally the only type that is automatically marshalled is `string`. You can also
> add the `UnmanagedFunctionPointer` attribute to the delegate to specify the character set, but this is not required.

Next we declare a hook object. This object needs to be kept alive by the plugin, otherwise the hook will be removed. It also allows dynamically enabling and disabling the hook.
```csharp
private Hook<ReleaseResourceDelegate> _releaseResourceHook;
```

In the `OnLoad` method we actually create the hook. The first parameter is the address of the function we want to hook, and the second is the method that will be called when the function is called.
```csharp
_releaseResourceHook = Hook.Create<ReleaseResourceDelegate>(0x142224890, ReleaseResourceDetour);
```

Finally, we implement the detour method. This method will be called instead of the original function. The first thing we do is create a `Resource` object from the pointer passed to the function. `Resource` is a wrapper around the `cResource` class, and has a few exposed properties such as the `RefCount`. We can use this to check if the resource is being unloaded.
The `releaseResource` method decrements the resources reference count by 1, so if the reference count is 1, it means the resource is being unloaded.

Lastly we call the original function to let the game perform its normal behavior. This is done by calling the `Original` property of the hook object, which is a delegate with the same signature as the original function.
```csharp
private void ReleaseResourceHook(nint resourceManager, nint resourcePtr)
{
    var resource = new Resource(resourcePtr);
    if (resource.RefCount == 1)
        Log.Info($"Resource {resource.Name} was unloaded!");

    _releaseResourceHook.Original(resourceManager, resourcePtr);
}
```
> [!WARNING]
> Once you create a `Resource` object around a `cResource` pointer, it will automatically increment the resources reference count by 1. Once the `Resource` object collected by the garbage collector, it will decrement the reference count by 1 again. If the reference count reaches 0, the resource will be unloaded. 
> 
> This means that so long as you hold a reference to a `Resource` object, the resource will never be unloaded (unless the game force-unloads it for some reason, but that should generally not be the case).

Of course, you can also use a lambda expression instead of a separate method for the detour. This makes the code a bit more compact for simple hooks.
```csharp
_releaseResourceHook = Hook.Create<ReleaseResourceDelegate>(0x142224890, (resourceManager, resourcePtr) =>
{
    var resource = new Resource(resourcePtr);
    if (resource.RefCount == 1)
        Log.Info($"Resource {resource.Name} was unloaded!");

    _releaseResourceHook.Original(resourceManager, resourcePtr);
});
```

## Extended Marshalling
As mentioned before, the framework only automatically marshalls `string` types. If you want to hook a function that takes other types as parameters, you need to manually marshal them.
However there is an experimental feature that allows automatic marshalling of any `NativeWrapper` types in addition to `string`.
Using this feature the above example can be rewritten as follows:
```csharp hl_lines="2 8 9 17"
using SharpPluginLoader.Core;
using SharpPluginLoader.Experimental;

namespace Example;

public class Plugin : IPlugin
{
    private delegate void ReleaseResourceDelegate(MtObject resourceManager, Resource resource);
    private MarshallingHook<ReleaseResourceDelegate> _releaseResourceHook;

    public void OnLoad()
    {
        // Using a lambda expression for the detour
        _releaseResourceHook = MarshallingHook.Create<ReleaseResourceDelegate>(0x142224890, (resourceManager, resource) => 
        {
            if (resource.RefCount == 1)
                Log.Info($"Resource {resource.Name} was unloaded!");

            _releaseResourceHook.Original(resourceManager, resource);
        });
    }
}
```

## Credits
The framework uses `Reloaded.Hooks` under the hood for function hooking. Check it out [here](https://github.com/Reloaded-Project/Reloaded.Hooks).
