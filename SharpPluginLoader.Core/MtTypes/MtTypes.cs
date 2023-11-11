using System.Runtime.InteropServices;

namespace SharpPluginLoader.Core.MtTypes
{
    /// <summary>
    /// Represents an MtFramework struct type.
    /// </summary>
    /// <remarks>All structs that implement this interface must have an explicit struct layout, 
    /// and match their native counterparts exactly.</remarks>
    public interface IMtType { }

    [StructLayout(LayoutKind.Explicit)]
    public struct Float2 : IMtType
    {
        [FieldOffset(0x0)] public float X;
        [FieldOffset(0x4)] public float Y;

        [FieldOffset(0x0)] private unsafe fixed float _data[2];

        public unsafe float this[int index]
        {
            get => _data[index];
            set => _data[index] = value;
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct Float3 : IMtType
    {
        [FieldOffset(0x0)] public float X;
        [FieldOffset(0x4)] public float Y;
        [FieldOffset(0x8)] public float Z;

        [FieldOffset(0x0)] private unsafe fixed float _data[3];

        public unsafe float this[int index]
        {
            get => _data[index];
            set => _data[index] = value;
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct Float4 : IMtType
    {
        [FieldOffset(0x0)] public float X;
        [FieldOffset(0x4)] public float Y;
        [FieldOffset(0x8)] public float Z;
        [FieldOffset(0xC)] public float W;

        [FieldOffset(0x0)] private unsafe fixed float _data[4];

        public unsafe float this[int index]
        {
            get => _data[index];
            set => _data[index] = value;
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct Float3X3 : IMtType
    {
        [FieldOffset(0x0)] private unsafe fixed float _data[9];

        public unsafe float this[int index]
        {
            get => _data[index];
            set => _data[index] = value;
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct Float4X3 : IMtType
    {
        [FieldOffset(0x0)] private unsafe fixed float _data[12];

        public unsafe float this[int index]
        {
            get => _data[index];
            set => _data[index] = value;
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct Float3X4 : IMtType
    {
        [FieldOffset(0x0)] private unsafe fixed float _data[12];

        public unsafe float this[int index]
        {
            get => _data[index];
            set => _data[index] = value;
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct Float4X4 : IMtType
    {
        [FieldOffset(0x0)] private unsafe fixed float _data[16];

        public unsafe float this[int index]
        {
            get => _data[index];
            set => _data[index] = value;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MtPoint : IMtType
    {
        public uint X;
        public uint Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MtPointF : IMtType
    {
        public float X;
        public float Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MtSize : IMtType
    {
        public uint Width;
        public uint Height;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MtSizeF : IMtType
    {
        public float Width;
        public float Height;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MtRect : IMtType
    {
        public uint X;
        public uint Y;
        public uint W;
        public uint H;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MtRectF : IMtType
    {
        public float X;
        public float Y;
        public float W;
        public float H;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MtTime : IMtType
    {
        public ulong Time;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MtEaseCurve : IMtType
    {
        public float P1;
        public float P2;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MtLine : IMtType
    {
        public MtVector3 Point;
        public MtVector3 Direction;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MtLineSegment : IMtType
    {
        public MtVector3 Point1;
        public MtVector3 Point2;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MtPlane : IMtType
    {
        public MtVector3 Normal;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MtSphere : IMtType
    {
        public MtVector3 Center;
        public float Radius;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MtCapsule : IMtType
    {
        public MtVector3 Point1;
        public MtVector3 Point2;
        public float Radius;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MtAABB : IMtType
    {
        public MtVector3 Min;
        public MtVector3 Max;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MtOBB : IMtType
    {
        public Float4X4 Transform;
        public MtVector3 Extents;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MtCylinder : IMtType
    {
        public MtVector3 Point1;
        public MtVector3 Point2;
        public float Radius;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MtTriangle : IMtType
    {
        public MtVector3 Point1;
        public MtVector3 Point2;
        public MtVector3 Point3;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MtRange : IMtType
    {
        public int Min;
        public int Max;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MtRangeF : IMtType
    {
        public float Min;
        public float Max;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MtRangeU16 : IMtType
    {
        public ushort Min;
        public ushort Max;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MtHermitCurve : IMtType
    {
        public unsafe fixed float X[8];
        public unsafe fixed float Y[8];

        public int PointCount => 8;

        public unsafe int EffectivePointCount
        {
            get
            {
                for (var i = 0; i < PointCount; i++)
                {
                    if (X[i] == 1.0f)
                        return i;
                }

                return PointCount;
            }
        }

        private unsafe float Get(float t)
        {
            if (t <= X[0])
            {
                return Y[0];
            }

            if (t >= 1.0f)
            {
                for (var i = 0; i < 8; ++i)
                {
                    if (X[i] == 1.0f)
                    {
                        return Y[i];
                    }
                }
            }

            var n = 0;
            for (var i = 1; i < 8; ++i)
            {
                if (X[i] > t)
                {
                    n = i - 1;
                    break;
                }
            }

            var dx = X[n + 1] - X[n];
            float dy;

            if (n == 0)
            {
                dy = Y[1] - Y[0];
            }
            else
            {
                var dx0 = X[n] - X[n - 1];
                var dy0 = Y[n] - Y[n - 1];
                dy = (Y[n + 1] - ((1.0f - dx / dx0) * dy0 + Y[n - 1])) * 0.5f;
            }

            float _dy;
            if (X[n + 1] == 1.0f || n > 5)
            {
                _dy = Y[n + 1] - Y[n];
            }
            else
            {
                var dx1 = X[n + 2] - X[n + 1];
                var dy1 = Y[n + 2] - Y[n + 1];
                _dy = ((dx / dx1) * dy1 + Y[n + 1] - Y[n]) * 0.5f;
            }

            var ratio = (t - X[n]) / dx;
            var rsq = ratio * ratio;
            var rcb = rsq * ratio;

            var o2 = (2 * (Y[n] - Y[n + 1]) + dy + _dy) * rcb;
            var o3 = (3 * (Y[n + 1] - Y[n]) - dy - dy - _dy) * rsq;

            var result = o2 + o3 + ratio * dy + Y[n];

            return MathF.Max(MathF.Min(result, 1.0f), 0.0f);
        }

        public float this[float t] => Get(t);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MtPlaneXZ : IMtType 
    {
        public MtVector3 Normal;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct MtColor : IMtType
    {
        [FieldOffset(0x0)] public uint Rgba;

        [FieldOffset(0x0)] public byte R;
        [FieldOffset(0x1)] public byte G;
        [FieldOffset(0x2)] public byte B;
        [FieldOffset(0x3)] public byte A;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MtString : IMtType
    {
        public int RefCount;
        public uint Length;
        public unsafe fixed char String[1];

        public unsafe char this[int index]
        {
            get => String[index];
            set => String[index] = value;
        }
    }
}
