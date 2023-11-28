using SharpPluginLoader.Core.Entities;
using SharpPluginLoader.Core.Memory;

namespace SharpPluginLoader.Core
{
    /// <summary>
    /// Provides a set of utility methods.
    /// </summary>
    public static unsafe class Utility
    {
        private static readonly NativeFunction<string, int, uint> Crc32Func = new(0x1421e5830); // TODO
        private static readonly NativeFunction<uint, nint> FindDtiFunc = new(AddressRepository.Get("MtDti:Find"));
        private static readonly NativeFunction<uint, nint> GetMonsterDtiFunc = new(0x14139eaf0); // TODO
        private static readonly NativeAction<nint, uint> ResizeArrayFunc = new(AddressRepository.Get("MtArray:Reserve"));
        private static readonly NativeFunction<MonsterType, nint> GetMonsterNameFunc = new(AddressRepository.Get("Monster:GetNameFromId"));
        private static readonly NativeFunction<nint, nint, nint, nint, nint> SpawnShellPlayerFunc = new(AddressRepository.Get("Player:CreateShell"));

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

        public static uint MakeDtiId(string name) => Crc32(name) & 0x7FFFFFFF;

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
