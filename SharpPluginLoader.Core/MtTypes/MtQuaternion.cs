using System.Runtime.InteropServices;

namespace SharpPluginLoader.Core.MtTypes
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MtQuaternion : IMtType
    {
        public float X;
        public float Y;
        public float Z;
        public float W;

        public MtQuaternion(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        #region Properties

        public float Length => (float)Math.Sqrt(X * X + Y * Y + Z * Z + W * W);

        public float LengthSquared => X * X + Y * Y + Z * Z + W * W;

        public MtQuaternion Normalized => this / Length;

        public MtQuaternion NormalizedSafe => LengthSquared == 0 ? Identity : Normalized;

        public float Angle => (float)Math.Acos(W) * 2f;

        public MtVector3 Axis
        {
            get
            {
                var s1 = 1f - W * W;
                if (s1 < 0) return new MtVector3(0f, 0f, 1f);
                var s2 = 1f / (float)Math.Sqrt(s1);
                return new MtVector3(X * s2, Y * s2, Z * s2);
            }
        }

        public float Yaw => (float)Math.Asin(-2 * (double)(X * Z - W * Y));

        public float Pitch => (float)Math.Atan2(2 * (double)(Y * Z + W * X), W * W - X * X - Y * Y + Z * Z);

        public float Roll => (float)Math.Atan2(2 * (double)(X * Y + W * Z), W * W + X * X - Y * Y - Z * Z);

        public MtVector3 EulerAngle => new(Pitch, Yaw, Roll);

        public MtQuaternion Conjugate => new(-X, -Y, -Z, W);

        public MtQuaternion Inverse => Conjugate / LengthSquared;

        #endregion

        #region Static Properties

        public static MtQuaternion Zero => new(0f, 0f, 0f, 0f);

        public static MtQuaternion One => new(1f, 1f, 1f, 1f);

        public static MtQuaternion Identity => new(0f, 0f, 0f, 1f);
        
        public static MtQuaternion UnitX => new(1f, 0f, 0f, 0f);

        public static MtQuaternion UnitY => new(0f, 1f, 0f, 0f);

        public static MtQuaternion UnitZ => new(0f, 0f, 1f, 0f);    

        public static MtQuaternion UnitW => new(0f, 0f, 0f, 1f);

        #endregion

        #region operators

        public static MtQuaternion operator *(MtQuaternion l, MtQuaternion r)
        {
            return new MtQuaternion(
                l.W * r.X + l.X * r.W + l.Y * r.Z - l.Z * r.Y,
                l.W * r.Y + l.Y * r.W + l.Z * r.X - l.X * r.Z,
                l.W * r.Z + l.Z * r.W + l.X * r.Y - l.Y * r.X,
                l.W * r.W - l.X * r.X - l.Y * r.Y - l.Z * r.Z
            );
        }

        public static MtQuaternion operator /(MtQuaternion q, float s)
        {
            return new MtQuaternion(q.X / s, q.Y / s, q.Z / s, q.W / s);
        }

        public static MtVector3 operator *(MtQuaternion q, MtVector3 v)
        {
            var qv = new MtVector3(q.X, q.Y, q.Z);
            var uv = MtVector3.Cross(qv, v);
            var uuv = MtVector3.Cross(qv, uv);
            return v + (uv * q.W + uuv) * 2f;
        }

        public static MtVector4 operator *(MtQuaternion q, MtVector4 v)
        {
            return new MtVector4(q * new MtVector3(v.X, v.Y, v.Z), v.W);
        }

        public static bool operator ==(MtQuaternion left, MtQuaternion right)
        {
            return left.X.Equals(right.X) && left.Y.Equals(right.Y) && left.Z.Equals(right.Z) && left.W.Equals(right.W);
        }

        public static bool operator !=(MtQuaternion left, MtQuaternion right)
        {
            return !(left == right);
        }

        #endregion

        public override bool Equals(object? obj)
        {
            if (obj is null)
                return false;

            return obj is MtQuaternion quaternion &&
                   this == quaternion;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z, W);
        }

        public override string ToString()
        {
            return $"({X}, {Y}, {Z}, {W})";
        }
    }
}
