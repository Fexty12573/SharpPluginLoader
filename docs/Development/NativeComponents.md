# Native Components
Generally, most plugin-related things should be achievable in C# with the functionality provided by the framework. But sometimes it is perhaps easier to implement in C/C++ than in C#. Or maybe there is a C++ library that you want to use from your plugin.

This is where Native Components come in. A native component is an unmanaged counterpart to a C# plugin. Usually when you need to interop with native code in C# you do so using [P/Invoke](https://learn.microsoft.com/en-us/dotnet/standard/native-interop/pinvoke). However the framework provides a more performant and versatile alternative to P/Invoke in the form of Internal Calls.

Every plugin `PluginName.dll` can have a native component `PluginName.Native.dll`, which is required to be a native dll. Native components need to export 2 functions:
```cpp
#include "SPL/InternalCall.h"

namespace SPLNative = SharpPluginLoader::Native;

SPL_INTERNAL_CALL int get_internal_call_count() { 
    // ... 
}

SPL_INTERNAL_CALL void collect_internal_calls(SPLNative::InternalCall* icalls) { 
    // ...
}
```

`get_internal_call_count` tells the framework how many internal calls are exported by the native component.

In the `collect_internal_calls` function the native component then fills out the `icalls` array.

For example, say you have some ImGui widget implemented in C++ that you want to expose to your plugin, your native component might look something like this:
```cpp
...
bool my_imgui_widget(const char* label, int* values, unsigned flags) { ... }

SPL_INTERNAL_CALL int get_internal_call_count() { return 1; }
SPL_INTERNAL_CALL void collect_internal_calls(SPLNative::InternalCall* icalls) {
    icalls[0] = { "MyImGuiWidget", &my_imgui_widget };
}
```

The important part here is this line:
```cpp
icalls[0] = { "MyImGuiWidget", &my_imgui_widget };
```

The first component here is the name of the internal call on the C# side, which must match exactly. The second component is the function pointer.

Now on the C# side you need to first add a reference to the `SharpPluginLoader.InternalCallGenerator` NuGet package to your plugin.

Next you need to designate one class in your plugin as the internal call manager. Typically you would create a class dedicated to holding all of the internal calls. Technically however, you can make any class (even your main plugin class) the internal call manager. What is important though is that there can only be *one* internal call manager per plugin.

```cs
using SharpPluginLoader.InternalCallGenerator;

[InternalCallManager]
public partial class InternalCalls
{
    [InternalCall]
    public static partial bool MyImGuiWidget(string label, Span<int> values, uint flags);
}
```

The internal call manager is marked using the `[InternalCallManager]` attribute. Each internal call is then marked with an `[InternalCall]` attribute. It is required that the internal call manager class is `partial`. The same goes for all internal call methods, which must be marked `static partial`.

This is all the code required on the managed side. Now you can call your native function by doing `InternalCalls.MyImGuiWidget("Test", [3, 4, 5]);`.

Of course you can also bind an internal call to a game function directly:
```cpp
icalls[2] = { "SomeGameFunction", (void*)0x1430ae620 };
```

As you can see the managed function takes a `string` and a `Span<int>` as parameters. This works because the InternalCallGenerator will generate marshalling code for these types behind the scenes. Below is a list of managed types and what they map to on the native end:

| Managed Type | Native Type | Copy Required | Remarks |
| ------------ | ----------- | ------------- | ------- |
| `string` | `char*` / `char8_t*` | Yes | UTF8 Encoding |
| `[WideString] string` | `wchar_t*` / `char16_t*` | Yes | UTF16 Encoding |
| `T[]` | `T*` | No | `T` must be unmanaged |
| `T` | `T` | Yes | ^ |
| `List<T>` | `T*` | No | ^ |
| `{ReadOnly}Span<T>` | `T*` | No | ^ |
| `{ReadOnly}Memory<T>` | `T*` | No | ^ |
| `ref/out T` | `T*` | No | ^ |
| `struct` | `struct` | Yes | The struct must be unmanaged (i.e. contain no reference types) |
| `ref/out struct` | `struct*` | No | ^ |
| `class` | Unsupported | | |

## Other Languages
It is also possible to write native components in languages other than C++ such as Rust. The only requirement is that the dll uses the [Microsoft x64 calling convention](https://learn.microsoft.com/en-us/cpp/build/x64-calling-convention?view=msvc-170) for the two exported functions. This is the default calling convention when compiling C/C++ with MSVC on x64.
