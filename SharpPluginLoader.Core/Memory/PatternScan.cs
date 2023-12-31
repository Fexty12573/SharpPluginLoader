﻿using System.Runtime.InteropServices;
using SharpPluginLoader.Core.Memory.Windows;

namespace SharpPluginLoader.Core.Memory
{
    public static class PatternScanner
    {
        /// <summary>
        /// Scans the entire process for the specified pattern.
        /// </summary>
        /// <param name="pattern">The pattern to scan for.</param>
        /// <returns>A list of addresses where the pattern was found.</returns>
        /// <remarks>
        /// It is strongly recommended to cache the results of this method.
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
            var pat = new Span<short>(pattern.Bytes);

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

        #region Internal

        private static unsafe byte* Search(byte* hayStackBegin, byte* hayStackEnd, Span<short> pattern)
        {
            // Boyer-Moore-Horspool algorithm, and essentially just copied from 
            // the MSVC implementation of std::search.
            var first1 = hayStackBegin;
            var last1 = hayStackEnd;
            fixed (short* first2 = &pattern[0])
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

                        if (*mid2 != -1 && *mid1 != *mid2)
                            break;
                    }
                }
            }
        }

        #endregion
    }

    public readonly struct Pattern
    {
        public required short[] Bytes { get; init; }

        public static Pattern FromString(string pattern)
        {
            var patternBytes = new List<short>();
            var patternSplit = pattern.Split(' ');
            foreach (var patternByte in patternSplit)
            {
                if (patternByte is "?" or "??")
                {
                    patternBytes.Add(-1);
                }
                else
                {
                    patternBytes.Add(Convert.ToInt16(patternByte, 16));
                }
            }

            return new Pattern { Bytes = [.. patternBytes] };
        }
    }
}
