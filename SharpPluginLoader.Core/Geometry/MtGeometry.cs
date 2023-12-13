using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpPluginLoader.Core.Geometry;

public class MtGeometry : MtObject
{
    public MtGeometry(nint instance) : base(instance) { }
    public MtGeometry() { }

    public GeometryType Type => Get<GeometryType>(0x8);

    public ref T GetGeometry<T>() where T : unmanaged
    {
        return ref GetRef<T>(0x10);
    }
}

public enum GeometryType
{
    None = 0,
    Line = 1,
    LineSegment = 2,
    Ray = 3,
    Plane = 4,
    Sphere = 5,
    Capsule = 6,
    Aabb = 7,
    Obb = 8,
    Cylinder = 9,
    ConvexHull = 10,
    Triangle = 11,
    Cone = 12,
    Torus = 13,
    Ellipsoid = 14,
    MincowskiSum = 15,
    MincowskiDifference = 16,
    LineSegment4 = 17,
    Aabb4 = 18,
    LineSweptSphere = 19,
    PlaneXz = 20,
    RayY = 21
}
