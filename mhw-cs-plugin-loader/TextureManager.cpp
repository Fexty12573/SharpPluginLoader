#include "TextureManager.h"
#include "HResultHandler.h"

#include <filesystem>

#include <directxtk12/ResourceUploadBatch.h>
#include <directxtk12/DDSTextureLoader.h>
#include <directxtk12/WICTextureLoader.h>
#include <directxtk/DDSTextureLoader.h>
#include <directxtk/WICTextureLoader.h>


TextureManager::TextureManager(ID3D12Device* device, ID3D12CommandQueue* cmd_queue)
    : m_is_d3d12(true), m_device12(device), m_command_queue12(cmd_queue) { }

TextureManager::TextureManager(ID3D11Device* device, ID3D11DeviceContext* context) 
    : m_is_d3d12(false), m_device11(device), m_context11(context) { }

TextureHandle TextureManager::load_texture(std::string_view path) {
    TextureHandle handle;
    TextureEntry entry{
        .Path = std::string(path)
    };

    if (m_is_d3d12) {
        entry.Texture12 = load_texture12(path);
        if (!entry.Texture12) {
            return nullptr;
        }

        handle = (TextureHandle)get_gpu_descriptor_handle(entry).ptr;
    } else {
        entry.Texture11 = load_texture11(path);
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

TextureManager::ComPtr<ID3D11ShaderResourceView> TextureManager::load_texture11(std::string_view path) const {
    namespace fs = std::filesystem;

    const auto file = fs::path(path);
    if (!fs::exists(file)) {
        dlog::error("Failed to load texture: {} does not exist", path);
        return nullptr;
    }

    const auto ext = file.extension().string();
    if (ext == ".dds") {
        ComPtr<ID3D11Resource> texture;
        ComPtr<ID3D11ShaderResourceView> srv;
        HandleResult(DirectX::CreateDDSTextureFromFile(
            m_device11,
            file.c_str(),
            texture.GetAddressOf(),
            srv.GetAddressOf()
        ));

        return srv;
    }

    if (ext == ".png" || ext == ".jpg" || ext == ".jpeg") {
        ComPtr<ID3D11Resource> texture;
        ComPtr<ID3D11ShaderResourceView> srv;

        Microsoft::WRL::Wrappers::RoInitializeWrapper initialize(RO_INIT_MULTITHREADED);
        HandleResult(DirectX::CreateWICTextureFromFile(
            m_device11,
            m_context11,
            file.c_str(),
            texture.GetAddressOf(),
            srv.GetAddressOf()
        ));

        return srv;
    }

    dlog::error("Failed to load texture: unsupported format {}", ext);
    return nullptr;
}

TextureManager::ComPtr<ID3D12Resource> TextureManager::load_texture12(std::string_view path) const {
    namespace fs = std::filesystem;

    const auto file = fs::path(path);
    if (!fs::exists(file)) {
        dlog::error("Failed to load texture: {} does not exist", path);
        return nullptr;
    }

    const auto ext = file.extension().string();
    if (ext == ".dds") {
        ComPtr<ID3D12Resource> texture;
        DirectX::ResourceUploadBatch resource_upload(m_device12);

        HandleResult(DirectX::CreateDDSTextureFromFile(
            m_device12,
            resource_upload,
            file.c_str(),
            texture.GetAddressOf()
        ));

        resource_upload.End(m_command_queue12).wait();

        return texture;
    }

    if (ext == ".png" || ext == ".jpg" || ext == ".jpeg") {
        ComPtr<ID3D12Resource> texture;
        DirectX::ResourceUploadBatch resource_upload(m_device12);

        HandleResult(DirectX::CreateWICTextureFromFile(
            m_device12,
            resource_upload,
            file.c_str(),
            texture.GetAddressOf()
        ));

        resource_upload.End(m_command_queue12).wait();

        return texture;
    }

    dlog::error("Failed to load texture: unsupported format {}", ext);
    return nullptr;
}

D3D12_GPU_DESCRIPTOR_HANDLE TextureManager::get_gpu_descriptor_handle(TextureEntry& entry) {
    if (m_descriptor_heaps.empty()) {
        m_descriptor_heaps.emplace_back(m_device12, DESCRIPTOR_HEAP_SIZE);
        m_next_descriptor_index = 0;
    } else if (m_next_descriptor_index >= DESCRIPTOR_HEAP_SIZE) {
        m_descriptor_heaps.emplace_back(m_device12, DESCRIPTOR_HEAP_SIZE);
        m_next_descriptor_index = 0;
    }

    DescriptorIndex descriptor_index{
        .HeapIndex = (u32)m_descriptor_heaps.size() - 1,
        .Index = m_next_descriptor_index
    };

    if (!m_free_descriptor_indices.empty()) {
        descriptor_index = m_free_descriptor_indices.front();
        m_free_descriptor_indices.pop();
    } else {
        m_next_descriptor_index++;
    }

    auto& heap = m_descriptor_heaps[descriptor_index.HeapIndex];

    // Place the texture in the next available descriptor
    D3D12_SHADER_RESOURCE_VIEW_DESC srv_desc = {};
    srv_desc.Format = entry.Texture12->GetDesc().Format;
    srv_desc.ViewDimension = D3D12_SRV_DIMENSION_TEXTURE2D;
    srv_desc.Shader4ComponentMapping = D3D12_DEFAULT_SHADER_4_COMPONENT_MAPPING;
    srv_desc.Texture2D.MostDetailedMip = 0;
    srv_desc.Texture2D.MipLevels = -1;
    srv_desc.Texture2D.PlaneSlice = 0;
    srv_desc.Texture2D.ResourceMinLODClamp = 0.0f;

    D3D12_CPU_DESCRIPTOR_HANDLE cpu_handle = heap.GetCpuHandle(descriptor_index.Index);
    m_device12->CreateShaderResourceView(entry.Texture12.Get(), &srv_desc, cpu_handle);

    auto handle = heap.GetGpuHandle(descriptor_index.Index);

    entry.DescriptorIndex = descriptor_index;
    m_next_descriptor_index++;

    return handle;
}
