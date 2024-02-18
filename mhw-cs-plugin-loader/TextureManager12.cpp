#include "TextureManager.h"
#include "HResultHandler.h"

#include <filesystem>

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

void TextureManager::get_texture_dimensions(const ComPtr<ID3D12Resource>& texture, u32* width, u32* height) {
    const D3D12_RESOURCE_DESC desc = texture->GetDesc();
    *width = (u32)desc.Width;
    *height = desc.Height;
}
