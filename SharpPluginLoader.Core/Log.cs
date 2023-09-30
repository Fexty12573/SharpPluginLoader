using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SharpPluginLoader.Core
{
    public static unsafe class Log
    {
        private static delegate* unmanaged<int, nint, void> _logFunc;

        private enum LogLevel
        {
            Debug = 10,
            Info = 7,
            Warn = 14,
            Error = 12
        }

        internal static void Initialize(delegate* unmanaged<int, nint, void> logFunc) => _logFunc = logFunc;

        private static void DoLog(LogLevel level, string message)
        {
            var str = Marshal.StringToHGlobalAnsi(message);
            _logFunc((int)level, str);
            Marshal.FreeHGlobal(str);
        }

        public static void Debug(string message) => DoLog(LogLevel.Debug, message);

        public static void Info(string message) => DoLog(LogLevel.Info, message);

        public static void Warn(string message) => DoLog(LogLevel.Warn, message);

        public static void Error(string message) => DoLog(LogLevel.Error, message);
    }
}
