# Rendering

Besides the ImGui rendering, the framework also supports arbitrary rendering using DirectX. This is done by subscribing to the `OnRender` event. This event is called right before the ImGui rendering.

```csharp
public void OnRender()
{
    // Render your stuff here
}
```

Technically it's possible to directly use something like [`Vortice.Windows`](https://github.com/amerkoleci/Vortice.Windows) or [`Silk.NET`](https://github.com/dotnet/Silk.NET) to render things, but this is not (yet) officially supported.

Currently the framework only provides a couple functions for rendering some 3D shapes. These functions are located in the `SharpPluginLoader.Core.Rendering.Primitives` class.
    
```csharp
using SharpPluginLoader.Core.Rendering;

...

public void OnRender()
{
    var player = Player.MainPlayer;
    if (player is null)
        return;

    // Render a sphere above the player
    Primitives.RenderSphere(player.Position, 25.0f, Color.Red);
}
```
