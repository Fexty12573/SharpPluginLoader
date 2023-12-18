# ImGuiExtensions

Namespace: SharpPluginLoader.Core.Rendering

```csharp
public static class ImGuiExtensions
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [ImGuiExtensions](./SharpPluginLoader.Core.Rendering.ImGuiExtensions.md)

## Methods

### **BeginTimeline(String, Single, Single, Single&, ImGuiTimelineFlags)**

```csharp
public static bool BeginTimeline(string label, float startFrame, float endFrame, Single& currentFrame, ImGuiTimelineFlags flags)
```

#### Parameters

`label` [String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>

`startFrame` [Single](https://docs.microsoft.com/en-us/dotnet/api/System.Single)<br>

`endFrame` [Single](https://docs.microsoft.com/en-us/dotnet/api/System.Single)<br>

`currentFrame` [Single&](https://docs.microsoft.com/en-us/dotnet/api/System.Single&)<br>

`flags` [ImGuiTimelineFlags](./SharpPluginLoader.Core.Rendering.ImGuiTimelineFlags.md)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean)<br>

### **EndTimeline()**

```csharp
public static void EndTimeline()
```

### **BeginTimelineGroup(String, Boolean&)**

```csharp
public static bool BeginTimelineGroup(string label, Boolean& expanded)
```

#### Parameters

`label` [String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>

`expanded` [Boolean&](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean&)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean)<br>

### **BeginTimelineGroup(String)**

```csharp
public static bool BeginTimelineGroup(string label)
```

#### Parameters

`label` [String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean)<br>

### **EndTimelineGroup()**

```csharp
public static void EndTimelineGroup()
```

### **TimelineTrack(String, Span&lt;Single&gt;, Int32&, Int32)**

```csharp
public static bool TimelineTrack(string label, Span<float> keyframes, Int32& selectedKeyframe, int explicitCount)
```

#### Parameters

`label` [String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>

`keyframes` [Span&lt;Single&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Span-1)<br>

`selectedKeyframe` [Int32&](https://docs.microsoft.com/en-us/dotnet/api/System.Int32&)<br>

`explicitCount` [Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean)<br>

### **TimelineTrack(String, Span&lt;Single&gt;, Int32)**

```csharp
public static bool TimelineTrack(string label, Span<float> keyframes, int explicitCount)
```

#### Parameters

`label` [String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>

`keyframes` [Span&lt;Single&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Span-1)<br>

`explicitCount` [Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean)<br>

### **NotificationSuccess(String, Int32)**

```csharp
public static void NotificationSuccess(string message, int duration)
```

#### Parameters

`message` [String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>

`duration` [Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32)<br>

### **NotificationError(String, Int32)**

```csharp
public static void NotificationError(string message, int duration)
```

#### Parameters

`message` [String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>

`duration` [Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32)<br>

### **NotificationWarning(String, Int32)**

```csharp
public static void NotificationWarning(string message, int duration)
```

#### Parameters

`message` [String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>

`duration` [Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32)<br>

### **NotificationInfo(String, Int32)**

```csharp
public static void NotificationInfo(string message, int duration)
```

#### Parameters

`message` [String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>

`duration` [Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32)<br>

### **Notification(ImGuiToastType, String, String, Int32)**

```csharp
public static void Notification(ImGuiToastType type, string title, string message, int duration)
```

#### Parameters

`type` [ImGuiToastType](./SharpPluginLoader.Core.Rendering.ImGuiToastType.md)<br>

`title` [String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>

`message` [String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>

`duration` [Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32)<br>

### **InputScalar(String, SByte&, SByte, SByte, String, ImGuiInputTextFlags)**

```csharp
public static bool InputScalar(string label, SByte& value, sbyte step, sbyte stepFast, string format, ImGuiInputTextFlags flags)
```

#### Parameters

`label` [String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>

`value` [SByte&](https://docs.microsoft.com/en-us/dotnet/api/System.SByte&)<br>

`step` [SByte](https://docs.microsoft.com/en-us/dotnet/api/System.SByte)<br>

`stepFast` [SByte](https://docs.microsoft.com/en-us/dotnet/api/System.SByte)<br>

`format` [String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>

`flags` ImGuiInputTextFlags<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean)<br>

### **InputScalar(String, Byte&, Byte, Byte, String, ImGuiInputTextFlags)**

```csharp
public static bool InputScalar(string label, Byte& value, byte step, byte stepFast, string format, ImGuiInputTextFlags flags)
```

#### Parameters

`label` [String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>

`value` [Byte&](https://docs.microsoft.com/en-us/dotnet/api/System.Byte&)<br>

`step` [Byte](https://docs.microsoft.com/en-us/dotnet/api/System.Byte)<br>

`stepFast` [Byte](https://docs.microsoft.com/en-us/dotnet/api/System.Byte)<br>

`format` [String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>

`flags` ImGuiInputTextFlags<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean)<br>

### **InputScalar(String, Int16&, Int16, Int16, String, ImGuiInputTextFlags)**

```csharp
public static bool InputScalar(string label, Int16& value, short step, short stepFast, string format, ImGuiInputTextFlags flags)
```

#### Parameters

`label` [String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>

`value` [Int16&](https://docs.microsoft.com/en-us/dotnet/api/System.Int16&)<br>

`step` [Int16](https://docs.microsoft.com/en-us/dotnet/api/System.Int16)<br>

`stepFast` [Int16](https://docs.microsoft.com/en-us/dotnet/api/System.Int16)<br>

`format` [String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>

`flags` ImGuiInputTextFlags<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean)<br>

### **InputScalar(String, UInt16&, UInt16, UInt16, String, ImGuiInputTextFlags)**

```csharp
public static bool InputScalar(string label, UInt16& value, ushort step, ushort stepFast, string format, ImGuiInputTextFlags flags)
```

#### Parameters

`label` [String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>

`value` [UInt16&](https://docs.microsoft.com/en-us/dotnet/api/System.UInt16&)<br>

`step` [UInt16](https://docs.microsoft.com/en-us/dotnet/api/System.UInt16)<br>

`stepFast` [UInt16](https://docs.microsoft.com/en-us/dotnet/api/System.UInt16)<br>

`format` [String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>

`flags` ImGuiInputTextFlags<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean)<br>

### **InputScalar(String, Int32&, Int32, Int32, String, ImGuiInputTextFlags)**

```csharp
public static bool InputScalar(string label, Int32& value, int step, int stepFast, string format, ImGuiInputTextFlags flags)
```

#### Parameters

`label` [String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>

`value` [Int32&](https://docs.microsoft.com/en-us/dotnet/api/System.Int32&)<br>

`step` [Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32)<br>

`stepFast` [Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32)<br>

`format` [String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>

`flags` ImGuiInputTextFlags<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean)<br>

### **InputScalar(String, UInt32&, UInt32, UInt32, String, ImGuiInputTextFlags)**

```csharp
public static bool InputScalar(string label, UInt32& value, uint step, uint stepFast, string format, ImGuiInputTextFlags flags)
```

#### Parameters

`label` [String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>

`value` [UInt32&](https://docs.microsoft.com/en-us/dotnet/api/System.UInt32&)<br>

`step` [UInt32](https://docs.microsoft.com/en-us/dotnet/api/System.UInt32)<br>

`stepFast` [UInt32](https://docs.microsoft.com/en-us/dotnet/api/System.UInt32)<br>

`format` [String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>

`flags` ImGuiInputTextFlags<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean)<br>

### **InputScalar(String, Int64&, Int64, Int64, String, ImGuiInputTextFlags)**

```csharp
public static bool InputScalar(string label, Int64& value, long step, long stepFast, string format, ImGuiInputTextFlags flags)
```

#### Parameters

`label` [String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>

`value` [Int64&](https://docs.microsoft.com/en-us/dotnet/api/System.Int64&)<br>

`step` [Int64](https://docs.microsoft.com/en-us/dotnet/api/System.Int64)<br>

`stepFast` [Int64](https://docs.microsoft.com/en-us/dotnet/api/System.Int64)<br>

`format` [String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>

`flags` ImGuiInputTextFlags<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean)<br>
