using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SharpPluginLoader.Core.MtTypes
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MtVector2 : IMtType
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
            return Math.Abs(a.X - b.X) < 0.0001f && Math.Abs(a.Y - b.Y) < 0.0001f;
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

        public float Length()
        {
            return (float)Math.Sqrt(LengthSquared());
        }

        public float LengthSquared()
        {
            return X * X + Y * Y;
        }

        public MtVector2 Normalize()
        {
            return this / Length();
        }

        public MtVector2 SetLength(float length)
        {
            this /= Length();
            this *= length;
            return this;
        }

        public MtVector2 Limit(float limit)
        {
            return LengthSquared() > limit * limit ? SetLength(limit) : this;
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
