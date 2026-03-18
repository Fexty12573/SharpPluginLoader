using System.Runtime.InteropServices;
using SharpPluginLoader.Core.Memory.Windows;

namespace SharpPluginLoader.Core.Memory
{
    /// <summary>
    /// Provides methods for scanning memory for patterns.
    /// </summary>
    public static class PatternScanner
    {
        /// <inheritdoc cref="Scan(Pattern)"/>
        public static unsafe List<nint> Scan(string pattern) => Scan(Pattern.FromString(pattern));

        /// <inheritdoc cref="FindFirst(Pattern,bool)"/>
        public static unsafe nint FindFirst(string pattern, bool cache = true) => FindFirst(Pattern.FromString(pattern), cache);

        /// <summary>
        /// Scans the entire process for the specified pattern.
        /// </summary>
        /// <param name="pattern">The pattern to scan for.</param>
        /// <returns>A list of addresses where the pattern was found.</returns>
        /// <remarks>
        /// It is strongly recommended to cache the results of this method.
        /// For automatic caching, use <see cref="FindFirst(Pattern)"/> instead.
        /// </remarks>
        public static unsafe List<nint> Scan(Pattern pattern)
        {
            List<nint> results = [];
            var module = WinApi.GetModuleHandle("MonsterHunterWorld.exe");
            if (module == 0)
                return results;

            var moduleInfo = new ModuleInfo();
            if (!WinApi.GetModuleInformation(WinApi.GetCurrentProcess(), module, out moduleInfo, (uint)Marshal.SizeOf(moduleInfo)))
                return results;

            var addr = module;
            var endAddr = module + moduleInfo.SizeOfImage;
            var pat = new Span<Pattern.Byte>(pattern.Bytes);

            while (addr < endAddr)
            {
                var memInfo = new MemoryBasicInformation();
                if (WinApi.VirtualQuery(addr, out memInfo, (ulong)Marshal.SizeOf(memInfo)) == 0
                    || memInfo.State != Constants.MemCommit
                    || (memInfo.Protect & Constants.PageGuard) != 0)
                    break;

                var begin = (byte*)memInfo.BaseAddress;
                var end = begin + memInfo.RegionSize;

                var found = Search(begin, end, pat);

                while (found != null)
                {
                    results.Add((nint)found);
                    found = Search(found + 1, end, pat);
                }

                addr = (nint)end;
            }

            return results;
        }

        /// <summary>
        /// Finds the first occurrence of the specified pattern in the process.
        /// </summary>
        /// <param name="pattern">The pattern to search for.</param>
        /// <returns>The address of the first occurrence of the pattern, or 0 if not found.</returns>
        /// <remarks>
        /// This method will cache the result for future calls. To disable caching, use <see cref="FindFirst(Pattern, bool)"/>.
        /// </remarks>
        public static nint FindFirst(Pattern pattern)
        {
            return FindFirst(pattern, true);
        }

        /// <summary>
        /// Finds the first occurrence of the specified pattern in the process.
        /// </summary>
        /// <param name="pattern">The pattern to search for.</param>
        /// <param name="cache">Whether to cache the result for future calls. (Works across restarts)</param>
        /// <returns>The address of the first occurrence of the pattern, or 0 if not found.</returns>
        public static unsafe nint FindFirst(Pattern pattern, bool cache)
        {
            var records = AddressRepository.GetPluginRecords();
            if (cache)
            {
                if (records.TryGetValue(pattern.ToString(), out var address))
                {
                    Log.Debug($"Found cached address for {pattern}: 0x{address:X}");
                    return address;
                }

                Log.Debug($"No cached address found for {pattern}");
            }

            var module = WinApi.GetModuleHandle("MonsterHunterWorld.exe");
            if (module == 0)
                return 0;

            var moduleInfo = new ModuleInfo();
            if (!WinApi.GetModuleInformation(WinApi.GetCurrentProcess(), module, out moduleInfo, (uint)Marshal.SizeOf(moduleInfo)))
                return 0;

            var addr = module;
            var endAddr = module + moduleInfo.SizeOfImage;
            var pat = new Span<Pattern.Byte>(pattern.Bytes);

            while (addr < endAddr)
            {
                var memInfo = new MemoryBasicInformation();
                if (WinApi.VirtualQuery(addr, out memInfo, (ulong)Marshal.SizeOf(memInfo)) == 0
                    || memInfo.State != Constants.MemCommit
                    || (memInfo.Protect & Constants.PageGuard) != 0)
                    break;

                var begin = (byte*)memInfo.BaseAddress;
                var end = begin + memInfo.RegionSize;

                var found = Search(begin, end, pat);

                if (found != null)
                {
                    if (cache)
                        records[pattern.ToString()] = (nint)found;

                    return (nint)found;
                }

                addr = (nint)end;
            }

            return 0;
        }

        #region Internal

        private static unsafe byte* Search(byte* hayStackBegin, byte* hayStackEnd, Span<Pattern.Byte> pattern)
        {
            // Boyer-Moore-Horspool algorithm, and essentially just copied from 
            // the MSVC implementation of std::search.
            var first1 = hayStackBegin;
            var last1 = hayStackEnd;
            fixed (Pattern.Byte* first2 = &pattern[0])
            {
                var last2 = first2 + pattern.Length;

                for (;; ++first1)
                {
                    var mid1 = first1;
                    for (var mid2 = first2;; ++mid1, ++mid2)
                    {
                        if (mid2 == last2)
                            return first1;

                        if (mid1 == last1)
                            return null;

                        if (!mid2->IsWildcard && *mid1 != mid2->Value)
                            break;
                    }
                }
            }
        }

        #endregion
    }

    public readonly struct Pattern
    {
        public required Byte[] Bytes { get; init; }

        public static Pattern FromString(string pattern)
        {
            var patternBytes = new List<Byte>();
            var patternSplit = pattern.Split(' ');
            foreach (var patternByte in patternSplit)
            {
                if (patternByte is "?" or "??")
                {
                    patternBytes.Add(new Byte(true, 0));
                }
                else
                {
                    patternBytes.Add(new Byte(false, Convert.ToByte(patternByte, 16)));
                }
            }

            return new Pattern { Bytes = [.. patternBytes] };
        }

        public static Pattern FromBytes(params byte[] bytes)
        {
            var patternBytes = new List<Byte>();
            foreach (var patternByte in bytes)
            {
                patternBytes.Add(new Byte(false, patternByte));
            }

            return new Pattern { Bytes = [.. patternBytes] };
        }

        public override string ToString()
        {
            var outputParts = new List<string>();
            foreach (var patternByte in Bytes)
            {
                if(patternByte.IsWildcard)
                {
                    outputParts.Add("??");
                }
                else
                {
                    outputParts.Add(Convert.ToHexString([patternByte.Value]));
                }
            }
            return string.Join(" ", outputParts);
        }

        [StructLayout(LayoutKind.Sequential)]
        public record struct Byte(bool IsWildcard, byte Value);
    }
}
