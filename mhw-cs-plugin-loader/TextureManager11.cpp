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

        HandleResult(DirectX::CreateDDSTextureFromFileEx(
            m_device11,
            file.c_str(),
            0,
            D3D11_USAGE_DEFAULT, D3D11_BIND_SHADER_RESOURCE, 0, 0,
            DirectX::DDS_LOADER_IGNORE_SRGB,
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

        HandleResult(DirectX::CreateWICTextureFromFileEx(
            m_device11,
            m_context11.Get(),
            file.c_str(),
            0,
            D3D11_USAGE_DEFAULT, D3D11_BIND_SHADER_RESOURCE, 0, 0,
            DirectX::WIC_LOADER_IGNORE_SRGB,
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

TextureManager::ComPtr<ID3D11Resource> TextureManager::copy_texture11(const ComPtr<ID3D11Resource>& src, const D3D11_TEXTURE2D_DESC& desc) const {
    ComPtr<ID3D11Texture2D> texture;
    HandleResult(m_device11->CreateTexture2D(
        &desc,
        nullptr,
        texture.GetAddressOf()
    ));

    m_context11->CopyResource(texture.Get(), src.Get());

    return texture;
}

D3D11_RESOURCE_DIMENSION TextureManager::get_resource_dimension(const ComPtr<ID3D11Resource>& texture) {
    D3D11_RESOURCE_DIMENSION dimension;
    texture->GetType(&dimension);
    return dimension;
}

D3D11_TEXTURE2D_DESC TextureManager::get_desc_2d(const ComPtr<ID3D11Resource>& texture) {
    ComPtr<ID3D11Texture2D> texture2d;
    HandleResult(texture.As(&texture2d));

    D3D11_TEXTURE2D_DESC desc;
    texture2d->GetDesc(&desc);

    return desc;
}

void TextureManager::get_texture_dimensions(const ComPtr<ID3D11Resource>& texture, u32* width, u32* height) {
    switch (get_resource_dimension(texture)) {
    case D3D11_RESOURCE_DIMENSION_TEXTURE2D: {
        D3D11_TEXTURE2D_DESC desc = get_desc_2d(texture);
        *width = desc.Width;
        *height = desc.Height;
        break;
    }
    default:
        dlog::error("Failed to get texture dimensions: unsupported resource type");
        break;
    }
}
