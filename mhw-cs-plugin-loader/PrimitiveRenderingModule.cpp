#include "PrimitiveRenderingModule.h"

#include <d3dcompiler.h>
#include <strstream>

#include "HResultHandler.h"
#include "D3DModule.h"

#include <tiny_obj_loader.h>
#include <dti/sMhCamera.h>

#include "ChunkModule.h"
#include "NativePluginFramework.h"

PrimitiveRenderingModule::PrimitiveRenderingModule() = default;

void PrimitiveRenderingModule::initialize(CoreClr* coreclr) {
    coreclr->add_internal_call("RenderSphere", render_sphere_api);
    coreclr->add_internal_call("RenderObb", render_obb_api);
    coreclr->add_internal_call("RenderCapsule", render_capsule_api);
}

void PrimitiveRenderingModule::shutdown() {
}

void PrimitiveRenderingModule::late_init(D3DModule* d3dmodule) {
    if (D3DModule::is_d3d12()) {
        late_init_d3d12(d3dmodule);
    } else {
        late_init_d3d11(d3dmodule);
    }
}

void PrimitiveRenderingModule::render_sphere(const MtSphere& sphere, MtVector4 color) {
    m_spheres.emplace_back(sphere, color);
}

void PrimitiveRenderingModule::render_obb(const MtOBB& obb, MtVector4 color) {
    m_cubes.emplace_back(obb, color);
}

void PrimitiveRenderingModule::render_capsule(const MtCapsule& capsule, MtVector4 color) {
    m_capsules.emplace_back(capsule, color);
}

