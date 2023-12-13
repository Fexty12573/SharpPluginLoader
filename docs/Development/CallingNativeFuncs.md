# Calling Native Functions

## Introduction
The framework already provides access to a lot of game functions, but sometimes something may not be available. 
In these cases you can call native functions directly from your plugin.

!!! warning
    Calling native functions can lead to crashes very quickly if done incorrectly.
    Make sure you know what you're doing and test your code thoroughly.

If you're already familiar with calling function pointers in C++, you can just read the API Reference for the [`NativeFunction`]() class and the [`NativeAction`]() class.

Natively, C# provides a couple ways to call native functions. The most common method is a P/Invoke.
Traditionally, you would declare a `static extern` method with the `DllImport` attribute, and then call it like any other method.
```csharp
[DllImport("kernel32.dll")]
private static extern nint LoadLibrary(string dllToLoad);
```
.NET 8 introduced the new `LibraryImport` attribute which allows more marshalling behavior using source generators.

However both of these require the function to be exported from a DLL. MHW does not export any game functions, so you can only refer to the function by its address.

The other next-best option is to use the `Marshal.GetDelegateForFunctionPointer` method. This method allows you to create a delegate from a function pointer, which you can then call like any other method.
```csharp
private delegate void MyDelegate(int arg1, float arg2);

private void MyMethod()
{
    var myDelegate = Marshal.GetDelegateForFunctionPointer<MyDelegate>(0x140000000);
    myDelegate(1, 2.0f);
}
```

This method is a bit more complicated than the previous two, but it allows you to call any function, regardless of whether it's exported or not.
It creates a run-time wrapper around the function pointer, which you can then call like any other method.

## Function Pointers
C# 9.0 introduced a new feature called function pointers. These are similar to delegates, but more low-level and with less overhead. The drawback is that
there is almost no marshalling done automatically, so you have to do it yourself. The only type that is automatically marshalled is `string`.
```csharp
delegate* unmanaged<int, float, void> myDelegate = (delegate* unmanaged<int, float, void>)0x140000000;
myDelegate(1, 2.0f);
```

This is almost identical to how you call a function pointer in C++. The `delegate* unmanaged` syntax is a bit weird, but it's just a pointer to a function that takes two parameters and returns void.
The syntax is still a bit clunky tho, so the framework provides wrappers around function pointers to make them easier to use. These wrappers are called `NativeFunction` and `NativeAction`.

If you are familiar with C#, these are essentially just `Func` and `Action` but for native functions.
```csharp
var myFunction = new NativeAction<int, float>(0x140000000);
myFunction.Invoke(1, 2.0f);
```

!!! info
    The framework also provides a `NativeFunction` class, which is identical to `NativeAction` except it returns a value. If your function doesn't return a value, you can use `NativeAction`,
    otherwise you need to use `NativeFunction`.
    ```csharp
    var myFunction = new NativeFunction<int, float, int>(0x140000000);
    int result = myFunction.Invoke(1, 2.0f);
    ```

You call the native function using the `Invoke` property of the object. This is a bit different from how you would call a normal delegate, but unfortunately, C# doesn't allow overloading the `()` operator so this is the best we can do.

## The GC Transition
I mentioned that function pointers do not have a lot of overhead. This is true, however there still remains a bit of overhead. When invoking a native function, the runtime needs to transition the GC from cooperative to preemptive mode, and back again after the function returns.

For some very short native functions however it can be beneficial to skip this transition entirely, and instead call the function directly without any overhead. This is done using the `InvokeUnsafe` property.
```csharp
var myFunction = new NativeAction<int, float>(0x140000000);
myFunction.InvokeUnsafe(1, 2.0f);
```

!!! warning
    `InvokeUnsafe` is unsafe for a reason. When you're calling a function using this there are some things you need to pay attention to. 
    
    Due to the fact that the GC does not get transitioned, the native function you are calling is not allowed to call back into managed code. So if the function you are calling happens to be hooked by another C# plugin, your game will crash immediately.

    It also should not run for more than 1 microsecond. You can see a full list of constraints for this [here](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute?view=net-8.0).
