using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SharpPluginLoader.Core.Memory
{
    internal static class AddressRepository
    {
        public static unsafe void Initialize()
        {
            var defaultChunk = InternalCalls.GetDefaultChunk();
            var addressRecords = InternalCalls.ChunkGetFile(defaultChunk, "/Resources/AddressRecords.json");
            var addressRecordsSize = InternalCalls.FileGetSize(addressRecords);
            var addressRecordsContents = InternalCalls.FileGetContents(addressRecords);
            var addressRecordsString = Encoding.UTF8.GetString((byte*)addressRecordsContents, (int)addressRecordsSize);

            var records = JsonSerializer.Deserialize<AddressRecordJson[]>(addressRecordsString, SerializerOptions) 
                          ?? throw new Exception("Failed to deserialize address records");

            foreach (var record in records)
                Records.Add(record.Name, new AddressRecord(record.Pattern, record.Offset));
        }

        public static nint Get(string name)
        {
           if (Records.TryGetValue(name, out var record)) 
               return record.Address;

           throw new Exception($"Failed to find address for {name}");
        }

        private static readonly Dictionary<string, AddressRecord> Records = [];

        private static readonly JsonSerializerOptions SerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
    }
}
