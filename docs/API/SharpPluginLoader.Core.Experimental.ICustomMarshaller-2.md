# ICustomMarshaller&lt;TManaged, TNative&gt;

Namespace: SharpPluginLoader.Core.Experimental

A custom marshaller for a pair of managed and native types

```csharp
public interface ICustomMarshaller<TManaged, TNative>
```

#### Type Parameters

`TManaged`<br>
The managed type

`TNative`<br>
The native type

## Methods

### **NativeToManaged(TNative)**

Marshals a native type to a managed type

```csharp
TManaged NativeToManaged(TNative native)
```

#### Parameters

`native` TNative<br>
The native input value

#### Returns

TManaged<br>
The marshalled managed value

### **ManagedToNative(TManaged)**

Marshals a managed type to a native type

```csharp
TNative ManagedToNative(TManaged managed)
```

#### Parameters

`managed` TManaged<br>
The managed input value

#### Returns

TNative<br>
The marshalled native value
