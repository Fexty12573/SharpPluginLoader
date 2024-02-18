#include "TextureManager.h"
#include "HResultHandler.h"

TextureManager::TextureManager(ID3D12Device* device, ID3D12CommandQueue* cmd_queue, const ComPtr<ID3D12DescriptorHeap>& heap)
    : m_is_d3d12(true), m_device12(device), m_command_queue12(cmd_queue)
    , m_descriptor_heap12(std::make_unique<DirectX::DescriptorHeap>(heap.Get())) {}

TextureManager::TextureManager(ID3D11Device* device, ID3D11DeviceContext* context) 
    : m_is_d3d12(false), m_device11(device), m_context11(context) {}

TextureHandle TextureManager::load_texture(std::string_view path, u32* out_width, u32* out_height) {
    TextureHandle handle;
    TextureEntry entry{
        .Path = std::string(path)
    };

    if (m_is_d3d12) {
        entry.Texture12 = load_texture12(path, out_width, out_height);
        if (!entry.Texture12) {
            return nullptr;
        }

        handle = (TextureHandle)get_gpu_descriptor_handle(entry).ptr;
    } else {
        entry.Texture11 = load_texture11(path, out_width, out_height);
        if (!entry.Texture11) {
            return nullptr;
        }

        handle = (TextureHandle)entry.Texture11.Get();
    }

    dlog::debug("Loaded texture: handle {}, path {}", handle, path);

    m_textures.emplace(handle, std::move(entry));
    return handle;
}

void TextureManager::unload_texture(TextureHandle handle) {
    const auto it = m_textures.find(handle);
    if (it == m_textures.end()) {
        dlog::error("Failed to unload texture: handle {} does not exist", handle);
        return;
    }

    // Free the descriptor index
    if (m_is_d3d12) {
        m_free_descriptor_indices.push(it->second.DescriptorIndex);
    }

    m_textures.erase(it);

    dlog::debug("Unloaded texture: handle {}", handle);
}

D3D12_GPU_DESCRIPTOR_HANDLE TextureManager::get_gpu_descriptor_handle(TextureEntry& entry) {
    if (m_next_descriptor_index >= DESCRIPTOR_HEAP_SIZE && m_free_descriptor_indices.empty()) {
        dlog::error("Failed to get GPU descriptor handle: descriptor heap is full");
        return { 0 };
    }

    u32 descriptor_index;
    if (m_free_descriptor_indices.empty()) {
        descriptor_index = m_next_descriptor_index++;
    } else {
        descriptor_index = m_free_descriptor_indices.front();
        m_free_descriptor_indices.pop();
    }

    // Place the texture in the next available descriptor
    D3D12_SHADER_RESOURCE_VIEW_DESC srv_desc = {};
    srv_desc.Format = entry.Texture12->GetDesc().Format;
    srv_desc.ViewDimension = D3D12_SRV_DIMENSION_TEXTURE2D;
    srv_desc.Shader4ComponentMapping = D3D12_DEFAULT_SHADER_4_COMPONENT_MAPPING;
    srv_desc.Texture2D.MostDetailedMip = 0;
    srv_desc.Texture2D.MipLevels = -1;
    srv_desc.Texture2D.PlaneSlice = 0;
    srv_desc.Texture2D.ResourceMinLODClamp = 0.0f;

    D3D12_CPU_DESCRIPTOR_HANDLE cpu_handle = m_descriptor_heap12->GetCpuHandle(descriptor_index);
    m_device12->CreateShaderResourceView(entry.Texture12.Get(), &srv_desc, cpu_handle);

    auto handle = m_descriptor_heap12->GetGpuHandle(descriptor_index);
    entry.DescriptorIndex = descriptor_index;

    return handle;
}