void PrimitiveRenderingModule::render_primitives_for_d3d11(ID3D11DeviceContext* context) {
    using namespace DirectX;

    if (m_spheres.empty() && m_cubes.empty() && m_capsules.empty()) {
        return;
    }

    // Set up common pipeline state
    ComPtr<ID3D11RenderTargetView> rtv;
    context->OMGetRenderTargets(1, rtv.GetAddressOf(), nullptr);

    context->IASetPrimitiveTopology(D3D11_PRIMITIVE_TOPOLOGY_LINELIST);
    context->IASetInputLayout(m_d3d11_input_layout.Get());
    context->VSSetShader(m_d3d11_vertex_shader.Get(), nullptr, 0);
    context->PSSetShader(m_d3d11_pixel_shader.Get(), nullptr, 0);
    context->RSSetState(m_d3d11_rasterizer_state.Get());
    context->OMSetDepthStencilState(m_d3d11_depth_stencil_state.Get(), 0);
    context->OMSetRenderTargets(1, rtv.GetAddressOf(), m_d3d11_depth_stencil_view.Get());
    context->OMSetBlendState(m_d3d11_blend_state.Get(), nullptr, 0xFFFFFFFF);

    // Store VP data
    const auto camera = sMhCamera::get();
    const ViewProj viewproj = {
        .View = XMMatrixTranspose(XMMATRIX(camera->mViewports[0].mViewMat.ptr())),
        .Proj = XMMatrixTranspose(XMMATRIX(camera->mViewports[0].mProjMat.ptr()))
    };

    D3D11_MAPPED_SUBRESOURCE msr{};
    HandleResult(context->Map(m_d3d11_viewproj_buffer.Get(), 0, D3D11_MAP_WRITE_DISCARD, 0, &msr));

    std::memcpy(msr.pData, &viewproj, sizeof viewproj);
    context->Unmap(m_d3d11_viewproj_buffer.Get(), 0);

    // Set up VP constant buffer
    context->VSSetConstantBuffers(0, 1, m_d3d11_viewproj_buffer.GetAddressOf());

    // Common variables
    int i = 0;
    constexpr std::array<u32, 2> strides = {
        sizeof(Vertex),
        sizeof(Instance)
    };
    constexpr std::array<u32, 2> offsets = { 0, 0 };
    std::array<ID3D11Buffer*, 2> buffers{
        nullptr,
        m_d3d11_transform_buffer.Get()
    };

    // Spheres ------------------------------
    if (!m_spheres.empty()) {
        // Build Instance Data
        for (const auto& sphere : m_spheres) {
            const XMMATRIX scale = XMMatrixScaling(sphere.sphere.r, sphere.sphere.r, sphere.sphere.r);
            const XMMATRIX translation = XMMatrixTranslation(sphere.sphere.pos.x, sphere.sphere.pos.y, sphere.sphere.pos.z);

            m_instances[i++] = {
                .Transform = XMMatrixTranspose(scale * translation),
                .Color = { sphere.color.r, sphere.color.g, sphere.color.b, sphere.color.a }
            };

            if (i >= MAX_INSTANCES) {
                break;
            }
        }

        // Copy Instance Data to GPU
        HandleResult(context->Map(m_d3d11_transform_buffer.Get(), 0, D3D11_MAP_WRITE_DISCARD, 0, &msr));

        std::memcpy(msr.pData, m_instances.data(), sizeof(Instance) * i);
        context->Unmap(m_d3d11_transform_buffer.Get(), 0);

        // Set up pipeline
        context->IASetIndexBuffer(m_d3d11_sphere.IndexBuffer.Get(), DXGI_FORMAT_R32_UINT, 0);

        buffers[0] = m_d3d11_sphere.VertexBuffer.Get();

        context->IASetVertexBuffers(0, buffers.size(), buffers.data(), strides.data(), offsets.data());
        context->DrawIndexedInstanced(
            m_d3d11_sphere.IndexCount,
            i, 0, 0, 0
        );
    }

    // OBBs ---------------------------------
    if (!m_cubes.empty()) {
        // Build Instance Data
        i = 0;
        for (const auto& cube : m_cubes) {
            const XMMATRIX scale = XMMatrixScaling(cube.obb.extent.x, cube.obb.extent.y, cube.obb.extent.z);
            const XMMATRIX translation_rotation{ cube.obb.coord.ptr() };

            m_instances[i++] = {
                .Transform = XMMatrixTranspose(scale * translation_rotation),
                .Color = { cube.color.r, cube.color.g, cube.color.b, cube.color.a }
            };

            if (i >= MAX_INSTANCES) {
                break;
            }
        }

        // Copy Instance Data to GPU
        HandleResult(context->Map(m_d3d11_transform_buffer.Get(), 0, D3D11_MAP_WRITE_DISCARD, 0, &msr));

        std::memcpy(msr.pData, m_instances.data(), sizeof(Instance) * i);
        context->Unmap(m_d3d11_transform_buffer.Get(), 0);

        // Set up pipeline
        context->IASetIndexBuffer(m_d3d11_cube.IndexBuffer.Get(), DXGI_FORMAT_R32_UINT, 0);

        buffers[0] = m_d3d11_cube.VertexBuffer.Get();

        context->IASetVertexBuffers(0, buffers.size(), buffers.data(), strides.data(), offsets.data());
        context->DrawIndexedInstanced(
            m_d3d11_cube.IndexCount,
            i, 0, 0, 0
        );
    }

    // Capsules -----------------------------
    if (!m_capsules.empty()) {
        // Build Instance Data
        i = 0;
        for (const auto& capsule : m_capsules) {
            // Translation
            XMVECTOR p0{ capsule.capsule.p0.x, capsule.capsule.p0.y, capsule.capsule.p0.z, 1.0f };
            XMVECTOR p1{ capsule.capsule.p1.x, capsule.capsule.p1.y, capsule.capsule.p1.z, 1.0f };
            if (XMVectorGetY(p0) > XMVectorGetY(p1)) {
                std::swap(p0, p1);
            }

            const XMMATRIX translation_htop = XMMatrixTranslationFromVector(p1);
            const XMMATRIX translation_hbottom = XMMatrixTranslationFromVector(p0);
            const XMMATRIX translation_cylinder = XMMatrixTranslationFromVector((p0 + p1) * 0.5f);

            // Rotation
            // if (ez := {0,1,0}) == (ev := P2-P1/norm(P2-P1)) -> R = Id
            // if (ez := {0,1,0}) == (ev := P1-P2/norm(P2-P1)) -> R = Id (but swap which cap you use for each end)
            // else
            // v = ez x ev
            // s = norm(ev)
            // c = ez . ev
            // R = Id + [v]x + [v]^2x 1/(1+c)
            XMMATRIX rotation = XMMatrixIdentity();
            XMVECTOR ez = XMVectorSet(0, 1, 0, 0); // Y-up coordinate system
            XMVECTOR ev = XMVector3Normalize(XMVectorSubtract(p1, p0));

            // Check if ev is parallel or anti-parallel to ez
            if (XMVector3Equal(ev, ez) || XMVector3Equal(ev, XMVectorNegate(ez))) {
                // No rotation needed
                rotation = XMMatrixIdentity();
            }
            else {
                // Compute the rotation matrix
                const XMVECTOR v = XMVector3Cross(ez, ev);
                const float c = XMVectorGetX(XMVector3Dot(ez, ev));

                const XMMATRIX vx = { 0, XMVectorGetZ(v), XMVectorGetY(v), 0,
                                -XMVectorGetZ(v), 0, XMVectorGetX(v), 0,
                                -XMVectorGetY(v), -XMVectorGetX(v), 0, 0,
                                0, 0, 0, 0 };
                const XMMATRIX vx2 = XMMatrixMultiply(vx, vx);
                
                rotation = XMMatrixAdd(XMMatrixAdd(XMMatrixIdentity(), vx), vx2 * (1.0f / (1.0f + c)));
            }

            // Scale
            const XMMATRIX scale_hemisphere = XMMatrixScaling(capsule.capsule.r, capsule.capsule.r, capsule.capsule.r);
            const XMMATRIX scale_cylinder = XMMatrixScaling(
                capsule.capsule.r, 
                XMVectorGetX(XMVector3Length(p1 - p0)) * 0.5f, 
                capsule.capsule.r
            );

            m_instances[i] = {
                .Transform = XMMatrixTranspose(scale_cylinder * rotation * translation_cylinder),
                .Color = { capsule.color.r, capsule.color.g, capsule.color.b, capsule.color.a }
            };
            m_instances_hemisphere_top[i] = {
                .Transform = XMMatrixTranspose(scale_hemisphere * rotation * translation_htop),
                .Color = { capsule.color.r, capsule.color.g, capsule.color.b, capsule.color.a }
            };
            m_instances_hemisphere_bottom[i] = {
                .Transform = XMMatrixTranspose(scale_hemisphere * rotation * translation_hbottom),
                .Color = { capsule.color.r, capsule.color.g, capsule.color.b, capsule.color.a }
            };

            i += 1;
            if (i >= MAX_INSTANCES) {
                break;
            }
        }

        // Top Hemisphere
        HandleResult(context->Map(m_d3d11_htop_transform_buffer.Get(), 0, D3D11_MAP_WRITE_DISCARD, 0, &msr));

        std::memcpy(msr.pData, m_instances_hemisphere_top.data(), sizeof(Instance) * i);
        context->Unmap(m_d3d11_htop_transform_buffer.Get(), 0);

        context->IASetIndexBuffer(m_d3d11_hemisphere_top.IndexBuffer.Get(), DXGI_FORMAT_R32_UINT, 0);

        buffers[0] = m_d3d11_hemisphere_top.VertexBuffer.Get();
        buffers[1] = m_d3d11_htop_transform_buffer.Get();
        context->IASetVertexBuffers(0, buffers.size(), buffers.data(), strides.data(), offsets.data());
        context->DrawIndexedInstanced(
            m_d3d11_hemisphere_top.IndexCount,
            i, 0, 0, 0
        );

        // Bottom Hemisphere
        HandleResult(context->Map(m_d3d11_hbottom_transform_buffer.Get(), 0, D3D11_MAP_WRITE_DISCARD, 0, &msr));

        std::memcpy(msr.pData, m_instances_hemisphere_bottom.data(), sizeof(Instance) * i);
        context->Unmap(m_d3d11_hbottom_transform_buffer.Get(), 0);

        context->IASetIndexBuffer(m_d3d11_hemisphere_bottom.IndexBuffer.Get(), DXGI_FORMAT_R32_UINT, 0);

        buffers[0] = m_d3d11_hemisphere_bottom.VertexBuffer.Get();
        buffers[1] = m_d3d11_hbottom_transform_buffer.Get();
        context->IASetVertexBuffers(0, buffers.size(), buffers.data(), strides.data(), offsets.data());
        context->DrawIndexedInstanced(
            m_d3d11_hemisphere_bottom.IndexCount,
            i, 0, 0, 0
        );

        // Cylinder
        HandleResult(context->Map(m_d3d11_transform_buffer.Get(), 0, D3D11_MAP_WRITE_DISCARD, 0, &msr));

        std::memcpy(msr.pData, m_instances.data(), sizeof(Instance) * i);
        context->Unmap(m_d3d11_transform_buffer.Get(), 0);

        context->IASetIndexBuffer(m_d3d11_cylinder.IndexBuffer.Get(), DXGI_FORMAT_R32_UINT, 0);

        buffers[0] = m_d3d11_cylinder.VertexBuffer.Get();
        buffers[1] = m_d3d11_transform_buffer.Get();
        context->IASetVertexBuffers(0, buffers.size(), buffers.data(), strides.data(), offsets.data());
        context->DrawIndexedInstanced(
            m_d3d11_cylinder.IndexCount,
            i, 0, 0, 0
        );
    }

    m_spheres.clear();
    m_cubes.clear();
    m_capsules.clear();
}

