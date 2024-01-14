using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SharpPluginLoader.Core.Memory
{
    internal class AddressRecordCacheJson
    {
        public required string Version { get; set; }
        public required string AddressRecordFileHash { get; set; }
        public required Dictionary<string, ulong> Addresses { get; set; }
    }
}
