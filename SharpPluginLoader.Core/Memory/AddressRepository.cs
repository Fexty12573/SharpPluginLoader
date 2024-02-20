using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SharpPluginLoader.Core.Memory
{
    internal static class AddressRepository
    {
        private const string PluginCachePath = "nativePC/plugins/CSharp/Loader/PluginCache.json";
        public static unsafe void Initialize()
        {
            // We only need to load the plugin cache here, as the "core" of the
            // address repository caching/scanning logic is done at the native level.
            LoadPluginRecords();
        }

        private static void LoadPluginRecords()
        {
            if (!File.Exists(PluginCachePath))
                return;

            using var fs = File.OpenRead(PluginCachePath);
            var pluginCache = JsonSerializer.Deserialize<PluginRecordCacheJson>(fs, SerializerOptions)
                ?? throw new Exception("Failed to deserialize plugin records cache");

            var gameVersion = InternalCalls.GetGameRevision();
            if (string.IsNullOrEmpty(gameVersion))
            {
                Log.Error("Failed to get game revision");
                return;
            }

            if (pluginCache.Version == gameVersion)
            {
                Log.Debug("[Core] Restoring from plugin record cache.");

                foreach (var record in pluginCache.Addresses)
                {
                    PluginRecords[record.Key] = (nint)record.Value;
                }

                return;
            }
            
            // Actual scanning will be performed by the plugins themselves.
            Log.Debug("[Core] No valid plugin record cache found. Performing first-time scan.");
        }

        public static void SavePluginRecords()
        {
            var gameVersion = InternalCalls.GetGameRevision();
            if (string.IsNullOrEmpty(gameVersion))
            {
                Log.Error("Failed to get game revision");
                return;
            }

            var cacheJson = JsonSerializer.Serialize(
                new PluginRecordCacheJson
                {
                    Version = gameVersion,
                    Addresses = PluginRecords.ToDictionary(e => e.Key, e => (ulong)e.Value),
                }) ?? throw new Exception("Failed to serialize plugin records cache");

            File.WriteAllText(PluginCachePath, cacheJson);
        }

        public static nint Get(string name)
        {
            nint address = InternalCalls.GetRepositoryAddress(name);
            if (address == 0)
            {
                throw new Exception($"Failed to find address for {name}");
            }
            return address;
        }

        public static IDictionary<string, nint> GetPluginRecords() => PluginRecords;

        private static readonly ConcurrentDictionary<string, nint> PluginRecords = [];

        private static readonly JsonSerializerOptions SerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
    }
}
