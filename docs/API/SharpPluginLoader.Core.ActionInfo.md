# ActionInfo

Namespace: SharpPluginLoader.Core

Represents an action that can be performed by an entity.
 This is a combination of an action set and an action id.

```csharp
public struct ActionInfo
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/System.ValueType) → [ActionInfo](./SharpPluginLoader.Core.ActionInfo.md)

## Fields

### **ActionSet**

The action set that this action belongs to.

```csharp
public int ActionSet;
```

### **ActionId**

The id of the action within the action set.

```csharp
public int ActionId;
```

## Constructors

### **ActionInfo(Int32, Int32)**

```csharp
ActionInfo(int actionSet, int actionId)
```

#### Parameters

`actionSet` [Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32)<br>

`actionId` [Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32)<br>

## Methods

### **Equals(ActionInfo)**

```csharp
bool Equals(ActionInfo other)
```

#### Parameters

`other` [ActionInfo](./SharpPluginLoader.Core.ActionInfo.md)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean)<br>

### **Equals(Object)**

```csharp
bool Equals(object obj)
```

#### Parameters

`obj` [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean)<br>

### **GetHashCode()**

```csharp
int GetHashCode()
```

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32)<br>

### **ToString()**

```csharp
string ToString()
```

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/System.String)<br>
