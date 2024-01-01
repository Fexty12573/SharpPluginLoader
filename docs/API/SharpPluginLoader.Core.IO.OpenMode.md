# OpenMode

Namespace: SharpPluginLoader.Core.IO

Specifies how to open a file.

```csharp
public enum OpenMode
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/System.ValueType) → [Enum](https://docs.microsoft.com/en-us/dotnet/api/System.Enum) → [OpenMode](./SharpPluginLoader.Core.IO.OpenMode.md)<br>
Implements [IComparable](https://docs.microsoft.com/en-us/dotnet/api/System.IComparable), [ISpanFormattable](https://docs.microsoft.com/en-us/dotnet/api/System.ISpanFormattable), [IFormattable](https://docs.microsoft.com/en-us/dotnet/api/System.IFormattable), [IConvertible](https://docs.microsoft.com/en-us/dotnet/api/System.IConvertible)

## Fields

| Name | Value | Description |
| --- | --: | --- |
| None | 0 | Do not open the file. |
| Read | 2 | Open the file for reading. If the file does not exist, an exception will be thrown. |
| Write | 3 | Open the file for writing. If the file exists, it will be truncated. If it does not exist, it will be created. |
| WriteAppend | 4 | Open the file for writing. If the file exists, it will be appended to. If it does not exist, it will be created. |
| ReadWrite | 5 | Open the file for reading and writing. If the file exists, it will be truncated. If it does not exist, it will be created. |
| ReadWriteAppend | 6 | Open the file for reading and writing. If the file exists, it will be appended to. If it does not exist, it will be created. |
