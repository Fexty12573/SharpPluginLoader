# Direct Memory Access

The framework provides ways to directly mess with the game's memory, as if you were writing a C++ plugin.
There are also helper structs/classes to abstract away certain operations like pointer dereferencing.

## Reading/Writing Memory
To directly mess with the game's memory, you can use the `MemoryUtil` class.
It provides methods to read and write memory, as well as to allocate and free memory.

```csharp
// Read a single byte from the game's memory
byte value = MemoryUtil.Read<byte>(0x12345678);

// Get a reference to a value in the game's memory
ref int valueRef = ref MemoryUtil.GetRef<int>(0x12345678);
valueRef = 42;
```

Additionally, since C# doesn't allow pointers as type parameters, there are also methods to read a pointer from memory.

```csharp
unsafe
{
    int* pointer = MemoryUtil.ReadPointer<int>(0x12345678);
    int value = *pointer;
}
```

## `Span<T>` and `NativeArray<T>`
The framework provides a `NativeArray<T>` struct, which is a wrapper around a pointer to a native array, and a length.
It implements `IEnumerable<T>`, so you can perform LINQ operations on it.

```csharp
// Declare a native array of 10 integers that exists at the address 0x12345678
var myArray = new NativeArray<int>(0x12345678, 10);
foreach (int val in myArray)
{
    Log.Info(val);
}
```

Sometimes it's possible that you need to create your own array in native memory to pass to a method for example.
In that case, you can use the `NativeArray<T>.Create` method. `NativeArray<T>` also implements `IDisposable`, so you can use it in a `using` statement.
```csharp
using (var myArray = NativeArray<int>.Create(10))
{
    // Do something with the array
} // The array is freed here
```
!!! warning
    If you don't use `using`, you have to call `Dispose` manually, otherwise the array will not be freed.

If performance is critical, you can also use `Span<T>` to wrap existing arrays.
```csharp
// Declare a span over an array of 10 integers that exists at the address 0x12345678
var mySpan = new Span<int>(0x12345678, 10);
foreach (int val in mySpan)
{
    Log.Info(val);
}
```

## Patching Code
The framework provides a `Patch` class, which allows you to patch the game's code.

```csharp
// Create a patch at the address 0x12345678 to replace the instruction "mov al, 0" with "mov eax, 1"
var myPatch = new Patch(0x12345678, [ 0xB8, 0x01, 0x00, 0x00, 0x00 ]); // Using C# 12 collection expression syntax

// Apply the patch
myPatch.Enable();

// Disable the patch
myPatch.Disable();
```
For short term patching (for example if you need to patch a method for a single call), 
the `Patch` class implements `IDisposable`, so you can use it in a `using` statement.
```csharp
using (var myPatch = new Patch(0x12345678, [ 0xB8, 0x01, 0x00, 0x00, 0x00 ]))
{
    // Call a method or something
} // The patch is disabled here
```
The `Patch` class also allows initializing it with an asm string, which is useful for readability.
```csharp
var myPatch = new Patch(0x12345678, "mov eax, 1");

// Multiple instructions
var myPatch2 = new Patch(0x87654321, [
    "test al, al",
    "je +0x67",
    "nop"
]);
```

## Allocating Native Memory
If you need to allocate unmanaged memory, you can use the `MemoryUtil.Alloc` method.
```csharp
// Allocate 100 bytes of memory
nint memory = MemoryUtil.Alloc(100);

// Modify the memory
MemoryUtil.GetRef<int>(memory + 0x10) = 42;

// Free the memory
MemoryUtil.Free(memory); // Do not forget this!
```
!!! warning
    Allocating native memory is dangerous, as it can cause memory leaks if not freed properly.
    
## Utility
There are some utility methods in the `MemoryUtil` class that can be helpful in certain scenarios.

For example, sometimes you might have a native method that expects a pointer to an int as an argument.
Declaring a delegate with a `ref` parameter is not possible, neither is a `int*` parameter. Instead,
you can use the `MemoryUtil.AddressOf` method to get a pointer to a value in the game's memory.
```csharp
var myMethod = new NativeAction<nint>(0x12345678); // In this case 'nint' signifies a pointer
int myValue = 42;

myMethod.Invoke(MemoryUtil.AddressOf(ref myValue));
```

You can also convert between `T*` and `ref T` using the `MemoryUtil.AsRef` and `MemoryUtil.AsPointer` methods.
```csharp
// Get a reference to a value in the game's memory
ref int myRef = ref MemoryUtil.GetRef<int>(0x12345678);

// Convert the reference to a pointer
int* myPtr = MemoryUtil.AsPointer(ref myRef);

// Convert the pointer back to a reference
ref int myRef2 = ref MemoryUtil.AsRef(myPtr);
```
