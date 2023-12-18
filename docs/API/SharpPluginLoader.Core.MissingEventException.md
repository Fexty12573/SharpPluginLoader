# MissingEventException

Namespace: SharpPluginLoader.Core

```csharp
public class MissingEventException : System.Exception, System.Runtime.Serialization.ISerializable
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [Exception](https://docs.microsoft.com/en-us/dotnet/api/System.Exception) → [MissingEventException](./SharpPluginLoader.Core.MissingEventException.md)<br>
Implements [ISerializable](https://docs.microsoft.com/en-us/dotnet/api/System.Runtime.Serialization.ISerializable)

## Properties

### **TargetSite**

```csharp
public MethodBase TargetSite { get; }
```

#### Property Value

[MethodBase](https://docs.microsoft.com/en-us/dotnet/api/System.Reflection.MethodBase)<br>

### **Message**

```csharp
public string Message { get; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>

### **Data**

```csharp
public IDictionary Data { get; }
```

#### Property Value

[IDictionary](https://docs.microsoft.com/en-us/dotnet/api/System.Collections.IDictionary)<br>

### **InnerException**

```csharp
public Exception InnerException { get; }
```

#### Property Value

[Exception](https://docs.microsoft.com/en-us/dotnet/api/System.Exception)<br>

### **HelpLink**

```csharp
public string HelpLink { get; set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>

### **Source**

```csharp
public string Source { get; set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>

### **HResult**

```csharp
public int HResult { get; set; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32)<br>

### **StackTrace**

```csharp
public string StackTrace { get; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>

## Constructors

### **MissingEventException(String)**

```csharp
public MissingEventException(string eventName)
```

#### Parameters

`eventName` [String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>
