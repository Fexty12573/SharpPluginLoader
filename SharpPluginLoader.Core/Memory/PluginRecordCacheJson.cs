using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpPluginLoader.Core.Memory;

internal class PluginRecordCacheJson
{
    public required string Version { get; set; }
    public required Dictionary<string, ulong> Addresses { get; set; }
}
