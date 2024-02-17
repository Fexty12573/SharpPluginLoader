#include "TextureManager.h"
#include "HResultHandler.h"

#include <filesystem>

#include <directxtk/DDSTextureLoader.h>
#include <directxtk/WICTextureLoader.h>


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

