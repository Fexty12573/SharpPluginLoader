using System.Runtime.CompilerServices;

namespace SharpPluginLoader.Core
{
    public class MissingEventException : Exception
    {
        public MissingEventException([CallerMemberName] string eventName = "") : base($"Subscribed event {eventName} was not implemented") { }
    }
}
