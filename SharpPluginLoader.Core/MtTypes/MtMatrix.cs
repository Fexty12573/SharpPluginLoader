using System.Numerics;
using System.Runtime.CompilerServices;

namespace SharpPluginLoader.Core.MtTypes
{
    public struct MtMatrix3X3 // TODO: Finish this
    {
        public unsafe fixed float Data[9];

        public unsafe float this[int index]
        {
            get => Data[index];
            set => Data[index] = value;
        }

        public unsafe float this[int row, int col]
        {
            get => Data[row * 3 + col];
            set => Data[row * 3 + col] = value;
        }

        public static MtMatrix3X3 Identity => new MtMatrix3X3
        {
            [0, 0] = 1,
            [1, 1] = 1,
            [2, 2] = 1
        };
    }

    public struct MtMatrix4X4 // TODO: Finish this
    {
        public unsafe fixed float Data[16];

        public unsafe float this[int index]
        {
            get => Data[index];
            set => Data[index] = value;
        }

        public unsafe float this[int row, int col]
        {
            get => Data[row * 4 + col];
            set => Data[row * 4 + col] = value;
        }

        public static implicit operator Matrix4x4(MtMatrix4X4 m) => Unsafe.As<MtMatrix4X4, Matrix4x4>(ref m);
        public static implicit operator MtMatrix4X4(Matrix4x4 m) => Unsafe.As<Matrix4x4, MtMatrix4X4>(ref m);

        public static MtMatrix4X4 Identity => new()
        {
            [0, 0] = 1,
            [1, 1] = 1,
            [2, 2] = 1,
            [3, 3] = 1
        };
    }
}
