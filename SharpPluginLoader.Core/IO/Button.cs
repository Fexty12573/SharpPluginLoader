using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpPluginLoader.Core.IO
{
    /// <summary>
    /// The buttons on a controller using Playstation names.
    /// </summary>
    [Flags]
    public enum Button : uint
    {
        Share = 1 << 0,
        L3 = 1 << 1,
        R3 = 1 << 2,
        Options = 1 << 3,
        Up = 1 << 4,
        Right = 1 << 5,
        Down = 1 << 6,
        Left = 1 << 7,
        L1 = 1 << 8,
        R1 = 1 << 9,
        L2 = 1 << 10,
        R2 = 1 << 11,
        Triangle = 1 << 12,
        Circle = 1 << 13,
        Cross = 1 << 14,
        Square = 1 << 15,
        LsUp = 1 << 16,
        LsRight = 1 << 17,
        LsDown = 1 << 18,
        LsLeft = 1 << 19,
        RsUp = 1 << 20,
        RsRight = 1 << 21,
        RsDown = 1 << 22,
        RsLeft = 1 << 23
    }
}