void PrimitiveRenderingModule::render_primitives_for_d3d12() {
}

void PrimitiveRenderingModule::late_init_d3d11(D3DModule* d3dmodule) {
    load_mesh_d3d11(d3dmodule->m_d3d11_device, "/Resources/Sphere.obj", m_d3d11_sphere);
    load_mesh_d3d11(d3dmodule->m_d3d11_device, "/Resources/Cube.obj", m_d3d11_cube);
    load_mesh_d3d11(d3dmodule->m_d3d11_device, "/Resources/Hemisphere.obj", m_d3d11_hemisphere_top);
    load_mesh_d3d11(d3dmodule->m_d3d11_device, "/Resources/BottomHemisphere.obj", m_d3d11_hemisphere_bottom);
    load_mesh_d3d11(d3dmodule->m_d3d11_device, "/Resources/Cylinder.obj", m_d3d11_cylinder);

    // Create ViewProj Constant Buffer
    D3D11_BUFFER_DESC bd{};
    bd.ByteWidth = sizeof ViewProj;
    bd.Usage = D3D11_USAGE_DYNAMIC;
    bd.BindFlags = D3D11_BIND_CONSTANT_BUFFER;
    bd.CPUAccessFlags = D3D11_CPU_ACCESS_WRITE;
    bd.StructureByteStride = sizeof ViewProj;

    HandleResult(d3dmodule->m_d3d11_device->CreateBuffer(&bd, nullptr, m_d3d11_viewproj_buffer.GetAddressOf()));

    bd.ByteWidth = sizeof Instance * MAX_INSTANCES;
    bd.Usage = D3D11_USAGE_DYNAMIC;
    bd.BindFlags = D3D11_BIND_VERTEX_BUFFER;
    bd.CPUAccessFlags = D3D11_CPU_ACCESS_WRITE;
    bd.StructureByteStride = sizeof Instance;

    HandleResult(d3dmodule->m_d3d11_device->CreateBuffer(&bd, nullptr, m_d3d11_transform_buffer.GetAddressOf()));
    HandleResult(d3dmodule->m_d3d11_device->CreateBuffer(&bd, nullptr, m_d3d11_htop_transform_buffer.GetAddressOf()));
    HandleResult(d3dmodule->m_d3d11_device->CreateBuffer(&bd, nullptr, m_d3d11_hbottom_transform_buffer.GetAddressOf()));

    ComPtr<ID3DBlob> vs_blob;
    ComPtr<ID3DBlob> ps_blob;

    const auto& chunk = NativePluginFramework::get_module<ChunkModule>()->request_chunk("Default");
    const auto& vs = chunk->get_file("/Resources/PrimitiveRenderingVS.hlsl");
    const auto& ps = chunk->get_file("/Resources/PrimitiveRenderingPS.hlsl");

    const auto load = [&](const Ref<FileSystemFile>& file, const char* target, ComPtr<ID3DBlob>& blob) {
        HandleResult(D3DCompile(
            file->Contents.data(),
            file->size(),
            nullptr,
            nullptr,
            nullptr,
            "main",
            target,
            D3DCOMPILE_DEBUG,
            0,
            blob.GetAddressOf(),
            nullptr
        ));
    };

    load(vs, "vs_5_0", vs_blob);
    load(ps, "ps_5_0", ps_blob);

    HandleResult(d3dmodule->m_d3d11_device->CreateVertexShader(
        vs_blob->GetBufferPointer(),
        vs_blob->GetBufferSize(),
        nullptr,
        m_d3d11_vertex_shader.GetAddressOf()
    ));

    HandleResult(d3dmodule->m_d3d11_device->CreatePixelShader(
        ps_blob->GetBufferPointer(),
        ps_blob->GetBufferSize(),
        nullptr,
        m_d3d11_pixel_shader.GetAddressOf()
    ));

    constexpr D3D11_INPUT_ELEMENT_DESC input_element_desc[] = {
        // Per Vertex (Buffer 1)
        {"POSITION", 0, DXGI_FORMAT_R32G32B32A32_FLOAT, 0, 0, D3D11_INPUT_PER_VERTEX_DATA, 0},
        // Per Instance (Buffer 2)
        {"TRANSFORM", 0, DXGI_FORMAT_R32G32B32A32_FLOAT, 1, 0, D3D11_INPUT_PER_INSTANCE_DATA, 1},
        {"TRANSFORM", 1, DXGI_FORMAT_R32G32B32A32_FLOAT, 1, 16, D3D11_INPUT_PER_INSTANCE_DATA, 1},
        {"TRANSFORM", 2, DXGI_FORMAT_R32G32B32A32_FLOAT, 1, 32, D3D11_INPUT_PER_INSTANCE_DATA, 1},
        {"TRANSFORM", 3, DXGI_FORMAT_R32G32B32A32_FLOAT, 1, 48, D3D11_INPUT_PER_INSTANCE_DATA, 1},
        {"COLOR", 0, DXGI_FORMAT_R32G32B32A32_FLOAT, 1, 64, D3D11_INPUT_PER_INSTANCE_DATA, 1},
    };

    HandleResult(d3dmodule->m_d3d11_device->CreateInputLayout(
        input_element_desc,
        _countof(input_element_desc),
        vs_blob->GetBufferPointer(),
        vs_blob->GetBufferSize(),
        m_d3d11_input_layout.GetAddressOf()
    ));

    D3D11_RASTERIZER_DESC rasterizer_desc{};
    rasterizer_desc.FillMode = D3D11_FILL_SOLID;
    rasterizer_desc.CullMode = D3D11_CULL_NONE;
    rasterizer_desc.FrontCounterClockwise = false;
    rasterizer_desc.DepthBias = 0;
    rasterizer_desc.DepthClipEnable = false;

    HandleResult(d3dmodule->m_d3d11_device->CreateRasterizerState(
        &rasterizer_desc,
        m_d3d11_rasterizer_state.GetAddressOf()
    ));

    D3D11_DEPTH_STENCIL_DESC depth_stencil_desc{};
    depth_stencil_desc.DepthEnable = false;
    depth_stencil_desc.DepthWriteMask = D3D11_DEPTH_WRITE_MASK_ZERO;
    depth_stencil_desc.DepthFunc = D3D11_COMPARISON_LESS;

    HandleResult(d3dmodule->m_d3d11_device->CreateDepthStencilState(
        &depth_stencil_desc,
        m_d3d11_depth_stencil_state.GetAddressOf()
    ));

    RECT rect{};
    if (!GetClientRect(d3dmodule->m_game_window, &rect)) {
        dlog::error("Failed to get client rect");
    }

    D3D11_TEXTURE2D_DESC texture_desc{};
    texture_desc.Width = rect.right - rect.left;
    texture_desc.Height = rect.bottom - rect.top;
    texture_desc.MipLevels = 1;
    texture_desc.ArraySize = 1;
    texture_desc.Format = DXGI_FORMAT_D32_FLOAT;
    texture_desc.SampleDesc.Count = 1;
    texture_desc.SampleDesc.Quality = 0;
    texture_desc.Usage = D3D11_USAGE_DEFAULT;
    texture_desc.BindFlags = D3D11_BIND_DEPTH_STENCIL;

    HandleResult(d3dmodule->m_d3d11_device->CreateTexture2D(
        &texture_desc,
        nullptr,
        m_d3d11_depth_stencil_texture.GetAddressOf()
    ));

    D3D11_DEPTH_STENCIL_VIEW_DESC depth_stencil_view_desc{};
    depth_stencil_view_desc.Format = DXGI_FORMAT_D32_FLOAT;
    depth_stencil_view_desc.ViewDimension = D3D11_DSV_DIMENSION_TEXTURE2D;
    depth_stencil_view_desc.Texture2D.MipSlice = 0;

    HandleResult(d3dmodule->m_d3d11_device->CreateDepthStencilView(
        m_d3d11_depth_stencil_texture.Get(),
        &depth_stencil_view_desc,
        m_d3d11_depth_stencil_view.GetAddressOf()
    ));

    D3D11_BLEND_DESC blend_desc{};
    blend_desc.RenderTarget[0].BlendEnable = true;
    blend_desc.RenderTarget[0].SrcBlend = D3D11_BLEND_SRC_ALPHA;
    blend_desc.RenderTarget[0].DestBlend = D3D11_BLEND_INV_SRC_ALPHA;
    blend_desc.RenderTarget[0].BlendOp = D3D11_BLEND_OP_ADD;
    blend_desc.RenderTarget[0].SrcBlendAlpha = D3D11_BLEND_ONE;
    blend_desc.RenderTarget[0].DestBlendAlpha = D3D11_BLEND_ZERO;
    blend_desc.RenderTarget[0].BlendOpAlpha = D3D11_BLEND_OP_ADD;
    blend_desc.RenderTarget[0].RenderTargetWriteMask = D3D11_COLOR_WRITE_ENABLE_ALL;

    HandleResult(d3dmodule->m_d3d11_device->CreateBlendState(
        &blend_desc,
        m_d3d11_blend_state.GetAddressOf()
    ));
}

