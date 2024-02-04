using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SharpPluginLoader.Core.MtTypes;

namespace SharpPluginLoader.Core.Resources.Collision;

public class CollGeomResource : Resource
{
    public CollGeomResource(nint instance) : base(instance, true) { }
    public CollGeomResource() { }

    public PointerArray<CollGeom> Geometries => new(Get<nint>(0xC0), Get<int>(0xB0));
}

[StructLayout(LayoutKind.Explicit, Size = 0x70)]
public struct CollGeom
{
    [FieldOffset(0x08)] public CollGeomShape Shape;
    [FieldOffset(0x09)] public byte Option;
    [FieldOffset(0x0A)] public byte ScaleOption;
    [FieldOffset(0x0B)] public byte Priority;
    [FieldOffset(0x0C)] public byte Layer;
    [FieldOffset(0x0E)] public ushort Index;
    [FieldOffset(0x10)] public short Joint0;
    [FieldOffset(0x12)] public short Joint1;
    [FieldOffset(0x14)] public ushort Region;
    [FieldOffset(0x16)] public short RangeCheckBaseJoint;
    [FieldOffset(0x18)] public float Radius;
    [FieldOffset(0x20)] public MtVector2 Angle0;
    [FieldOffset(0x28)] public MtVector2 Angle1;
    [FieldOffset(0x30)] public MtVector4 Offset0;
    [FieldOffset(0x40)] public MtVector4 Offset1;
    [FieldOffset(0x50)] public MtVector3 Extent;
    [FieldOffset(0x60)] public bool Use;
}

public enum CollGeomShape : byte
{
    Sphere = 0,
    Capsule = 1,
    Obb = 2
}
