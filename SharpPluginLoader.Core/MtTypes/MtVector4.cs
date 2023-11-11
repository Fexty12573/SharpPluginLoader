using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SharpPluginLoader.Core.MtTypes
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MtVector4 : IMtType
    {
        public float X;
        public float Y;
        public float Z;
        public float W;

        public MtVector4(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public MtVector4(MtVector3 v3, float w = 1.0f)
        {
            X = v3.X;
            Y = v3.Y;
            Z = v3.Z;
            W = w;
        }

        public static MtVector4 operator +(MtVector4 a, MtVector4 b)
        {
            return new MtVector4(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W);
        }

        public static MtVector4 operator -(MtVector4 a, MtVector4 b)
        {
            return new MtVector4(a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.W - b.W);
        }

        public static MtVector4 operator *(MtVector4 a, float b)
        {
            return new MtVector4(a.X * b, a.Y * b, a.Z * b, a.W * b);
        }

        public static MtVector4 operator /(MtVector4 a, float b)
        {
            return new MtVector4(a.X / b, a.Y / b, a.Z / b, a.W / b);
        }

        public static MtVector4 operator -(MtVector4 a)
        {
            return new MtVector4(-a.X, -a.Y, -a.Z, -a.W);
        }

        public static bool operator ==(MtVector4 a, MtVector4 b)
        {
            return a.X.Equals(b.X) && a.Y.Equals(b.Y) && a.Z.Equals(b.Z) && a.W.Equals(b.W);
        }

        public static bool operator !=(MtVector4 a, MtVector4 b)
        {
            return !(a == b);
        }

        public override bool Equals(object? obj)
        {
            if (obj is MtVector4 vec4)
            {
                return this == vec4;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode() ^ W.GetHashCode();
        }

        public float Length => (float)Math.Sqrt(LengthSquared);

        public float LengthSquared => X * X + Y * Y + Z * Z + W * W;

        public MtVector4 Normalized => this / Length;

        public MtVector4 SetLength(float length)
        {
            this /= Length;
            this *= length;
            return this;
        }

        public MtVector4 Limit(float limit)
        {
            return LengthSquared > limit * limit ? SetLength(limit) : this;
        }

        public static float Dot(MtVector4 a, MtVector4 b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z + a.W * b.W;
        }

        public static MtVector4 Lerp(MtVector4 a, MtVector4 b, float t)
        {
            return a + (b - a) * t;
        }

        public MtVector3 ToVector3()
        {
            return new MtVector3(X, Y, Z);
        }
    }
}
