#pragma once

#include "FileSystemFile.h"

#include <algorithm>
#include <vector>
#include <memory>
#include <ranges>


struct FileSystemFolder : FileSystemItem {
    std::vector<Ref<FileSystemItem>> Children;

    constexpr bool empty() const { return Children.empty(); }

    auto files() const {
        return Children
            | std::views::filter([](const auto& item) { return std::dynamic_pointer_cast<FileSystemFile>(item) != nullptr; })
            | std::views::transform([](const auto& item) { return std::dynamic_pointer_cast<FileSystemFile>(item); });
    }

    auto folders() const {
        return Children
            | std::views::filter([](const auto& item) { return std::dynamic_pointer_cast<FileSystemFolder>(item) != nullptr; })
            | std::views::transform([](const auto& item) { return std::dynamic_pointer_cast<FileSystemFolder>(item); });
    }

    bool contains(std::string_view name) const {
        return std::ranges::any_of(Children, [name](const auto& item) { return item->Name == name; });
    }

    bool contains_file(std::string_view name) const {
        return std::ranges::any_of(files(), [name](const auto& item) { return item->Name == name; });
    }

    bool contains_folder(std::string_view name) const {
        return std::ranges::any_of(folders(), [name](const auto& item) { return item->Name == name; });
    }

    Ref<FileSystemFile> get_file(std::string_view name) const {
        auto files = this->files();
        auto it = std::ranges::find_if(files, [name](const auto& item) { return item->Name == name; });
        return it != files.end() ? *it : nullptr;
    }

    Ref<FileSystemFolder> get_folder(std::string_view name) const {
        auto folders = this->folders();
        auto it = std::ranges::find_if(folders, [name](const auto& item) { return item->Name == name; });
        return it != folders.end() ? *it : nullptr;
    }

    void add(const Ref<FileSystemItem>& item) {
        Children.emplace_back(std::move(item));
    }

    explicit FileSystemFolder(std::string_view name, const std::vector<Ref<FileSystemItem>>& children = {})
        : FileSystemItem(name), Children(std::move(children)) { }
};
