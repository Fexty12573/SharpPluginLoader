using System.Runtime.InteropServices;

namespace SharpPluginLoader.Core.MtTypes
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MtVector2
    {
        public float X;
        public float Y;

        public MtVector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public static MtVector2 operator +(MtVector2 a, MtVector2 b)
        {
            return new MtVector2(a.X + b.X, a.Y + b.Y);
        }

        public static MtVector2 operator -(MtVector2 a, MtVector2 b)
        {
            return new MtVector2(a.X - b.X, a.Y - b.Y);
        }

        public static MtVector2 operator *(MtVector2 a, float b)
        {
            return new MtVector2(a.X * b, a.Y * b);
        }

        public static MtVector2 operator /(MtVector2 a, float b)
        {
            return new MtVector2(a.X / b, a.Y / b);
        }

        public static MtVector2 operator -(MtVector2 a)
        {
            return new MtVector2(-a.X, -a.Y);
        }

        public static bool operator ==(MtVector2 a, MtVector2 b)
        {
            return a.X.Equals(b.X) && a.Y.Equals(b.Y);
        }

        public static bool operator !=(MtVector2 a, MtVector2 b)
        {
            return !(a == b);
        }

        public override bool Equals(object? obj)
        {
            if (obj is MtVector2 vec3)
            {
                return this == vec3;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }

        public float Length => (float)Math.Sqrt(LengthSquared);

        public float LengthSquared => X * X + Y * Y;

        public MtVector2 Normalize() => this / Length;

        public MtVector2 SetLength(float length)
        {
            this /= Length;
            this *= length;
            return this;
        }

        public MtVector2 Limit(float limit)
        {
            return LengthSquared > limit * limit ? SetLength(limit) : this;
        }

        public static float Dot(MtVector2 a, MtVector2 b)
        {
            return a.X * b.X + a.Y * b.Y;
        }

        public static MtVector2 Lerp(MtVector2 a, MtVector2 b, float t)
        {
            return a + (b - a) * t;
        }
    }
}
