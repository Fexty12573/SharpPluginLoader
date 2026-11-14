#include "TextureManager.h"
#include "HResultHandler.h"

TextureManager::TextureManager(ID3D12Device* device, ID3D12CommandQueue* cmd_queue, const ComPtr<ID3D12DescriptorHeap>& heap)
    : m_is_d3d12(true), m_device12(device), m_command_queue12(cmd_queue)
    , m_descriptor_heap12(std::make_unique<DirectX::DescriptorHeap>(heap.Get())) {
    D3D12_FEATURE_DATA_D3D12_OPTIONS3 options3{};
    if (SUCCEEDED(m_device12->CheckFeatureSupport(D3D12_FEATURE_D3D12_OPTIONS3, &options3, sizeof(options3)))) {
        m_supports_type_cast = options3.CastingFullyTypedFormatSupported;
    }
}

TextureManager::TextureManager(ID3D11Device* device, const ComPtr<ID3D11DeviceContext>& context)
    : m_is_d3d12(false), m_device11(device), m_context11(context) {}

// https://learn.microsoft.com/en-us/windows/win32/direct3ddxgi/converting-data-color-space
// The _SRGB format modifier causes an sRGB-to-linear conversion of the texture data at
// the point it's sampled in a shader. In our case, ImGui isn't expecting that and
// renders the texture incorrectly (too dark).
bool TextureManager::format_strip_srgb(DXGI_FORMAT& format) {
    switch (format) {
    case DXGI_FORMAT_BC1_UNORM_SRGB:
        format = DXGI_FORMAT_BC1_UNORM;
        break;
    case DXGI_FORMAT_BC7_UNORM_SRGB:
        format = DXGI_FORMAT_BC7_UNORM;
        break;
    case DXGI_FORMAT_R8G8B8A8_UNORM_SRGB:
        format = DXGI_FORMAT_R8G8B8A8_UNORM;
        break;
    default:
        return false;
    }
    return true;
}

TextureHandle TextureManager::register_texture(void* texture, void *srv_from_game) {
    TextureEntry entry;
    TextureHandle handle;

    if (m_is_d3d12) {
        ComPtr<ID3D12Resource> resource = static_cast<ID3D12Resource*>(texture);
        D3D12_RESOURCE_DESC desc = resource->GetDesc();
        if (desc.Dimension != D3D12_RESOURCE_DIMENSION_TEXTURE2D) {
            return nullptr;
        }

        if (format_strip_srgb(desc.Format) && !m_supports_type_cast) {
            entry.Texture12 = copy_texture12(resource, desc);
        } else {
            entry.Texture12 = resource;
        }

        handle = (TextureHandle)get_gpu_descriptor_handle(entry, desc).ptr;
    } else {
        ComPtr<ID3D11Resource> resource = static_cast<ID3D11Resource*>(texture);
        if (get_resource_dimension(resource) != D3D11_RESOURCE_DIMENSION_TEXTURE2D) {
            return nullptr;
        }

        D3D11_TEXTURE2D_DESC desc = get_desc_2d(resource);
        if (format_strip_srgb(desc.Format)) {
            // Textures from the game might have Usage set to D3D11_USAGE_IMMUTABLE.
            desc.Usage = D3D11_USAGE_DEFAULT;
            desc.BindFlags = D3D11_BIND_SHADER_RESOURCE;
            resource = copy_texture11(resource, desc);
        } else if (srv_from_game != nullptr) {
            return srv_from_game;
        }

        ComPtr<ID3D11ShaderResourceView> srv;
        const D3D11_SHADER_RESOURCE_VIEW_DESC srv_desc = {
            .Format = desc.Format,
            .ViewDimension = D3D11_SRV_DIMENSION_TEXTURE2D,
            .Texture2D = {
                .MostDetailedMip = 0,
                .MipLevels = (UINT)-1
            }
        };

        HandleResult(m_device11->CreateShaderResourceView(
            resource.Get(),
            &srv_desc,
            srv.GetAddressOf()
        ));

        entry.Texture11 = srv;
        handle = (TextureHandle)srv.Get();
    }

    dlog::debug("Registered texture: handle {}", handle);

    m_textures.emplace(handle, std::move(entry));
    return handle;
}

TextureHandle TextureManager::load_texture(std::string_view path, u32* out_width, u32* out_height) {
    TextureHandle handle;
    TextureEntry entry{ .Path = std::string(path) };

    if (m_is_d3d12) {
        ComPtr<ID3D12Resource> resource = load_texture12(path, out_width, out_height);
        if (!resource) {
            return nullptr;
        }

        entry.Texture12 = resource;
        handle = (TextureHandle)get_gpu_descriptor_handle(entry, resource->GetDesc()).ptr;
    } else {
        ComPtr<ID3D11ShaderResourceView> srv = load_texture11(path, out_width, out_height);
        if (!srv) {
            return nullptr;
        }

        entry.Texture11 = srv;
        handle = (TextureHandle)srv.Get();
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

D3D12_GPU_DESCRIPTOR_HANDLE TextureManager::get_gpu_descriptor_handle(TextureEntry& entry, const D3D12_RESOURCE_DESC& desc) {
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
    const D3D12_SHADER_RESOURCE_VIEW_DESC srv_desc = {
        .Format = desc.Format,
        .ViewDimension = D3D12_SRV_DIMENSION_TEXTURE2D,
        .Shader4ComponentMapping = D3D12_DEFAULT_SHADER_4_COMPONENT_MAPPING,
        .Texture2D = {
            .MostDetailedMip = 0,
            .MipLevels = (UINT)-1,
            .PlaneSlice = 0,
            .ResourceMinLODClamp = 0.0f
        }
    };

    m_device12->CreateShaderResourceView(
        entry.Texture12.Get(),
        &srv_desc,
        m_descriptor_heap12->GetCpuHandle(descriptor_index)
    );

    entry.DescriptorIndex = descriptor_index;

    return m_descriptor_heap12->GetGpuHandle(descriptor_index);
}
