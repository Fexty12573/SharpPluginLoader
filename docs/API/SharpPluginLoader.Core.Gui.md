# Gui

Namespace: SharpPluginLoader.Core

```csharp
public static class Gui
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [Gui](./SharpPluginLoader.Core.Gui.md)

## Properties

### **SingletonInstance**

The singleton instance of the sMhGUI class.

```csharp
public static nint SingletonInstance { get; }
```

#### Property Value

[IntPtr](https://docs.microsoft.com/en-us/dotnet/api/System.IntPtr)<br>

## Methods

### **DisplayPopup(String, Nullable&lt;TimeSpan&gt;, Nullable&lt;TimeSpan&gt;, Single, Single)**

Displays a popup message on the screen.

```csharp
public static void DisplayPopup(string message, Nullable<TimeSpan> duration, Nullable<TimeSpan> delay, float xOff, float yOff)
```

#### Parameters

`message` [String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>
The message to display

`duration` [Nullable&lt;TimeSpan&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Nullable-1)<br>
The duration for which the popup should be displayed

`delay` [Nullable&lt;TimeSpan&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Nullable-1)<br>
The delay before the popup is displayed

`xOff` [Single](https://docs.microsoft.com/en-us/dotnet/api/System.Single)<br>
The horizontal offset from the center of the screen

`yOff` [Single](https://docs.microsoft.com/en-us/dotnet/api/System.Single)<br>
The vertical offset from the center of the screen

### **DisplayMessage(String, Nullable&lt;TimeSpan&gt;, Boolean)**

Displays a blue/purple message in the chat box.

```csharp
public static void DisplayMessage(string message, Nullable<TimeSpan> delay, bool isImportant)
```

#### Parameters

`message` [String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>
The message to display

`delay` [Nullable&lt;TimeSpan&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Nullable-1)<br>
The delay before the message is displayed

`isImportant` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean)<br>
Whether the message should be displayed in purple

### **DisplayYesNoDialog(String, DialogCallback)**

Displays a prompt with a yes/no/cancel button. The callback will be called when the user clicks a button.
 Note: The user is responsible for keeping the callback alive.

```csharp
public static void DisplayYesNoDialog(string message, DialogCallback callback)
```

#### Parameters

`message` [String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>
The message to display

`callback` [DialogCallback](./SharpPluginLoader.Core.DialogCallback.md)<br>
The callback to call when the user clicks a button

### **DisplayMessageWindow(String, MtVector2)**

```csharp
public static void DisplayMessageWindow(string message, MtVector2 offset)
```

#### Parameters

`message` [String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>

`offset` [MtVector2](./SharpPluginLoader.Core.MtTypes.MtVector2.md)<br>

### **DisplayAlert(String)**

```csharp
public static void DisplayAlert(string message)
```

#### Parameters

`message` [String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>

### **Initialize()**

```csharp
internal static void Initialize()
```
