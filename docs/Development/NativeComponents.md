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

## Binding to Game Functions
It is also possible to bind an internal call to a game function directly. 

The naive way of doing this is to simply cast the function pointer to a `void*` and pass it to the internal call.
```cpp
icalls[0] = { "SomeGameFunction", (void*)0x1430ae620 };
```

However it is also possible to simplify this process by specifying the address directly in the C# code.
```cs
[InternalCall(Address = 0x1430ae620)]
public static partial void SomeGameFunction();
```

If you want to make this internal call update proof, you can use an AOB instead of a direct address.
```cs
[InternalCall(Pattern = "48 8B 05 ? ? ? ? 48 85 C0 74 0A", Offset = -8, Cache = true)]
public static partial void SomeGameFunction();
```

The `Pattern` field is a string containing the bytes of the pattern. The `?` character (`??` is also valid) is a wildcard. The `Offset` field is an integer that is added to the result of the pattern scan. The `Cache` field is a boolean that specifies whether the address of this function should be cached or not. Cached addresses are evaluated once and then stored on disk. If the game version changes, the cache is invalidated and the address is re-evaluated. 

It is generally recommended to always mark internal calls that use a pattern with `Cache = true` to avoid unnecessary pattern scans. If you have a lot of internal calls that use patterns, it can result in a significant performance improvement on startup (except for the first time the plugin is loaded after the game has been updated).
