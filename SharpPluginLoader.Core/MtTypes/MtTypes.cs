using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;

namespace SharpPluginLoader.Core.MtTypes
{
    [StructLayout(LayoutKind.Explicit)]
    public struct Float2
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
    public struct Float3
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
    public struct Float4
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
    public struct Float3X3
    {
        [FieldOffset(0x0)] private unsafe fixed float _data[9];

        public unsafe float this[int index]
        {
            get => _data[index];
            set => _data[index] = value;
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct Float4X3
    {
        [FieldOffset(0x0)] private unsafe fixed float _data[12];

        public unsafe float this[int index]
        {
            get => _data[index];
            set => _data[index] = value;
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct Float3X4
    {
        [FieldOffset(0x0)] private unsafe fixed float _data[12];

        public unsafe float this[int index]
        {
            get => _data[index];
            set => _data[index] = value;
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct Float4X4
    {
        [FieldOffset(0x0)] private unsafe fixed float _data[16];

        public unsafe float this[int index]
        {
            get => _data[index];
            set => _data[index] = value;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MtPoint
    {
        public uint X;
        public uint Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MtPointF
    {
        public float X;
        public float Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MtSize
    {
        public uint Width;
        public uint Height;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MtSizeF
    {
        public float Width;
        public float Height;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MtRect
    {
        public uint X;
        public uint Y;
        public uint W;
        public uint H;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MtRectF
    {
        public float X;
        public float Y;
        public float W;
        public float H;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MtTime
    {
        public ulong Time;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MtEaseCurve
    {
        public float P1;
        public float P2;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MtLine
    {
        public MtVector3 Point;
        public MtVector3 Direction;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MtLineSegment
    {
        public MtVector3 Point1;
        public MtVector3 Point2;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MtPlane
    {
        public MtVector3 Normal;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MtSphere
    {
        public MtVector3 Center;
        public float Radius;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MtCapsule
    {
        public MtVector3 Point1;
        public MtVector3 Point2;
        public float Radius;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MtAabb
    {
        public MtVector3 Min;
        public MtVector3 Max;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MtObb
    {
        public Float4X4 Transform;
        public MtVector3 Extents;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MtCylinder
    {
        public MtVector3 Point1;
        public MtVector3 Point2;
        public float Radius;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MtTriangle
    {
        public MtVector3 Point1;
        public MtVector3 Point2;
        public MtVector3 Point3;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MtRange
    {
        public int Min;
        public int Max;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MtRangeF
    {
        public float Min;
        public float Max;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MtRangeU16
    {
        public ushort Min;
        public ushort Max;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MtHermiteCurve
    {
        public unsafe fixed float X[8];
        public unsafe fixed float Y[8];

        public static int PointCount => 8;

        /// <summary>
        /// Creates a new Hermite curve from a list of points.
        /// </summary>
        /// <param name="points">A list of at most 8 points, with the first element of each tuple being the X value,
        /// and the second element being the Y value.</param>
        public unsafe MtHermiteCurve(IList<(float, float)> points)
        {
            for (var i = 0; i < points.Count; i++)
            {
                X[i] = points[i].Item1;
                Y[i] = points[i].Item2;
            }
        }

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
    public struct MtPlaneXz
    {
        public MtVector3 Normal;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct MtColor(byte r, byte g, byte b, byte a)
    {
        [FieldOffset(0x0)] public uint Rgba;

        [FieldOffset(0x0)] public byte R = r;
        [FieldOffset(0x1)] public byte G = g;
        [FieldOffset(0x2)] public byte B = b;
        [FieldOffset(0x3)] public byte A = a;

        public static implicit operator MtColor(Color color) => new() { R = color.R, G = color.G, B = color.B, A = color.A };
        public static implicit operator MtColor(uint rgba) => new() { Rgba = rgba };
        public static implicit operator uint(MtColor color) => color.Rgba;
        public static explicit operator MtColor(MtVector4 color) => new()
        {
            R = (byte)(color.X * 255), 
            G = (byte)(color.Y * 255), 
            B = (byte)(color.Z * 255), 
            A = (byte)(color.W * 255)
        };
        public static explicit operator MtVector4(MtColor color) => new()
        {
            X = color.R / 255.0f, 
            Y = color.G / 255.0f, 
            Z = color.B / 255.0f, 
            W = color.A / 255.0f
        };

        public Vector4 ToVector4() => new() { X = R / 255.0f, Y = G / 255.0f, Z = B / 255.0f, W = A / 255.0f };
        public static MtColor FromVector4(Vector4 color) => new()
        {
            R = (byte)(color.X * 255), 
            G = (byte)(color.Y * 255), 
            B = (byte)(color.Z * 255), 
            A = (byte)(color.W * 255)
        };
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MtString
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
