using System.Reflection;
using System.Runtime.InteropServices;

namespace SharpPluginLoader.Bootstrapper
{
    public class EntryPoint
    {
        private static CoreLoadContext? _loadContext;
        private static Assembly? _coreAssembly;

        [UnmanagedCallersOnly]
        public static void Initialize()
        {
            try
            {
                _loadContext = new CoreLoadContext(Path.GetFullPath("nativePC/plugins/CSharp/SharpPluginLoader.Core.dll"));
                _coreAssembly = _loadContext.LoadFromAssemblyName(new AssemblyName("SharpPluginLoader.Core"));
            }
            catch (Exception e)
            {
                Log(LogLevel.Error, $"Failed to load SharpPluginLoader.Core.dll: {e.Message}");
                throw;
            }
            
            Log(LogLevel.Info, "SharpPluginLoader.Core.dll loaded into CoreLoadContext");
        }

        [UnmanagedCallersOnly]
        public static void Shutdown()
        {
            if (_loadContext == null)
            {
                Log(LogLevel.Warn, "Shutdown called before Initialize");
                return;
            }

            _loadContext.Unload();
            _loadContext = null;
            _coreAssembly = null;
        }

        [LibraryImport("SharpPluginLoader.dll", EntryPoint = "public_log_interface", StringMarshalling = StringMarshalling.Utf8)]
        private static extern void Log(LogLevel level, string message);

        private enum LogLevel
        {
            Debug = 10,
            Info = 7,
            Warn = 14,
            Error = 12
        }
    }
}
