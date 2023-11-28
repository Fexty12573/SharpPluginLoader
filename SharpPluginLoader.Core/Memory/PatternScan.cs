using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SharpPluginLoader.Core.Memory.Windows;

namespace SharpPluginLoader.Core.Memory
{
    public static class PatternScanner
    {
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

            var predicate = new PatternCompareFunc<byte>((a, b) => b == -1 || a == b);

            while (addr < endAddr)
            {
                var memInfo = new MemoryBasicInformation();
                if (WinApi.VirtualQuery(addr, out memInfo, (ulong)Marshal.SizeOf(memInfo)) == 0 
                    || memInfo.State != Constants.MemCommit 
                    || (memInfo.Protect & Constants.PageGuard) != 0)
                    break;

                var begin = (byte*)memInfo.BaseAddress;
                var end = begin + memInfo.RegionSize;

                var found = Search(begin, end, pat, predicate);

                while (found != null)
                {
                    results.Add((nint)found);
                    found = Search(found + 1, end, pat, predicate);
                }

                addr = (nint)end;
            }

            return results;
        }
        
        #region Internal
        private delegate bool PatternCompareFunc<in T>(T a, short b) where T : unmanaged;

        private static unsafe T* Search<T>(T* hayStackBegin, T* hayStackEnd, 
            Span<short> pattern, PatternCompareFunc<T> predicate) where T : unmanaged
        {
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

                        if (!predicate(*mid1, *mid2))
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
