using System.Numerics;
using System.Runtime.CompilerServices;

namespace SharpPluginLoader.Core.MtTypes
{
    public unsafe struct MtMatrix3X3 // TODO: Finish this
    {
        public fixed float Data[9];
        
        public float this[int index]
        {
            get => Data[index];
            set => Data[index] = value;
        }

        public float this[int row, int col]
        {
            get => Data[row * 3 + col];
            set => Data[row * 3 + col] = value;
        }

        public static MtMatrix3X3 Identity => new()
        {
            [0, 0] = 1,
            [1, 1] = 1,
            [2, 2] = 1
        };
    }
}
