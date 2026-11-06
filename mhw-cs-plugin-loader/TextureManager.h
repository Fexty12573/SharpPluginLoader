#pragma once

#include "SharpPluginLoader.h"

#include <vector>
#include <unordered_map>

#include <d3d11.h>
#include <d3d12.h>
#include <memory>
#include <queue>
#include <wrl.h>
#include <directxtk12/DescriptorHeap.h>


typedef void* TextureHandle;

class TextureManager {
    template <typename T> using ComPtr = Microsoft::WRL::ComPtr<T>;

    struct TextureEntry {
        std::string Path;
        ComPtr<ID3D11ShaderResourceView> Texture11 = nullptr;
        ComPtr<ID3D12Resource> Texture12 = nullptr;
        u32 DescriptorIndex;
    };

public:
    explicit TextureManager(ID3D12Device* device, ID3D12CommandQueue* cmd_queue, const ComPtr<ID3D12DescriptorHeap>& heap);
    explicit TextureManager(ID3D11Device* device, ID3D11DeviceContext* context);

    TextureHandle register_texture(void* texture, void* srv_from_game);
    TextureHandle load_texture(std::string_view path, u32* out_width, u32* out_height);
    void unload_texture(TextureHandle handle);

    void update_command_queue(ID3D12CommandQueue* cmd_queue) { m_command_queue12 = cmd_queue; }

    static constexpr u32 DESCRIPTOR_HEAP_SIZE = 1024;

private:
    static bool format_strip_srgb(DXGI_FORMAT& format);

    ComPtr<ID3D11ShaderResourceView> load_texture11(std::string_view path, u32* width, u32* height) const;
    ComPtr<ID3D12Resource> load_texture12(std::string_view path, u32* width, u32* height) const;

    ComPtr<ID3D11Resource> copy_texture11(const ComPtr<ID3D11Resource>& src, const D3D11_TEXTURE2D_DESC& desc) const;
    ComPtr<ID3D12Resource> copy_texture12(const ComPtr<ID3D12Resource>& src, const D3D12_RESOURCE_DESC& desc) const;

    static void get_texture_dimensions(const ComPtr<ID3D11Resource>& texture, u32* width, u32* height);
    static void get_texture_dimensions(const ComPtr<ID3D12Resource>& texture, u32* width, u32* height);
    static D3D11_RESOURCE_DIMENSION get_resource_dimension(const ComPtr<ID3D11Resource>& texture);
    static D3D11_TEXTURE2D_DESC get_desc_2d(const ComPtr<ID3D11Resource>& texture);

    D3D12_GPU_DESCRIPTOR_HANDLE get_gpu_descriptor_handle(TextureEntry& entry, const D3D12_RESOURCE_DESC& desc);

private:
    bool m_is_d3d12;
    bool m_supports_type_cast = false;
    ID3D12Device* m_device12 = nullptr;
    ID3D11Device* m_device11 = nullptr;
    ID3D11DeviceContext* m_context11 = nullptr;
    ID3D12CommandQueue* m_command_queue12 = nullptr;
    std::unique_ptr<DirectX::DescriptorHeap> m_descriptor_heap12;
    std::unordered_map<TextureHandle, TextureEntry> m_textures;

    u32 m_next_descriptor_index = 1; // 0 is reserved for the imgui font texture
    std::queue<u32> m_free_descriptor_indices;
};