void PrimitiveRenderingModule::late_init_d3d12(D3DModule* d3dmodule) {
}

PrimitiveRenderingModule::CpuMesh PrimitiveRenderingModule::load_mesh(const std::string& path) {
    CpuMesh mesh;
    const auto& chunk_module = NativePluginFramework::get_module<ChunkModule>();
    const auto& chunk = chunk_module->request_chunk("Default");
    const auto& sphere = chunk->get_file(path);

    std::istringstream obj_stream{ std::string{(const char*)sphere->Contents.data(), sphere->size()} };

    tinyobj::attrib_t attrib;
    std::vector<tinyobj::shape_t> shapes;
    std::string warn, err;

    if (!tinyobj::LoadObj(&attrib, &shapes, nullptr, &warn, &err, &obj_stream)) {
        dlog::error("Failed to load obj file: {}", err);
    }

    for (const auto& shape : shapes) {
        for (const auto& index : shape.mesh.indices) {
            mesh.Vertices.emplace_back(
                attrib.vertices[3 * index.vertex_index + 0],
                attrib.vertices[3 * index.vertex_index + 1],
                attrib.vertices[3 * index.vertex_index + 2],
                1.0f
            );
            mesh.Indices.push_back((u32)mesh.Indices.size());
        }
    }

    return mesh;
}

