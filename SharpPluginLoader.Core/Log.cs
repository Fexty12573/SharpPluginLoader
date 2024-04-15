using System.Runtime.InteropServices;

namespace SharpPluginLoader.Core
{
    /// <summary>
    /// Provides basic logging functionality.
    /// </summary>
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

        /// <summary>
        /// Logs a debug message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void Debug(string message) => DoLog(LogLevel.Debug, message);

        /// <summary>
        /// Logs an info message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void Info(string message) => DoLog(LogLevel.Info, message);

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void Warn(string message) => DoLog(LogLevel.Warn, message);

        /// <summary>
        /// Logs an error message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void Error(string message) => DoLog(LogLevel.Error, message);
    }
}
