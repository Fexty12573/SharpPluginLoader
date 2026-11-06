#include "TextureManager.h"
#include "HResultHandler.h"

#include <filesystem>

#include <d3dx12.h>
#include <directxtk12/ResourceUploadBatch.h>
#include <directxtk12/DDSTextureLoader.h>
#include <directxtk12/WICTextureLoader.h>

TextureManager::ComPtr<ID3D12Resource> TextureManager::load_texture12(std::string_view path, u32* width, u32* height) const {
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
        resource_upload.Begin();

        HandleResult(DirectX::CreateDDSTextureFromFile(
            m_device12,
            resource_upload,
            file.c_str(),
            texture.GetAddressOf()
        ));

        resource_upload.End(m_command_queue12).wait();

        if (width && height) {
            get_texture_dimensions(texture, width, height);
        }

        return texture;
    }

    if (ext == ".png" || ext == ".jpg" || ext == ".jpeg") {
        ComPtr<ID3D12Resource> texture;
        DirectX::ResourceUploadBatch resource_upload(m_device12);
        resource_upload.Begin();

        HandleResult(DirectX::CreateWICTextureFromFile(
            m_device12,
            resource_upload,
            file.c_str(),
            texture.GetAddressOf()
        ));

        resource_upload.End(m_command_queue12).wait();

        if (width && height) {
            get_texture_dimensions(texture, width, height);
        }

        return texture;
    }

    dlog::error("Failed to load texture: unsupported format {}", ext);
    return nullptr;
}

TextureManager::ComPtr<ID3D12Resource> TextureManager::copy_texture12(const ComPtr<ID3D12Resource>& src, const D3D12_RESOURCE_DESC& desc) const {
    D3D12_HEAP_PROPERTIES heap_properties = {
        .Type = D3D12_HEAP_TYPE_DEFAULT,
        .CPUPageProperty = D3D12_CPU_PAGE_PROPERTY_UNKNOWN,
        .MemoryPoolPreference = D3D12_MEMORY_POOL_UNKNOWN
    };

    ComPtr<ID3D12Resource> texture;
    HandleResult(m_device12->CreateCommittedResource(
        &heap_properties,
        D3D12_HEAP_FLAG_NONE,
        &desc,
        D3D12_RESOURCE_STATE_COPY_DEST,
        nullptr,
        IID_PPV_ARGS(texture.GetAddressOf())
    ));

    ComPtr<ID3D12Fence> fence = nullptr;
    HandleResult(m_device12->CreateFence(0, D3D12_FENCE_FLAG_NONE, IID_PPV_ARGS(fence.GetAddressOf())));

    ComPtr<ID3D12CommandAllocator> command_allocator = nullptr;
    HandleResult(m_device12->CreateCommandAllocator(
        D3D12_COMMAND_LIST_TYPE_DIRECT,
        IID_PPV_ARGS(command_allocator.GetAddressOf())
    ));

    ComPtr<ID3D12GraphicsCommandList> command_list = nullptr;
    HandleResult(m_device12->CreateCommandList(
        0,
        D3D12_COMMAND_LIST_TYPE_DIRECT,
        command_allocator.Get(),
        nullptr,
        IID_PPV_ARGS(command_list.GetAddressOf())
    ));

    command_list->CopyResource(texture.Get(), src.Get());

    D3D12_RESOURCE_BARRIER barrier = {
        .Type = D3D12_RESOURCE_BARRIER_TYPE_TRANSITION,
        .Flags = D3D12_RESOURCE_BARRIER_FLAG_NONE,
        .Transition = {
            .pResource = texture.Get(),
            .Subresource = D3D12_RESOURCE_BARRIER_ALL_SUBRESOURCES,
            .StateBefore = D3D12_RESOURCE_STATE_COPY_DEST,
            .StateAfter = D3D12_RESOURCE_STATE_PIXEL_SHADER_RESOURCE
        }
    };

    command_list->ResourceBarrier(1, &barrier);

    HandleResult(command_list->Close());

    m_command_queue12->ExecuteCommandLists(1, CommandListCast(command_list.GetAddressOf()));
    HandleResult(m_command_queue12->Signal(fence.Get(), 1));

    HANDLE event = CreateEvent(nullptr, FALSE, FALSE, nullptr);
    fence->SetEventOnCompletion(1, event);
    WaitForSingleObject(event, INFINITE);
    CloseHandle(event);

    return texture;
}

void TextureManager::get_texture_dimensions(const ComPtr<ID3D12Resource>& texture, u32* width, u32* height) {
    const D3D12_RESOURCE_DESC desc = texture->GetDesc();
    *width = (u32)desc.Width;
    *height = desc.Height;
}
