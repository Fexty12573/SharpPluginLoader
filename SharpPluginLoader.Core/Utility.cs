using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpPluginLoader.Core
{
    /// <summary>
    /// Provides a set of utility methods.
    /// </summary>
    public static unsafe class Utility
    {
        private static readonly NativeFunction<string, int, uint> Crc32Func = new(0x1421e5830);
        private static readonly NativeFunction<uint, nint> FindDtiFunc = new(0x14216da70);
        private static readonly NativeFunction<uint, nint> GetMonsterDtiFunc = new(0x14139eaf0);
        private static readonly NativeAction<nint, uint> ResizeArrayFunc = new(0x140249b20);
        private static readonly NativeFunction<MonsterType, nint> GetMonsterNameFunc = new(0x14139ee90);

        /// <summary>
        /// Computes the CRC of the specified string. This is the same CRC used by Monster Hunter World.
        /// </summary>
        /// <param name="str">The string to hash</param>
        /// <param name="crc">The initial CRC value</param>
        /// <returns>The CRC hash of the string</returns>
        public static uint Crc32(string str, int crc = -1)
        {
            return Crc32Func.InvokeUnsafe(str, crc);
        }

        internal static uint MakeDtiId(string name) => Crc32(name) & 0x7FFFFFFF;

        internal static nint FindDti(uint id) => FindDtiFunc.Invoke(id);

        internal static nint GetMonsterDti(uint monsterId) => GetMonsterDtiFunc.InvokeUnsafe(monsterId);

        internal static void ResizeArray<T>(MtArray<T> array, uint newSize) where T : MtObject, new()
        {
            ResizeArrayFunc.Invoke(array.Instance, newSize);
        }

        internal static string GetMonsterName(MonsterType monsterType)
        {
            var namePtr = GetMonsterNameFunc.Invoke(monsterType);
            return namePtr != 0 ? new string((sbyte*)namePtr) : "Unknown";
        }
    }
}