void PrimitiveRenderingModule::load_mesh_d3d11(ID3D11Device* device, const std::string& path, Mesh<ID3D11Buffer>& out) {
    const CpuMesh mesh = load_mesh(path);

    out.IndexCount = (u32)mesh.Indices.size();

    D3D11_BUFFER_DESC bd{};
    D3D11_SUBRESOURCE_DATA sd{};

    bd.ByteWidth = sizeof(Vertex) * (u32)mesh.Vertices.size();
    bd.Usage = D3D11_USAGE_DEFAULT;
    bd.BindFlags = D3D11_BIND_VERTEX_BUFFER;
    bd.CPUAccessFlags = 0;
    bd.StructureByteStride = sizeof(Vertex);

    sd.pSysMem = mesh.Vertices.data();

    HandleResult(device->CreateBuffer(&bd, &sd, out.VertexBuffer.GetAddressOf()));

    bd.ByteWidth = sizeof(u32) * (u32)mesh.Indices.size();
    bd.Usage = D3D11_USAGE_DEFAULT;
    bd.BindFlags = D3D11_BIND_INDEX_BUFFER;
    bd.CPUAccessFlags = 0;
    bd.StructureByteStride = sizeof(u32);

    sd.pSysMem = mesh.Indices.data();

    HandleResult(device->CreateBuffer(&bd, &sd, out.IndexBuffer.GetAddressOf()));
}

