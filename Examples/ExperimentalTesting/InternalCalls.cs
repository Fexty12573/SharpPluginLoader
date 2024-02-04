using SharpPluginLoader.InternalCallGenerator;
using SharpPluginLoader.Core.MtTypes;
using SharpPluginLoader.Core;

namespace ExperimentalTesting;

public struct HasRefTypes
{
    public string str;
    public MtVector2 vec;
}

[InternalCallManager]
public partial class InternalCalls
{
    // Test with simple primitive types
    [InternalCall(InternalCallOptions.Unsafe)]
    public static partial long Sum(long a, long b);

    // Test with single pointer type
    [InternalCall]
    public static unsafe partial void TestPointers(double* values);

    // Test with mixed unmanaged types
    [InternalCall]
    public static unsafe partial char ProcessData(byte* data, int length, ref ushort dataId);

    // Test with array and pointer types
    [InternalCall]
    public static unsafe partial void TransformCoordinates(float[] coords, MtVector3* transformMatrix);

    // Test with InternalCallOptions and ref parameter
    [InternalCall(InternalCallOptions.Unsafe)]
    public static partial void ModifyValue(ref int value);

    // Test with no return value and primitive parameters
    [InternalCall]
    public static unsafe partial void LogMessage(int level, [WideString] string message);

    // Test with complex struct return type and primitive parameters
    [InternalCall]
    public static partial MtMatrix4X4 ComputeStruct(int param1, float param2);

    // Test with pointer to struct and primitive types
    [InternalCall]
    public static unsafe partial MtVector2* CreateVector(int x, int y);

    // Test with multiple ref and out parameters
    [InternalCall]
    public static partial void TestRefOutParameters(ref float a, out double b, ref MtVector2 vec);

    // Test with mixed array, pointer, and primitive types
    [InternalCall]
    public static unsafe partial void ProcessBuffers(byte[] inputBuffer, float* outputBuffer, int bufferSize);

    // Test with strings
    [InternalCall]
    public static partial MtVector2 Parse(string str);

    [InternalCall]
    [return: WideString]
    public static partial string Format(MtVector2 vec, [WideString] string str);

    [InternalCall]
    public static partial void EnumTest(PropType type);

    [InternalCall]
    public static partial void StructTest(HasRefTypes type);

    [InternalCall]
    public static partial void SpanTest(Span<byte> span);

    [InternalCall]
    public static partial void ReadOnlySpanTest(ReadOnlySpan<byte> span);

    [InternalCall]
    public static partial void MemoryTest(Memory<byte> memory);
    
    [InternalCall]
    public static partial void ReadOnlyMemoryTest(ReadOnlyMemory<byte> memory);

    [InternalCall]
    public static partial void ListTest(List<byte> list);

    [InternalCall(Pattern = "AB CD EF 90")]
    public static partial void PatternTest();

    [InternalCall(Pattern = "AB CD EF 90", Offset = -23)]
    public static partial void PatternWithOffsetTest();

    [InternalCall(Pattern = "AB CD EF 90", Cache = true)]
    public static partial void PatternWithCacheTest();

    [InternalCall(Pattern = "AB CD EF 90", Offset = -23, Cache = true)]
    public static partial void PatternWithOffsetAndCacheTest();

    [InternalCall(Address = 0x1234567890)]
    public static partial void AddressTest();

    [InternalCall(Address = 0x1234567890, Pattern = "AB CD EF 90")]
    public static partial void AddressAndPatternTest();

    [InternalCall(Address = 0x1234567890, Pattern = "AB CD EF 90", Offset = -23)]
    public static partial void AddressPatternAndOffsetTest();

    [InternalCall(InternalCallOptions.Unsafe, Address = 0x1234567890)]
    public static partial void UnsafeAddressTest();
}
