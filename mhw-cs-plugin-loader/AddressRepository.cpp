#include "AddressRepository.h"

#include <execution>
#include <filesystem>
#include <memory>
#include <string>
#include <chrono>

#include "Chunk.h"
#include "Config.h"
#include "Log.h"
#include "PatternScan.h"

#include <nlohmann/json.hpp>
#include "picosha2/picosha2.h"
using json = nlohmann::json;

std::unordered_map<std::string, uintptr_t> scan_for_address_records(json records_json);
std::string get_game_revision();

void AddressRepository::initialize() {
	// Load address records json from the default chunk
	std::shared_ptr<Chunk> default_chunk = std::make_shared<Chunk>(config::SPL_DEFAULT_CHUNK_PATH);
	auto address_records = default_chunk.get()->get_file("/Resources/AddressRecords.json");
	auto& contents_raw = address_records->Contents;
	std::string contents(contents_raw.begin(), contents_raw.end());

	// Parse the json
	json records_json = json::parse(contents);

	// Get game version/revision and hash of the current address records json file.
	std::string game_revision = get_game_revision();
	if (game_revision.empty()) {
		dlog::debug("[AddressRepo] Failed to get game revision to validate address repository cache. Cache will be disregarded.");
	}

	std::vector<unsigned char> hash(picosha2::k_digest_size);
	picosha2::hash256(contents_raw.begin(), contents_raw.end(), hash.begin(), hash.end());
	std::string address_records_file_hash = picosha2::bytes_to_hex_string(hash.begin(), hash.end());

	dlog::debug("[AddressRepo] Attempting to initialize address repository for game revision: {}", game_revision);

	// Attempt to load file from disk
	if (std::filesystem::exists(config::SPL_ADDRESS_REPOSITORY_CACHE_PATH) && !game_revision.empty()) {
		if (this->restore_cache(game_revision, address_records_file_hash)) {
			dlog::debug("[AddressRepo] Restored from address record cache.");
			return;
		}
	}

	// Either the cache file doesn't exist, or the version/file hash didn't match.
	// So we AOB scan in cache.
	dlog::debug("[AddressRepo] No valid address record cache found. Performing first-time scan.");

	// Scan for the address records
	auto pattern_scan_start_time = std::chrono::steady_clock::now();
	m_address_records = scan_for_address_records(records_json);
	auto pattern_scan_end_time = std::chrono::steady_clock::now();

	dlog::debug(
		"[AddressRepo] Scanning for addresses took: {}ms",
		std::chrono::duration_cast<std::chrono::milliseconds>(pattern_scan_end_time - pattern_scan_start_time).count()
	);

	// Write cache file.
	this->write_cache(game_revision, address_records_file_hash);
	dlog::debug("[AddressRepo] Wrote cache file to disk.");

}

void AddressRepository::write_cache(const std::string& game_version, const std::string& address_records_file_hash) {
	std::ofstream file(config::SPL_ADDRESS_REPOSITORY_CACHE_PATH);
	if (file.is_open())
	{
		json data;
		data["Version"] = game_version;
		data["AddressRecordFileHash"] = address_records_file_hash;
		data["Addresses"] = m_address_records;
		file << std::setw(4) << data << "\n";
		file.close();
	}
}


bool AddressRepository::restore_cache(const std::string& game_version, const std::string& address_records_file_hash) {
	std::ifstream file(config::SPL_ADDRESS_REPOSITORY_CACHE_PATH);
	json address_cache_json = json::parse(file);
	std::string cache_version = address_cache_json["Version"];
	std::string cache_address_record_file_hash = address_cache_json["AddressRecordFileHash"];
	if (cache_version == game_version && cache_address_record_file_hash == address_records_file_hash) {
		auto cached_addresses = address_cache_json["Addresses"];
		m_address_records = cached_addresses;
		return true;
	}

	return false;
}


uintptr_t AddressRepository::get(const std::string& name) {
	if (!m_address_records.contains(name))
		return 0;

	return m_address_records[name];
}

/// <summary>
/// Scans for the patterns in the provided JSON object in parallel.
/// </summary>
std::unordered_map<std::string, uintptr_t> scan_for_address_records(json records_json) {
	std::mutex map_lock;
	std::unordered_map<std::string, uintptr_t> resolved_addresses;
	std::for_each(std::execution::par, records_json.begin(), records_json.end(), [&map_lock, &resolved_addresses](const json& o) {
		std::string name = o["Name"];
		std::string pattern = o["Pattern"];
		int64_t offset = o["Offset"];

		uintptr_t address = PatternScanner::find_first(Pattern::from_string(pattern));
		if (address == 0) {
			dlog::error("[AddressRepo] Failed to find address for: {}", name);
			return;
		}
		address += offset;

		// lock map and insert the scan result.
		{
			std::lock_guard lock(map_lock);
			resolved_addresses[name] = address;
		}
	});
	return resolved_addresses;
}

// TODO: Essentially a duplicate of the same function in NativePluginFramework,
// should be moved somewhere general.
std::string get_game_revision() {
	const auto pattern = Pattern::from_string("48 83 EC 48 48 8B 05 ? ? ? ? 4C 8D 0D ? ? ? ? BA 0A 00 00 00");
	const auto func = PatternScanner::find_first(pattern);

	if (func == 0) {
		dlog::error("[AddressRepo] Failed to find game revision function");
		return std::string();
	}

	const auto constant_offset = *reinterpret_cast<i32*>(func + 7);
	const uintptr_t offset_base = func + 11;

	const char* version = *reinterpret_cast<const char**>(offset_base + constant_offset);
	return version == nullptr ? std::string() : std::string(version);
}

