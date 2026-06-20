#pragma once

#include <unordered_map>
#include <string>

class AddressRepository
{
public:
    AddressRepository(): m_address_records() {}

    /// <summary>
    /// Loads all the patterns from the address repo JSON (in filechunk) and resolves them.
    /// If a valid cache is on disk, it will use that instead of pattern scanning for the addresses.
    /// </summary>
    void initialize();

    /// <summary>
    /// Get the game revision and cache it for future calls.
    ///
    /// Returns "unknown" on error, never null.
    /// </summary>
    const char* get_game_revision();

    /// <summary>
    /// Gets the address for the given pattern name.
    ///
    /// Returns 0 if not found.
    /// </summary>
    uintptr_t get(const std::string& name);

private:
    /// <summary>
    /// Writes the currently resolved address records to the on-disk cache file.
    /// </summary>
    void write_cache(const std::string& game_version, const std::string& address_records_file_hash);

    /// <summary>
    /// Restores the resolved address cache from disk.
    ///
    /// Returns true if successful.
    /// </summary>
    bool restore_cache(const std::string& game_version, const std::string& address_records_file_hash);

private:
    const char* m_game_revision = nullptr;
    static constexpr const char* UNKNOWN_REVISION = "unknown";
    std::unordered_map<std::string, uintptr_t> m_address_records;
};
