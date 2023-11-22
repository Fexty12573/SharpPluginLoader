#pragma once
#include <string_view>

struct FileSystemItem {
    std::string Name;

    explicit FileSystemItem(std::string_view name) : Name(name) {}
    virtual ~FileSystemItem() = default;
};

template<typename T> using Ref = std::shared_ptr<T>;
