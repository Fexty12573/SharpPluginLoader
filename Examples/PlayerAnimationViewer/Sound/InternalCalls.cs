using SharpPluginLoader.Core;
using SharpPluginLoader.InternalCallGenerator;
using System.Numerics;
using System.Runtime.InteropServices;

namespace PlayerAnimationViewer.Sound;

[InternalCallManager]
public static partial class InternalCalls
{
    [InternalCall(Pattern = "33 C0 40 32 ED 45 8B E1 4C 8B E9 89 44 24 40 39 41 10", Offset = -27)]
    public static partial bool CallEventById(nint container, nint source, nint wwct, int wwevId, int eventId,
        ref EventParams param);

    [StructLayout(LayoutKind.Sequential, Size = 0x30)]
    public struct EventParams
    {
        public Vector4 V0;
        public Vector4 V1;
        public Vector4 V2;
    }
}
