using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SharpPluginLoader.Core
{
    public class MissingEventException : Exception
    {
        public MissingEventException([CallerMemberName] string eventName = "") : base($"Subscribed event {eventName} was not implemented") { }
    }
}
