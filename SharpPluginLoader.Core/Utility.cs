using System.Diagnostics;
using System.Text;
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
        private static readonly NativeAction<nint, bool> ClearArrayFunc = new(AddressRepository.Get("MtArray:Clear"));
        private static readonly NativeAction<nint, nint, int> ArrayInsertFunc = new(AddressRepository.Get("MtArray:Insert"));
        private static readonly NativeAction<nint, nint> ArrayEraseFunc = new(AddressRepository.Get("MtArray:Erase"));
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

        internal static void ClearArray<T>(MtArray<T> array, bool freeMem) where T : MtObject, new()
        {
            ClearArrayFunc.Invoke(array.Instance, freeMem);
        }

        internal static void ArrayInsert<T>(MtArray<T> array, int index, T value) where T : MtObject, new()
        {
            ArrayInsertFunc.Invoke(array.Instance, value.Instance, index);
        }

        internal static void ArrayErase<T>(MtArray<T> array, T obj) where T : MtObject, new()
        {
            ArrayEraseFunc.Invoke(array.Instance, obj.Instance);
        }

        internal static string GetMonsterName(MonsterType monsterType)
        {
            var namePtr = GetMonsterNameFunc.Invoke(monsterType);
            return namePtr != 0 ? new string((sbyte*)namePtr) : "Unknown";
        }

        internal static string Format(this StackTrace st)
        {
            StringBuilder sb = new(1024);

            foreach (var frame in st.GetFrames())
            {
                var method = frame.GetMethod();
                if (method is null)
                {
                    sb.AppendLine($"    at {frame.GetNativeOffset()} (Native)");
                    continue;
                }

                sb.AppendLine($"    at {method.Name} ({frame.GetFileName()}:{frame.GetFileLineNumber()}:{frame.GetFileColumnNumber()})");
            }

            return sb.ToString();
        }
    }
}
