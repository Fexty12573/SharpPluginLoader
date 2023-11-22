#pragma once
#include "SharpPluginLoader.h"

#include "FileSystemItem.h"
#include "FileSystemFile.h"
#include "FileSystemFolder.h"

#include <fstream>
#include <istream>
#include <sstream>
#include <type_traits>

#define MAGIC_EQUALS(a, b) ((a)[0] == (b)[0] && (a)[1] == (b)[1] && (a)[2] == (b)[2] && (a)[3] == (b)[3])

template<typename T> T read(std::istream& stream, i64 count = 1);
template<typename T> void write(std::ostream& stream, const T& value);

class Chunk {
public:
    explicit Chunk(Ref<FileSystemFolder>&& root) : m_root(std::move(root)) {}
    explicit Chunk(std::string_view path) {
        std::ifstream stream(path.data(), std::ios::binary);

        const auto magic = read<std::string>(stream, 4);
        const auto version = read<u32>(stream);
        const auto root_offset = read<i64>(stream);

        if (!MAGIC_EQUALS(magic, Magic)) {
            throw std::runtime_error("Invalid magic");
        }

        if (version != Version) {
            throw std::runtime_error("Invalid version");
        }

        stream.seekg(root_offset, std::ios::beg);

        m_root = std::make_shared<FileSystemFolder>(read_folder(stream));
    }

    Ref<FileSystemFile> get_file(const std::string& path) const {
        const auto folder_path = path.substr(0, path.find_last_of('/'));
        const auto file_name = path.substr(path.find_last_of('/') + 1);
        const auto folder = get_folder(folder_path);

        return folder->get_file(file_name);
    }

    Ref<FileSystemFolder> get_folder(const std::string& path) const {
        const std::string trimmed = path.starts_with('/') ? path.substr(1) : path;
        std::stringstream ss(trimmed.data());
        Ref<FileSystemFolder> current = m_root;

        std::string part;
        while (std::getline(ss, part, '/')) {
            current = current->get_folder(part);
            if (!current) {
                throw std::runtime_error("Invalid path");
            }
        }

        return current;
    }

private:
    static Ref<FileSystemItem> read_item(std::istream& stream) {
        enum class ItemType : i8 { File = 0, Folder = 1 };
        const auto type = read<ItemType>(stream);

        switch (type) {
        case ItemType::File:
            return std::make_shared<FileSystemFile>(read_file(stream));
        case ItemType::Folder:
            return std::make_shared<FileSystemFolder>(read_folder(stream));
        }

        return nullptr;
    }

    static FileSystemFile read_file(std::istream& stream) {
        const auto contents_length = read<i32>(stream);
        const auto name_length = read<u16>(stream);
        const auto name = read<std::string>(stream, name_length);

        return { name, read<std::vector<u8>>(stream, contents_length) };
    }

    static FileSystemFolder read_folder(std::istream& stream) {
        const auto children_count = read<i16>(stream);
        const auto name_length = read<u16>(stream);
        const auto name = read<std::string>(stream, name_length);

        FileSystemFolder folder(name);

        for (i16 i = 0; i < children_count; ++i) {
            folder.add(read_item(stream));
        }

        return folder;
    }

private:
    Ref<FileSystemFolder> m_root;

    static constexpr const char* Magic = "bin\0";
    static constexpr u32 Version = 0x20230611;
};

template<typename T> T read(std::istream& stream, i64 count) {
    if constexpr (std::is_same_v<T, std::string>) {
        std::string str(count, '\0');
        stream.read(str.data(), count);
        return str;
    } else if constexpr (std::is_same_v<T, std::vector<u8>>) {
        std::vector<u8> vec(count);
        stream.read(reinterpret_cast<char*>(vec.data()), count);
        return vec;
    }

    T value;
    stream.read(reinterpret_cast<char*>(&value), sizeof(T));
    return value;
}

template<typename T> void write(std::ostream& stream, const T& value) {
    if constexpr (std::is_same_v<T, std::string>) {
        stream.write(value.data(), value.size());
        return;
    } else if constexpr (std::is_same_v<T, std::vector<u8>>) {
        stream.write(reinterpret_cast<const char*>(value.data()), value.size());
        return;
    }

    stream.write(reinterpret_cast<const char*>(&value), sizeof T);
}
