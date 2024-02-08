#include "PatternScan.h"

#include <algorithm>
#include <Windows.h>
#include <Psapi.h>

Pattern Pattern::from_string(const std::string& pattern) {
    std::vector<Byte> bytes;
    std::stringstream ss{ pattern };
    std::string byte;

    while (ss.good()) {
        std::getline(ss, byte, ' ');

        if (byte.empty()) {
            continue;
        }

        if (byte == "?" || byte == "??") {
            bytes.push_back({ true });
        }
        else {
            bytes.push_back({ .Value = (u8)std::stoul(byte, nullptr, 16) });
        }
    }

    return Pattern(bytes);
}

std::vector<uintptr_t> PatternScanner::scan(const Pattern& pattern) {
    std::vector<uintptr_t> results;

    const auto module = GetModuleHandleA(nullptr); // MonsterHunterWorld.exe
    if (!module) {
        return results;
    }

    MODULEINFO module_info;
    if (!GetModuleInformation(GetCurrentProcess(), module, &module_info, sizeof(module_info))) {
        return results;
    }

    auto addr = (u8*)module;
    const auto end_addr = addr + module_info.SizeOfImage;
    const auto& bytes = pattern.get_bytes();

    const auto predicate = [](u8 byte, Pattern::Byte pattern_byte) {
        return pattern_byte.IsWildcard || pattern_byte.Value == byte;
    };

    while (addr < end_addr) {
        MEMORY_BASIC_INFORMATION mbi;
        if (!VirtualQuery(addr, &mbi, sizeof(mbi)) || 
            mbi.State != MEM_COMMIT ||
            mbi.Protect & PAGE_GUARD) {
            break;
        }

        const auto begin = (u8*)mbi.BaseAddress;
        const auto end = begin + mbi.RegionSize;

        auto found = std::search(begin, end, bytes.begin(), bytes.end(), predicate);

        while (found != end) {
            results.push_back((uintptr_t)found);
            found = std::search(found + 1, end, bytes.begin(), bytes.end(), predicate);
        }

        addr = end;
    }

    return results;
}

uintptr_t PatternScanner::find_first(const Pattern& pattern) {
    std::vector<uintptr_t> results;

    const auto module = GetModuleHandleA(nullptr); // MonsterHunterWorld.exe
    if (!module) {
        return 0;
    }

    MODULEINFO module_info;
    if (!GetModuleInformation(GetCurrentProcess(), module, &module_info, sizeof(module_info))) {
        return 0;
    }

    auto addr = (u8*)module;
    const auto end_addr = addr + module_info.SizeOfImage;
    const auto& bytes = pattern.get_bytes();

    const auto predicate = [](u8 byte, Pattern::Byte pattern_byte) {
        return pattern_byte.IsWildcard || pattern_byte.Value == byte;
    };

    while (addr < end_addr) {
        MEMORY_BASIC_INFORMATION mbi;
        if (!VirtualQuery(addr, &mbi, sizeof(mbi)) ||
            mbi.State != MEM_COMMIT ||
            mbi.Protect & PAGE_GUARD) {
            break;
        }

        const auto begin = (u8*)mbi.BaseAddress;
        const auto end = begin + mbi.RegionSize;

        auto found = std::search(begin, end, bytes.begin(), bytes.end(), predicate);

        if (found != end) {
            return (uintptr_t)found;
        }

        addr = end;
    }

    return 0;
}
