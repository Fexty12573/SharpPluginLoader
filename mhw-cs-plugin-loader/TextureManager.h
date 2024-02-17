#pragma once

#include "SharpPluginLoader.h"

#include <vector>
#include <unordered_map>

#include <d3d11.h>
#include <d3d12.h>
#include <queue>
#include <wrl.h>
#include <directxtk12/DescriptorHeap.h>


typedef void* TextureHandle;

class TextureManager {
    template <typename T> using ComPtr = Microsoft::WRL::ComPtr<T>;

    struct DescriptorIndex {
        u32 HeapIndex;
        u32 Index;
    };
    
    struct TextureEntry {
        std::string Path;
        ComPtr<ID3D11ShaderResourceView> Texture11 = nullptr;
        ComPtr<ID3D12Resource> Texture12 = nullptr;
        DescriptorIndex DescriptorIndex;
    };

public:
    explicit TextureManager(ID3D12Device* device, ID3D12CommandQueue* cmd_queue);
    explicit TextureManager(ID3D11Device* device, ID3D11DeviceContext* context);

    TextureHandle load_texture(std::string_view path);
    void unload_texture(TextureHandle handle);

    void update_command_queue(ID3D12CommandQueue* cmd_queue) { m_command_queue12 = cmd_queue; }

private:
    ComPtr<ID3D11ShaderResourceView> load_texture11(std::string_view path) const;
    ComPtr<ID3D12Resource> load_texture12(std::string_view path) const;

    D3D12_GPU_DESCRIPTOR_HANDLE get_gpu_descriptor_handle(TextureEntry& entry);

private:
    bool m_is_d3d12;
    ID3D12Device* m_device12 = nullptr;
    ID3D11Device* m_device11 = nullptr;
    ID3D11DeviceContext* m_context11 = nullptr;
    ID3D12CommandQueue* m_command_queue12 = nullptr;
    std::vector<DirectX::DescriptorHeap> m_descriptor_heaps;
    std::unordered_map<TextureHandle, TextureEntry> m_textures;
    u32 m_next_descriptor_index = 0;

    std::queue<DescriptorIndex> m_free_descriptor_indices;

    static constexpr u32 DESCRIPTOR_HEAP_SIZE = 1024;
};

