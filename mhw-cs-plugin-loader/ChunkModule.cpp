#include "ChunkModule.h"
#include "NativePluginFramework.h"
#include "Config.h"

ChunkModule::ChunkModule() : m_default_chunk(std::make_shared<Chunk>(config::SPL_DEFAULT_CHUNK_PATH)) { }

void ChunkModule::initialize(CoreClr* coreclr) {
    coreclr->add_internal_call("LoadChunk", &ChunkModule::load_chunk_raw);
    coreclr->add_internal_call("GetDefaultChunk", &ChunkModule::get_default_chunk);
    coreclr->add_internal_call("RequestChunk", &ChunkModule::request_chunk_raw);
    coreclr->add_internal_call("ChunkGetFile", &ChunkModule::chunk_get_file);
    coreclr->add_internal_call("ChunkGetFolder", &ChunkModule::chunk_get_folder);
    coreclr->add_internal_call("FileGetContents", &ChunkModule::file_get_contents);
    coreclr->add_internal_call("FileGetSize", &ChunkModule::file_get_size);
}

void ChunkModule::shutdown() { }

void ChunkModule::load_chunk(const std::string& path) {
    const auto chunk_name = path.substr(path.find_last_of('/') + 1)
        .substr(0, path.find_last_of('.'));
    if (m_chunks.contains(chunk_name)) {
        return;
    }

    m_chunks[chunk_name] = std::make_shared<Chunk>(path);
}

Ref<Chunk> ChunkModule::request_chunk(const std::string& name) {
    if (name == "Default") {
        return m_default_chunk;
    }

    return m_chunks[name];
}

void ChunkModule::load_chunk_raw(const char* path) {
    const auto module = NativePluginFramework::get_module<ChunkModule>();
    module->load_chunk(std::string(path));
}

Handle<Chunk> ChunkModule::request_chunk_raw(const char* name) {
    const auto module = NativePluginFramework::get_module<ChunkModule>();
    return module->request_chunk(std::string(name)).get();
}

Handle<Chunk> ChunkModule::get_default_chunk() {
    const auto module = NativePluginFramework::get_module<ChunkModule>();
    return module->m_default_chunk.get();
}

Handle<FileSystemFile> ChunkModule::chunk_get_file(Handle<Chunk> chunk, const char* path) {
    return chunk->get_file(path).get();
}

Handle<FileSystemFolder> ChunkModule::chunk_get_folder(Handle<Chunk> chunk, const char* path) {
    return chunk->get_folder(path).get();
}

u8* ChunkModule::file_get_contents(Handle<FileSystemFile> file) {
    return file->Contents.data();
}

u64 ChunkModule::file_get_size(Handle<FileSystemFile> file) {
    return file->Contents.size();
}
