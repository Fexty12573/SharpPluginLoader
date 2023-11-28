using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SharpPluginLoader.Core.Memory
{
    [JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Default,
        PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
    [JsonSerializable(typeof(AddressRecordJson[]))]
    internal partial class AddressRecordJsonContext : JsonSerializerContext;

    internal class AddressRecordJson
    {
        public string Name { get; set; } = default!;
        public string Pattern { get; set; } = default!;
        public int Offset { get; set; }
    }
}