void PrimitiveRenderingModule::load_mesh_d3d12(ID3D12Device* device, const std::string& path, Mesh<ID3D12Resource>& out) {
    const CpuMesh mesh = load_mesh(path);

    out.IndexCount = (u32)mesh.Indices.size();

    D3D12_HEAP_PROPERTIES heap_properties{};
    heap_properties.Type = D3D12_HEAP_TYPE_UPLOAD;

    D3D12_RESOURCE_DESC resource_desc{};
    resource_desc.Dimension = D3D12_RESOURCE_DIMENSION_BUFFER;
    resource_desc.Width = sizeof(Vertex) * (u32)mesh.Vertices.size();
    resource_desc.Height = 1;
    resource_desc.DepthOrArraySize = 1;
    resource_desc.MipLevels = 1;
    resource_desc.SampleDesc.Count = 1;
    resource_desc.Layout = D3D12_TEXTURE_LAYOUT_ROW_MAJOR;

    HandleResult(device->CreateCommittedResource(
        &heap_properties,
        D3D12_HEAP_FLAG_NONE,
        &resource_desc,
        D3D12_RESOURCE_STATE_GENERIC_READ,
        nullptr,
        IID_PPV_ARGS(out.VertexBuffer.GetAddressOf())
    ));

    Vertex* vertex_data = nullptr;
    HandleResult(out.VertexBuffer->Map(0, nullptr, (void**)&vertex_data));
    std::memcpy(vertex_data, mesh.Vertices.data(), sizeof(Vertex) * mesh.Vertices.size());
    out.VertexBuffer->Unmap(0, nullptr);

    resource_desc.Width = sizeof(u32) * (u32)mesh.Indices.size();

    HandleResult(device->CreateCommittedResource(
        &heap_properties,
        D3D12_HEAP_FLAG_NONE,
        &resource_desc,
        D3D12_RESOURCE_STATE_GENERIC_READ,
        nullptr,
        IID_PPV_ARGS(out.IndexBuffer.GetAddressOf())
    ));

    u32* index_data = nullptr;
    HandleResult(out.IndexBuffer->Map(0, nullptr, (void**)&index_data));
    std::memcpy(index_data, mesh.Indices.data(), sizeof(u32) * mesh.Indices.size());
    out.IndexBuffer->Unmap(0, nullptr);
}

void PrimitiveRenderingModule::render_sphere_api(const MtSphere* sphere, const MtVector4* color) {
    NativePluginFramework::get_module<PrimitiveRenderingModule>()->render_sphere(*sphere, *color);
}

void PrimitiveRenderingModule::render_obb_api(const MtOBB* obb, const MtVector4* color) {
    NativePluginFramework::get_module<PrimitiveRenderingModule>()->render_obb(*obb, *color);
}

void PrimitiveRenderingModule::render_capsule_api(const MtCapsule* capsule, const MtVector4* color) {
    NativePluginFramework::get_module<PrimitiveRenderingModule>()->render_capsule(*capsule, *color);
}
