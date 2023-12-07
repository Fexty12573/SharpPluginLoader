using SharpPluginLoader.Core.MtTypes;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SharpPluginLoader.Core.Memory;

namespace SharpPluginLoader.Core.Resources.Animation
{
    [StructLayout(LayoutKind.Explicit, Size = 0x18)]
    public unsafe struct MotionListHeader
    {
        [FieldOffset(0x00)] public uint Magic;
        [FieldOffset(0x04)] public ushort Version;
        [FieldOffset(0x06)] public ushort MotionCount;
        [FieldOffset(0x08)] public uint Unknown1;
        [FieldOffset(0x10)] private nint _motions;

        public bool HasMotion(int id) => id < MotionCount && id >= 0 && Motions[id] != null;

        public ref Motion GetMotion(int id) => ref *Motions[id];

        internal Motion** Motions => (Motion**)Unsafe.AsPointer(ref _motions);
    }

    [StructLayout(LayoutKind.Explicit, Size = 0x60)]
    public unsafe struct Motion
    {
        [FieldOffset(0x00)] public nint Params;
        [FieldOffset(0x08)] public uint ParamNum;
        [FieldOffset(0x0C)] public uint FrameNum;
        [FieldOffset(0x10)] public uint LoopFrame;
        [FieldOffset(0x20)] public MtVector3 BaseTransform;
        [FieldOffset(0x30)] public MtQuaternion BaseQuat;
        [FieldOffset(0x40)] public uint Flags;
        [FieldOffset(0x58)] private readonly Metadata* _metadata;

        public readonly ref Metadata Metadata => ref *_metadata;

        public readonly bool HasMetadata => _metadata != null;
    }

    [StructLayout(LayoutKind.Explicit, Size = 0x30)]
    public unsafe struct MotionParam
    {
        [FieldOffset(0x00)] public byte Type;
        [FieldOffset(0x01)] public byte Usage;
        [FieldOffset(0x02)] public byte JointType;
        [FieldOffset(0x03)] public byte JointCount;
        [FieldOffset(0x04)] public int BoneId;
        [FieldOffset(0x08)] public float Weight;
        [FieldOffset(0x0C)] public int BufferSize;
        [FieldOffset(0x10)] public byte* Buffer;
        [FieldOffset(0x18)] public fixed float ReferenceFrame[4];
        [FieldOffset(0x28)] public MotionBounds* Bounds;
    }

    [StructLayout(LayoutKind.Explicit, Size = 0x20)]
    public unsafe struct MotionBounds
    {
        [FieldOffset(0x00)] public fixed float Addin[4];
        [FieldOffset(0x10)] public fixed float Offset[4];
    }

    [StructLayout(LayoutKind.Explicit, Size = 0x28)]
    public unsafe struct Metadata
    {
        [FieldOffset(0x00)] public MetadataParam* ParamPtr;
        [FieldOffset(0x08)] public int ParamNum;

        [FieldOffset(0x10)] public uint Type1;
        [FieldOffset(0x14)] public uint Type2;
        [FieldOffset(0x18)] public float FrameCount;
        [FieldOffset(0x1C)] public float LoopStart;
        [FieldOffset(0x20)] public uint LoopValue;
        [FieldOffset(0x24)] public uint Hash;

        public readonly ref MetadataParam GetParam(int index)
        {
            if (index >= ParamNum || index < 0)
                throw new IndexOutOfRangeException();
            
            return ref *(ParamPtr + index);
        }

        public readonly Span<MetadataParam> Params => new(ParamPtr, ParamNum);
    }

    [StructLayout(LayoutKind.Explicit, Size = 0x18)]
    public unsafe struct MetadataParam
    {
        [FieldOffset(0x00)] public MetadataParamMember* MemberPtr;
        [FieldOffset(0x08)] public int MemberNum;
        [FieldOffset(0x10)] public uint Hash;
        [FieldOffset(0x14)] public uint UniqueId;

        public readonly ref MetadataParamMember GetMember(int index)
        {
            if (index >= MemberNum || index < 0)
                throw new IndexOutOfRangeException();

            return ref *(MemberPtr + index);
        }

        public readonly Span<MetadataParamMember> Members => new(MemberPtr, MemberNum);

        public readonly MtDti? Dti => MtDti.Find(Hash);
    }

    [StructLayout(LayoutKind.Explicit, Size = 0x18)]
    public unsafe struct MetadataParamMember
    {
        [FieldOffset(0x00)] public MetadataKeyframe* KeyframePtr;
        [FieldOffset(0x08)] public int KeyframeNum;

        [FieldOffset(0x10)] public uint Hash;
        [FieldOffset(0x14)] public uint KeyframeType;

        public readonly ref MetadataKeyframe GetKeyframe(int index)
        {
            if (index >= KeyframeNum || index < 0)
                throw new IndexOutOfRangeException();

            return ref *(KeyframePtr + index);
        }

        public readonly Span<MetadataKeyframe> Keyframes => new(KeyframePtr, KeyframeNum);

        public readonly void SortKeyframes() => Keyframes.Sort((a, b) =>
        {
            if (a.Frame < b.Frame)
                return -1;
            return a.Frame > b.Frame ? 1 : 0;
        });
    }

    [StructLayout(LayoutKind.Explicit, Size = 0x14)]
    public unsafe struct MetadataKeyframe
    {
        [FieldOffset(0x00)] private int _value;
        [FieldOffset(0x04)] public float BounceForwardLimit;
        [FieldOffset(0x04)] public uint FlagsMask;
        [FieldOffset(0x08)] public float BounceBackLimit;
        [FieldOffset(0x0C)] public float Frame;
        [FieldOffset(0x10)] public ApplyType ApplyType;
        [FieldOffset(0x12)] public InterpType InterpolationType;

        public ref float FloatValue => ref Unsafe.AsRef<float>(ValuePtr);
        public ref int IntValue => ref Unsafe.AsRef<int>(ValuePtr);
        public ref uint UIntValue => ref Unsafe.AsRef<uint>(ValuePtr);
        public ref MtColor ColorValue => ref Unsafe.AsRef<MtColor>(ValuePtr);
        public ref bool BoolValue => ref Unsafe.AsRef<bool>(ValuePtr);

        private void* ValuePtr => Unsafe.AsPointer(ref _value);
    }

    public enum ApplyType : short
    {
        None = 0,
        Absolute = 1,
        Interpolated = 2,
        Eased = 3,
        Interpolated2 = 4,
        Flags = 5,
        Trigger = 6,
        Count,
    }

    public enum InterpType : short
    {
        I32 = 0,
        U32 = 1,
        Float = 2,
        Color = 3,
        Count = 4,
    }
}
