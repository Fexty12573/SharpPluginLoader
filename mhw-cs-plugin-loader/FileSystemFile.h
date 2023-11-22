#pragma once
#include "FileSystemItem.h"
#include <vector>

struct FileSystemFile : FileSystemItem {
    std::vector<u8> Contents;

    constexpr bool empty() const { return Contents.empty(); }
    std::string_view extension() const { return std::string_view(Name).substr(Name.find_last_of('.')); }
    constexpr size_t size() const { return Contents.size(); }

    FileSystemFile(std::string_view name, const std::vector<uint8_t>& contents)
        : FileSystemItem(name), Contents(contents) { }
};
