# Input

Namespace: SharpPluginLoader.Core.IO

Provides a set of methods for checking the state of the controller and keyboard.

```csharp
public static class Input
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [Input](./SharpPluginLoader.Core.IO.Input.md)

## Methods

### **IsDown(Button)**

Checks if the specified button is currently pressed.

```csharp
public static bool IsDown(Button button)
```

#### Parameters

`button` [Button](./SharpPluginLoader.Core.IO.Button.md)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean)<br>

### **IsPressed(Button)**

Checks if the specified button was pressed in the last frame.

```csharp
public static bool IsPressed(Button button)
```

#### Parameters

`button` [Button](./SharpPluginLoader.Core.IO.Button.md)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean)<br>

### **IsReleased(Button)**

Checks if the specified button was released in the last frame.

```csharp
public static bool IsReleased(Button button)
```

#### Parameters

`button` [Button](./SharpPluginLoader.Core.IO.Button.md)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean)<br>

### **IsChanged(Button)**

Checks if the specified button was pressed or released in the last frame.

```csharp
public static bool IsChanged(Button button)
```

#### Parameters

`button` [Button](./SharpPluginLoader.Core.IO.Button.md)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean)<br>

### **IsDown(Key)**

Checks if the specified button is currently pressed.

```csharp
public static bool IsDown(Key key)
```

#### Parameters

`key` [Key](./SharpPluginLoader.Core.IO.Key.md)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean)<br>

### **IsPressed(Key)**

Checks if the specified button was pressed in the last frame.

```csharp
public static bool IsPressed(Key key)
```

#### Parameters

`key` [Key](./SharpPluginLoader.Core.IO.Key.md)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean)<br>

### **IsReleased(Key)**

Checks if the specified button was released in the last frame.

```csharp
public static bool IsReleased(Key key)
```

#### Parameters

`key` [Key](./SharpPluginLoader.Core.IO.Key.md)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean)<br>

### **IsChanged(Key)**

Checks if the specified button was pressed or released in the last frame.

```csharp
public static bool IsChanged(Key key)
```

#### Parameters

`key` [Key](./SharpPluginLoader.Core.IO.Key.md)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean)<br>
