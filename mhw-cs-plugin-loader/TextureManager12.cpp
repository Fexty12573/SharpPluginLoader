#include "TextureManager.h"
#include "HResultHandler.h"

#include <filesystem>

#include <directxtk12/ResourceUploadBatch.h>
#include <directxtk12/DDSTextureLoader.h>
#include <directxtk12/WICTextureLoader.h>

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
