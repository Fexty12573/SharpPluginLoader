using SharpPluginLoader.InternalCallGenerator;
using SharpPluginLoader.Core.MtTypes;
using SharpPluginLoader.Core;

namespace ExperimentalTesting;

[InternalCallManager]
public unsafe partial class InternalCalls
{
    // Test with simple primitive types
    [InternalCall]
    public static partial long Sum(long a, long b);

    // Test with single pointer type
    [InternalCall]
    public static partial void TestPointers(double* values);

    // Test with mixed unmanaged types
    [InternalCall]
    public static partial char ProcessData(byte* data, int length, ref ushort dataId);

    // Test with array and pointer types
    [InternalCall]
    public static partial void TransformCoordinates(float[] coords, MtVector3* transformMatrix);

    // Test with InternalCallOptions and ref parameter
    [InternalCall(InternalCallOptions.Unsafe)]
    public static partial void ModifyValue(ref int value);

    // Test with no return value and primitive parameters
    [InternalCall]
    public static partial void LogMessage(int level, char* message);

    // Test with complex struct return type and primitive parameters
    [InternalCall]
    public static partial MtMatrix4X4 ComputeStruct(int param1, float param2);

    // Test with pointer to struct and primitive types
    [InternalCall]
    public static partial MtVector2* CreateVector(int x, int y);

    // Test with multiple ref and out parameters
    [InternalCall]
    public static partial void TestRefOutParameters(ref float a, out double b, ref MtVector2 vec);

    // Test with mixed array, pointer, and primitive types
    [InternalCall]
    public static partial void ProcessBuffers(byte[] inputBuffer, float* outputBuffer, int bufferSize);
}
