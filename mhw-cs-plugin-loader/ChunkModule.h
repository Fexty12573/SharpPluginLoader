#pragma once

#include "NativeModule.h"
#include "Chunk.h"

#include <unordered_map>

template<typename T> using Handle = T*;

class ChunkModule final : public NativeModule {
public:
    ChunkModule();
    void initialize(CoreClr* coreclr) override;
    void shutdown() override;

private:
    void load_chunk(const std::string& path);
    Ref<Chunk> request_chunk(const std::string& name);

    // ChunkManager
    static void load_chunk_raw(const char* path);
    static Handle<Chunk> request_chunk_raw(const char* name);
    static Handle<Chunk> get_default_chunk();

    // Chunk
    static Handle<FileSystemFile> chunk_get_file(Handle<Chunk> chunk, const char* path);
    static Handle<FileSystemFolder> chunk_get_folder(Handle<Chunk> chunk, const char* path);

    // FileSystemFile
    static u8* file_get_contents(Handle<FileSystemFile> file);
    static u64 file_get_size(Handle<FileSystemFile> file);

private:
    Ref<Chunk> m_default_chunk;
    std::unordered_map<std::string, Ref<Chunk>> m_chunks;

#ifdef _DEBUG
    static constexpr const char* DefaultChunkPath = "nativePC/plugins/CSharp/Loader/Default.Debug.bin";
#else
    static constexpr const char* DefaultChunkPath = "nativePC/plugins/CSharp/Loader/Default.bin";
#endif
};



