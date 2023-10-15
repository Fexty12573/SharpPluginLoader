using System;
using System.Runtime.InteropServices;

namespace SharpPluginLoader.Core.MtTypes
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MtVector3 : IMtType
    {
        public float X;
        public float Y;
        public float Z;

        public MtVector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static MtVector3 operator +(MtVector3 a, MtVector3 b)
        {
            return new MtVector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static MtVector3 operator -(MtVector3 a, MtVector3 b)
        {
            return new MtVector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static MtVector3 operator *(MtVector3 a, float b)
        {
            return new MtVector3(a.X * b, a.Y * b, a.Z * b);
        }

        public static MtVector3 operator /(MtVector3 a, float b)
        {
            return new MtVector3(a.X / b, a.Y / b, a.Z / b);
        }

        public static MtVector3 operator -(MtVector3 a)
        {
            return new MtVector3(-a.X, -a.Y, -a.Z);
        }

        public static bool operator ==(MtVector3 a, MtVector3 b)
        {
            return Math.Abs(a.X - b.X) < 0.0001f && Math.Abs(a.Y - b.Y) < 0.0001f && Math.Abs(a.Z - b.Z) < 0.0001f;
        }

        public static bool operator !=(MtVector3 a, MtVector3 b)
        {
            return !(a == b);
        }

        public override bool Equals(object? obj)
        {
            if (obj is MtVector3 vec3)
            {
                return this == vec3;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();
        }

        public float Length()
        {
            return (float)Math.Sqrt(LengthSquared());
        }

        public float LengthSquared()
        {
            return X * X + Y * Y + Z * Z;
        }

        public MtVector3 Normalize()
        {
            return this / Length();
        }

        public MtVector3 SetLength(float length)
        {
            this /= Length();
            this *= length;
            return this;
        }

        public MtVector3 Limit(float limit)
        {
            return LengthSquared() > limit * limit ? SetLength(limit) : this;
        }

        public static float Dot(MtVector3 a, MtVector3 b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }

        public static MtVector3 Cross(MtVector3 a, MtVector3 b)
        {
            return new MtVector3(a.Y * b.Z - a.Z * b.Y, a.Z * b.X - a.X * b.Z, a.X * b.Y - a.Y * b.X);
        }

        public static MtVector3 Lerp(MtVector3 a, MtVector3 b, float t)
        {
            return a + (b - a) * t;
        }
    }
}
