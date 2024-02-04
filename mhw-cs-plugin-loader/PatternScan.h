#pragma once

#include "SharpPluginLoader.h"

#include <sstream>
#include <string>
#include <vector>

struct Pattern {
    static Pattern from_string(const std::string& pattern);

    const std::vector<i16>& get_bytes() const {
        return m_bytes;
    }

    Pattern() = delete;

private:
    explicit Pattern(const std::vector<i16>& bytes) : m_bytes(bytes) {}

    std::vector<i16> m_bytes;
};

class PatternScanner {
public:
    static std::vector<uintptr_t> scan(const Pattern& pattern);
    static uintptr_t find_first(const Pattern& pattern);
};


