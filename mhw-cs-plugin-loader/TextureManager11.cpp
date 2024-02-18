#include "TextureManager.h"
#include "HResultHandler.h"

#include <filesystem>

#include <directxtk/DDSTextureLoader.h>
#include <directxtk/WICTextureLoader.h>


TextureManager::ComPtr<ID3D11ShaderResourceView> TextureManager::load_texture11(std::string_view path, u32* width, u32* height) const {
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

        if (width && height) {
            get_texture_dimensions(texture, width, height);
        }

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

        if (width && height) {
            get_texture_dimensions(texture, width, height);
        }

        return srv;
    }

    dlog::error("Failed to load texture: unsupported format {}", ext);
    return nullptr;
}

void TextureManager::get_texture_dimensions(const ComPtr<ID3D11Resource>& texture, u32* width, u32* height) {
    D3D11_RESOURCE_DIMENSION dim;
    texture->GetType(&dim);

    if (dim == D3D11_RESOURCE_DIMENSION_TEXTURE2D) {
        ComPtr<ID3D11Texture2D> tex2d;
        HandleResult(texture.As(&tex2d));

        D3D11_TEXTURE2D_DESC desc;
        tex2d->GetDesc(&desc);

        *width = desc.Width;
        *height = desc.Height;
    }
    else {
        dlog::error("Failed to get texture dimensions: unsupported resource type");
    }
}
