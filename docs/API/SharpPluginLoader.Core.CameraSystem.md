# CameraSystem

Namespace: SharpPluginLoader.Core

```csharp
public static class CameraSystem
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [CameraSystem](./SharpPluginLoader.Core.CameraSystem.md)

## Properties

### **SingletonInstance**

```csharp
public static nint SingletonInstance { get; }
```

#### Property Value

[IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

### **MainViewport**

Gets the main viewport.

```csharp
public static Viewport MainViewport { get; }
```

#### Property Value

[Viewport](./SharpPluginLoader.Core.View.Viewport.md)<br>

## Methods

### **GetViewport(Int32)**

Gets the viewport at the specified index.

```csharp
public static Viewport GetViewport(int index)
```

#### Parameters

`index` [Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32)<br>
The index of the viewport.

#### Returns

[Viewport](./SharpPluginLoader.Core.View.Viewport.md)<br>
The viewport at the specified index.
