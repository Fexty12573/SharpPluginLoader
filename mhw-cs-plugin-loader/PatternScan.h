#pragma once

#include "SharpPluginLoader.h"

#include <sstream>
#include <string>
#include <vector>

struct Pattern {
    struct Byte {
        bool IsWildcard = false;
        u8 Value = 0;
    };

    static Pattern from_string(const std::string& pattern);

    const std::vector<Byte>& get_bytes() const {
        return m_bytes;
    }

    Pattern() = delete;

private:
    explicit Pattern(const std::vector<Byte>& bytes) : m_bytes(bytes) {}

    std::vector<Byte> m_bytes;
};

class PatternScanner {
public:
    static std::vector<uintptr_t> scan(const Pattern& pattern);
    static uintptr_t find_first(const Pattern& pattern);
};


