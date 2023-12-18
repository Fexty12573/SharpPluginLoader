# IConfig

Namespace: SharpPluginLoader.Core.Configuration

Represents a configuration file.

```csharp
public interface IConfig
```

**Remarks:**

All classes that implement this interface must have a public parameterless constructor (or a default constructor).

## Properties

### **Name**

The name of the configuration file.

```csharp
public abstract string Name { get; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>

### **Version**

The version of the configuration file.

```csharp
public abstract string Version { get; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>
