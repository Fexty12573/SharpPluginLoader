using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SharpPluginLoader.Core.Memory
{
    internal static class AddressRepository
    {
        private const string AddressCachePath = "nativePC/plugins/CSharp/Loader/AddressCache.json";
        public static unsafe void Initialize()
        {
            // Load address records JSON from the chunk.
            var defaultChunk = InternalCalls.GetDefaultChunk();
            var addressRecordsPtr = InternalCalls.ChunkGetFile(defaultChunk, "/Resources/AddressRecords.json");
            var addressRecordsSize = InternalCalls.FileGetSize(addressRecordsPtr);
            var addressRecordsContents = InternalCalls.FileGetContents(addressRecordsPtr);
            var addressRecordsString = Encoding.UTF8.GetString((byte*)addressRecordsContents, (int)addressRecordsSize);
            var addressRecords = JsonSerializer.Deserialize<AddressRecordJson[]>(addressRecordsString, SerializerOptions)
              ?? throw new Exception("Failed to deserialize address records");


            var gameVersion = GetGameRevision(addressRecords);
            Log.Debug($"[Core] Attempting to initialize address repository for game revision: {gameVersion}");
            var addressRecordFileHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(addressRecordsString)));

            if (File.Exists(AddressCachePath))
            {
                // Verify the game client version and AddressRecords.json hash are the same.
                var addressCacheString = File.ReadAllText(AddressCachePath, Encoding.UTF8);
                var cacheRecords = JsonSerializer.Deserialize<AddressRecordCacheJson>(addressCacheString, SerializerOptions)
                              ?? throw new Exception("Failed to deserialize address records cache");

                if (cacheRecords.Version == gameVersion && cacheRecords.AddressRecordFileHash == addressRecordFileHash)
                {
                    Log.Debug("[Core] Restoring from address record cache.");

                    foreach (var record in cacheRecords.Addresses)
                    {
                        Records[record.Key] = (nint)record.Value;
                    }

                    // Restored from cache, return early.
                    return;
                }
            }

            // Either the cache file doesn't exist, or the version/file hash didn't match.
            // So we AOB scan in cache.
            Log.Debug("[Core] No valid address record cache found. Performing first-time scan.");

            var scannerWatch = Stopwatch.StartNew();
            Parallel.ForEach(addressRecords, (AddressRecordJson record) =>
            {
                var scanner = new AddressRecord(record.Pattern, record.Offset);
                Records.TryAdd(record.Name, scanner.Address);
            });
            scannerWatch.Stop();

            Log.Debug($"[Core] Scanning for addresses took {scannerWatch.ElapsedMilliseconds}ms");

            // Write cache file
            var cacheJson = JsonSerializer.Serialize(
                new AddressRecordCacheJson
                {
                    Version = gameVersion,
                    AddressRecordFileHash = addressRecordFileHash,
                    Addresses = Records.ToDictionary(e => e.Key, e => (ulong)e.Value),
            }) ?? throw new Exception("Failed to deserialize address records cache");

            File.WriteAllText(AddressCachePath, cacheJson);
        }

        private static unsafe string GetGameRevision(AddressRecordJson[] records)
        {
            // TODO: It's weird to scan for this here.
            // If we get an update, we _probably_ won't be able to get to this point
            // due to hardcoded addresses in the native core.
            // This should AOB scanned in native and exposed as an icall.

            var gameBuildRevisionJsonRecord = records.FirstOrDefault(r => r.Name == "Core::GetGameBuildRevision")
                ?? throw new Exception("Failed to get Core::GetGameBuildRevision from address records");

            var addressRecord = new AddressRecord(gameBuildRevisionJsonRecord.Pattern, gameBuildRevisionJsonRecord.Offset)
                ?? throw new Exception("Failed scan for Core::GetGameBuildRevision");


            // We unfortunately can't call this function directly, as it uses CRT functions
            // that might not have been initialized yet. Instead, we just parse the offset
            // in the instruction to the constant.
            // var getGameBuildRevision = new NativeFunction<nint>(addressRecord.Address);
            var constantOffset = Memory.MemoryUtil.Read<UInt32>(addressRecord.Address+7);
            var constantBase = addressRecord.Address + 11;
            var gameVersionPtr = MemoryUtil.Read<nint>(constantBase + constantOffset);
            var gameVersion = MemoryUtil.ReadString(gameVersionPtr);

            return gameVersion;
        }

        public static nint Get(string name)
        {
           if (Records.TryGetValue(name, out var record)) 
               return record;

           throw new Exception($"Failed to find address for {name}");
        }

        private static readonly ConcurrentDictionary<string, nint> Records = [];

        private static readonly JsonSerializerOptions SerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
    }
}
