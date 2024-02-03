#include "PrimitiveRenderingModule.h"

#include <d3dcompiler.h>
#include <strstream>

#include "HResultHandler.h"
#include "D3DModule.h"

#include <d3dx12.h>
#include <dxgi1_4.h>
#include <tiny_obj_loader.h>
#include <dti/sMhCamera.h>

#include "Config.h"
#include "ChunkModule.h"
#include "LoaderConfig.h"
#include "NativePluginFramework.h"

PrimitiveRenderingModule::PrimitiveRenderingModule() = default;

void PrimitiveRenderingModule::initialize(CoreClr* coreclr) {
    if (!preloader::LoaderConfig::get().get_primitive_rendering_enabled()) {
        auto disabled = [](void*, void*) {
            dlog::warn("Primitive rendering is disabled");
        };

        coreclr->add_internal_call("RenderSphere", &disabled);
        coreclr->add_internal_call("RenderObb", &disabled);
        coreclr->add_internal_call("RenderCapsule", &disabled);
        coreclr->add_internal_call("RenderLine", &disabled);
        return;
    }

    coreclr->add_internal_call("RenderSphere", render_sphere_api);
    coreclr->add_internal_call("RenderObb", render_obb_api);
    coreclr->add_internal_call("RenderCapsule", render_capsule_api);
    coreclr->add_internal_call("RenderLine", render_line_api);

    struct RenderingOptionPointers {
        float* LineThickness;
        bool* DrawPrimitivesAsLines;
    } rendering_option_pointers = {
        &m_line_thickness,
        &m_draw_primitives_as_lines
    };

    const auto set_rendering_options = coreclr->get_method<void(RenderingOptionPointers*)>(
        config::SPL_CORE_ASSEMBLY_NAME,
        L"SharpPluginLoader.Core.Rendering.Renderer",
        L"SetRenderingOptions"
    );

    set_rendering_options(&rendering_option_pointers);
}

void PrimitiveRenderingModule::shutdown() {
    dlog::debug("[PrimitiveRenderingModule] Shutting down");
    if (m_d3d12_frame_contexts) {
        m_d3d12_frame_contexts.reset();
    }
}

void PrimitiveRenderingModule::late_init(D3DModule* d3dmodule, IDXGISwapChain* swap_chain) {
    dlog::debug("[PrimitiveRenderingModule] Late init");
    if (D3DModule::is_d3d12()) {
        late_init_d3d12(d3dmodule, swap_chain);
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

void PrimitiveRenderingModule::render_line(const MtLineSegment& line, MtVector4 color) {
    m_lines.emplace_back(line, color);
}

void PrimitiveRenderingModule::render_primitives_for_d3d11(ID3D11DeviceContext* context) {
    using namespace DirectX;

    if (m_spheres.empty() && 
        m_cubes.empty() && 
        m_capsules.empty() &&
        m_lines.empty()) {
        return;
    }

    // Set up common pipeline state
    ComPtr<ID3D11RenderTargetView> rtv;
    context->OMGetRenderTargets(1, rtv.GetAddressOf(), nullptr);

    context->IASetPrimitiveTopology(
        m_draw_primitives_as_lines ? D3D11_PRIMITIVE_TOPOLOGY_LINELIST : D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST
    );
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

    // Lines --------------------------------
    if (!m_lines.empty()) {
        context->IASetInputLayout(m_d3d11_line_input_layout.Get());
        context->IASetPrimitiveTopology(D3D11_PRIMITIVE_TOPOLOGY_LINELIST);

        context->VSSetShader(m_d3d11_line_vertex_shader.Get(), nullptr, 0);
        context->GSSetShader(m_d3d11_line_geometry_shader.Get(), nullptr, 0);
        context->PSSetShader(m_d3d11_line_pixel_shader.Get(), nullptr, 0);

        HandleResult(context->Map(m_d3d11_line_params_buffer.Get(), 0, D3D11_MAP_WRITE_DISCARD, 0, &msr));
        const auto params = (LineParams*)msr.pData;
        params->Thickness = m_line_thickness;
        context->Unmap(m_d3d11_line_params_buffer.Get(), 0);
        
        const std::array constant_buffers = {
            m_d3d11_viewproj_buffer.Get(),
            m_d3d11_line_params_buffer.Get()
        };

        context->GSSetConstantBuffers(0, (u32)constant_buffers.size(), constant_buffers.data());

        HandleResult(context->Map(m_d3d11_line_vertex_buffer.Get(), 0, D3D11_MAP_WRITE_DISCARD, 0, &msr));
        auto data = (LineVertex*)msr.pData;

        i = 0;
        for (const auto& line : m_lines) {
            data[i++] = {
                .Position = { line.line.p0.x, line.line.p0.y, line.line.p0.z, 1.0f },
                .Color = { line.color.r, line.color.g, line.color.b, line.color.a }
            };
            data[i++] = {
                .Position = { line.line.p1.x, line.line.p1.y, line.line.p1.z, 1.0f },
                .Color = { line.color.r, line.color.g, line.color.b, line.color.a }
            };
        }

        context->Unmap(m_d3d11_line_vertex_buffer.Get(), 0);

        constexpr u32 stride = sizeof(LineVertex);
        constexpr u32 offset = 0;
        context->IASetVertexBuffers(0, 1, m_d3d11_line_vertex_buffer.GetAddressOf(), &stride, &offset);

        context->DrawInstanced(
            2,
            (u32)m_lines.size(), 0, 0
        );
    }

    m_spheres.clear();
    m_cubes.clear();
    m_capsules.clear();
    m_lines.clear();
}

void PrimitiveRenderingModule::render_primitives_for_d3d12(IDXGISwapChain3* swap_chain, ID3D12CommandQueue* command_queue) {
    using namespace DirectX;

    if (m_spheres.empty() && 
        m_cubes.empty() &&
        m_capsules.empty() &&
        m_lines.empty()) {
        return;
    }

    // Set up common pipeline state
    
    const FrameContext& frame_context = m_d3d12_frame_contexts[swap_chain->GetCurrentBackBufferIndex()];

    HandleResult(m_d3d12_command_allocator->Reset());

    D3D12_RESOURCE_BARRIER barrier{};
    barrier.Type = D3D12_RESOURCE_BARRIER_TYPE_TRANSITION;
    barrier.Flags = D3D12_RESOURCE_BARRIER_FLAG_NONE;
    barrier.Transition.pResource = frame_context.RenderTarget.Get();
    barrier.Transition.StateBefore = D3D12_RESOURCE_STATE_PRESENT;
    barrier.Transition.StateAfter = D3D12_RESOURCE_STATE_RENDER_TARGET;
    barrier.Transition.Subresource = D3D12_RESOURCE_BARRIER_ALL_SUBRESOURCES;

    HandleResult(m_d3d12_command_list->Reset(m_d3d12_command_allocator.Get(), m_d3d12_pipeline_state.Get()));
    m_d3d12_command_list->ResourceBarrier(1, &barrier);

    m_d3d12_command_list->SetGraphicsRootSignature(m_d3d12_root_signature.Get());
    m_d3d12_command_list->SetPipelineState(m_d3d12_pipeline_state.Get());

    constexpr float clear_color[] = { 1.f, 0.f, 1.f, 1.0f };
    m_d3d12_command_list->RSSetViewports(1, &m_d3d12_viewport);
    m_d3d12_command_list->RSSetScissorRects(1, &m_d3d12_scissor_rect);
    m_d3d12_command_list->OMSetRenderTargets(
        1,     
        &frame_context.RenderTargetDescriptor, 
        false, 
        nullptr
    );

    m_d3d12_command_list->IASetPrimitiveTopology(
        m_draw_primitives_as_lines ? D3D_PRIMITIVE_TOPOLOGY_LINELIST : D3D_PRIMITIVE_TOPOLOGY_TRIANGLELIST
    );

    // Store VP data
    const auto camera = sMhCamera::get();

    ViewProj12* vp = nullptr;
    HandleResult(m_d3d12_viewproj_buffer->Map(0, nullptr, (void**)&vp));
    vp->View = XMMatrixTranspose(XMMATRIX(camera->mViewports[0].mViewMat.ptr()));
    vp->Proj = XMMatrixTranspose(XMMATRIX(camera->mViewports[0].mProjMat.ptr()));

    m_d3d12_viewproj_buffer->Unmap(0, nullptr);

    // Set up VP constant buffer
    m_d3d12_command_list->SetGraphicsRootConstantBufferView(0, m_d3d12_viewproj_buffer->GetGPUVirtualAddress());

    // Common variables
    int i = 0;
    std::array<D3D12_VERTEX_BUFFER_VIEW, 2> views{};

    // Spheres ------------------------------
    if (!m_spheres.empty()) {
        // Build Instance Data
        const D3D12_RANGE range{ 
            0, 
            sizeof(Instance) * std::min<u32>((u32)m_spheres.size(), MAX_INSTANCES)
        };
        Instance* data = nullptr;
        HandleResult(m_d3d12_transform_buffer->Map(0, &range, (void**)&data));

        for (const auto& sphere : m_spheres) {
            const XMMATRIX scale = XMMatrixScaling(sphere.sphere.r, sphere.sphere.r, sphere.sphere.r);
            const XMMATRIX translation = XMMatrixTranslation(sphere.sphere.pos.x, sphere.sphere.pos.y, sphere.sphere.pos.z);

            data[i++] = {
                .Transform = XMMatrixTranspose(scale * translation),
                .Color = { sphere.color.r, sphere.color.g, sphere.color.b, sphere.color.a }
            };

            if (i >= MAX_INSTANCES) {
                break;
            }
        }

        m_d3d12_transform_buffer->Unmap(0, nullptr);

        // Set up pipeline
        views[0] = m_d3d12_sphere.VertexBufferView;
        views[1] = m_d3d12_transform_buffer_view;

        m_d3d12_command_list->IASetIndexBuffer(&m_d3d12_sphere.IndexBufferView);
        m_d3d12_command_list->IASetVertexBuffers(0, (u32)views.size(), views.data());
        m_d3d12_command_list->DrawIndexedInstanced(
            m_d3d12_sphere.IndexCount,
            i, 0, 0, 0
        );
    }

    // OBBs ---------------------------------
    if (!m_cubes.empty()) {
        // Build Instance Data
        i = 0;
        const D3D12_RANGE range{
            0,
            sizeof(Instance) * std::min<u32>((u32)m_cubes.size(), MAX_INSTANCES)
        };
        Instance* data = nullptr;
        HandleResult(m_d3d12_transform_buffer->Map(0, &range, (void**)&data));

        for (const auto& cube : m_cubes) {
            const XMMATRIX scale = XMMatrixScaling(cube.obb.extent.x, cube.obb.extent.y, cube.obb.extent.z);
            const XMMATRIX translation_rotation{ cube.obb.coord.ptr() };

            data[i++] = {
                .Transform = XMMatrixTranspose(scale * translation_rotation),
                .Color = { cube.color.r, cube.color.g, cube.color.b, cube.color.a }
            };

            if (i >= MAX_INSTANCES) {
                break;
            }
        }

        m_d3d12_transform_buffer->Unmap(0, nullptr);

        // Set up pipeline
        views[0] = m_d3d12_cube.VertexBufferView;
        views[1] = m_d3d12_transform_buffer_view;

        m_d3d12_command_list->IASetIndexBuffer(&m_d3d12_cube.IndexBufferView);
        m_d3d12_command_list->IASetVertexBuffers(0, (u32)views.size(), views.data());
        m_d3d12_command_list->DrawIndexedInstanced(
            m_d3d12_cube.IndexCount,
            i, 0, 0, 0
        );
    }

    // Capsules -----------------------------
    if (!m_capsules.empty()) {
        // Build Instance Data
        i = 0;
        const D3D12_RANGE range{
            0,
            sizeof(Instance) * std::min<u32>((u32)m_capsules.size(), MAX_INSTANCES)
        };
        Instance* data = nullptr;
        Instance* data_htop = nullptr;
        Instance* data_hbottom = nullptr;
        HandleResult(m_d3d12_transform_buffer->Map(0, &range, (void**)&data));
        HandleResult(m_d3d12_htop_transform_buffer->Map(0, &range, (void**)&data_htop));
        HandleResult(m_d3d12_hbottom_transform_buffer->Map(0, &range, (void**)&data_hbottom));

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

            data[i] = {
                .Transform = XMMatrixTranspose(scale_cylinder * rotation * translation_cylinder),
                .Color = { capsule.color.r, capsule.color.g, capsule.color.b, capsule.color.a }
            };
            data_htop[i] = {
                .Transform = XMMatrixTranspose(scale_hemisphere * rotation * translation_htop),
                .Color = { capsule.color.r, capsule.color.g, capsule.color.b, capsule.color.a }
            };
            data_hbottom[i] = {
                .Transform = XMMatrixTranspose(scale_hemisphere * rotation * translation_hbottom),
                .Color = { capsule.color.r, capsule.color.g, capsule.color.b, capsule.color.a }
            };

            i += 1;
            if (i >= MAX_INSTANCES) {
                break;
            }
        }

        m_d3d12_transform_buffer->Unmap(0, nullptr);
        m_d3d12_htop_transform_buffer->Unmap(0, nullptr);
        m_d3d12_hbottom_transform_buffer->Unmap(0, nullptr);

        // Top Hemisphere
        views[0] = m_d3d12_hemisphere_top.VertexBufferView;
        views[1] = m_d3d12_htop_transform_buffer_view;

        m_d3d12_command_list->IASetIndexBuffer(&m_d3d12_hemisphere_top.IndexBufferView);
        m_d3d12_command_list->IASetVertexBuffers(0, (u32)views.size(), views.data());
        m_d3d12_command_list->DrawIndexedInstanced(
            m_d3d12_hemisphere_top.IndexCount,
            i, 0, 0, 0
        );

        // Bottom Hemisphere
        views[0] = m_d3d12_hemisphere_bottom.VertexBufferView;
        views[1] = m_d3d12_hbottom_transform_buffer_view;

        m_d3d12_command_list->IASetIndexBuffer(&m_d3d12_hemisphere_bottom.IndexBufferView);
        m_d3d12_command_list->IASetVertexBuffers(0, (u32)views.size(), views.data());
        m_d3d12_command_list->DrawIndexedInstanced(
            m_d3d12_hemisphere_bottom.IndexCount,
            i, 0, 0, 0
        );

        // Cylinder
        views[0] = m_d3d12_cylinder.VertexBufferView;
        views[1] = m_d3d12_transform_buffer_view;

        m_d3d12_command_list->IASetIndexBuffer(&m_d3d12_cylinder.IndexBufferView);
        m_d3d12_command_list->IASetVertexBuffers(0, (u32)views.size(), views.data());
        m_d3d12_command_list->DrawIndexedInstanced(
            m_d3d12_cylinder.IndexCount,
            i, 0, 0, 0
        );
    }

    // Lines --------------------------------
    if (!m_lines.empty()) {
        // Set up line pipeline state
        m_d3d12_command_list->SetPipelineState(m_d3d12_line_pipeline_state.Get());
        m_d3d12_command_list->SetGraphicsRootSignature(m_d3d12_line_root_signature.Get());

        // Bind ViewProj buffer
        m_d3d12_command_list->SetGraphicsRootConstantBufferView(0, m_d3d12_viewproj_buffer->GetGPUVirtualAddress());

        // Set up line params buffer
        LineParams* line_params = nullptr;
        HandleResult(m_d3d12_line_params_buffer->Map(0, nullptr, (void**)&line_params));
        line_params->Thickness = m_line_thickness;
        m_d3d12_line_params_buffer->Unmap(0, nullptr);

        m_d3d12_command_list->SetGraphicsRootConstantBufferView(1, m_d3d12_line_params_buffer->GetGPUVirtualAddress());

        // Set up line vertex buffer
        const D3D12_RANGE range{
            0,
            sizeof(LineVertex) * m_lines.size() * 2
        };

        LineVertex* data = nullptr;
        HandleResult(m_d3d12_line_vertex_buffer->Map(0, &range, (void**)&data));

        i = 0;
        for (const auto& line : m_lines) {
            data[i++] = LineVertex{
                .Position = { line.line.p0.x, line.line.p0.y, line.line.p0.z, 1.0f },
                .Color = { line.color.r, line.color.g, line.color.b, line.color.a }
            };
            data[i++] = LineVertex{
                .Position = { line.line.p1.x, line.line.p1.y, line.line.p1.z, 1.0f },
                .Color = { line.color.r, line.color.g, line.color.b, line.color.a }
            };
        }

        m_d3d12_line_vertex_buffer->Unmap(0, nullptr);

        views[0] = m_d3d12_line_vertex_buffer_view;

        m_d3d12_command_list->IASetPrimitiveTopology(D3D_PRIMITIVE_TOPOLOGY_LINELIST);
        m_d3d12_command_list->IASetVertexBuffers(0, 1, views.data());
        m_d3d12_command_list->DrawInstanced(
            2,
            (u32)m_lines.size(), 0, 0
        );

        m_lines.clear();
    }

    // Close command list
    barrier.Transition.StateBefore = D3D12_RESOURCE_STATE_RENDER_TARGET;
    barrier.Transition.StateAfter = D3D12_RESOURCE_STATE_PRESENT;

    m_d3d12_command_list->ResourceBarrier(1, &barrier);
    HandleResult(m_d3d12_command_list->Close());

    command_queue->ExecuteCommandLists(1, CommandListCast(m_d3d12_command_list.GetAddressOf()));

    m_spheres.clear();
    m_cubes.clear();
    m_capsules.clear();
}

void PrimitiveRenderingModule::late_init_d3d11(D3DModule* d3dmodule) {
    if (m_is_initialized) {
        return;
    }

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

    // Create Line Params Constant Buffer
    bd.ByteWidth = sizeof LineParams;
    HandleResult(d3dmodule->m_d3d11_device->CreateBuffer(&bd, nullptr, m_d3d11_line_params_buffer.GetAddressOf()));

    bd.ByteWidth = sizeof Instance * MAX_INSTANCES;
    bd.Usage = D3D11_USAGE_DYNAMIC;
    bd.BindFlags = D3D11_BIND_VERTEX_BUFFER;
    bd.CPUAccessFlags = D3D11_CPU_ACCESS_WRITE;
    bd.StructureByteStride = sizeof Instance;

    HandleResult(d3dmodule->m_d3d11_device->CreateBuffer(&bd, nullptr, m_d3d11_transform_buffer.GetAddressOf()));
    HandleResult(d3dmodule->m_d3d11_device->CreateBuffer(&bd, nullptr, m_d3d11_htop_transform_buffer.GetAddressOf()));
    HandleResult(d3dmodule->m_d3d11_device->CreateBuffer(&bd, nullptr, m_d3d11_hbottom_transform_buffer.GetAddressOf()));
    
    // Create Line Vertex Buffer
    bd.ByteWidth = sizeof LineVertex * MAX_LINES * 2;
    bd.Usage = D3D11_USAGE_DYNAMIC;
    bd.BindFlags = D3D11_BIND_VERTEX_BUFFER;
    bd.CPUAccessFlags = D3D11_CPU_ACCESS_WRITE;
    bd.StructureByteStride = sizeof LineVertex;

    HandleResult(d3dmodule->m_d3d11_device->CreateBuffer(&bd, nullptr, m_d3d11_line_vertex_buffer.GetAddressOf()));

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

    vs_blob.Reset();
    ps_blob.Reset();
    ComPtr<ID3DBlob> gs_blob;

    const auto& line_vs = chunk->get_file("/Resources/LineRenderingVS.hlsl");
    const auto& line_gs = chunk->get_file("/Resources/LineRenderingGS.hlsl");
    const auto& line_ps = chunk->get_file("/Resources/LineRenderingPS.hlsl");

    load(line_vs, "vs_5_0", vs_blob);
    load(line_gs, "gs_5_0", gs_blob);
    load(line_ps, "ps_5_0", ps_blob);

    HandleResult(d3dmodule->m_d3d11_device->CreateVertexShader(
        vs_blob->GetBufferPointer(),
        vs_blob->GetBufferSize(),
        nullptr,
        m_d3d11_line_vertex_shader.GetAddressOf()
    ));

    HandleResult(d3dmodule->m_d3d11_device->CreateGeometryShader(
        gs_blob->GetBufferPointer(),
        gs_blob->GetBufferSize(),
        nullptr,
        m_d3d11_line_geometry_shader.GetAddressOf()
    ));

    HandleResult(d3dmodule->m_d3d11_device->CreatePixelShader(
        ps_blob->GetBufferPointer(),
        ps_blob->GetBufferSize(),
        nullptr,
        m_d3d11_line_pixel_shader.GetAddressOf()
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

    constexpr D3D11_INPUT_ELEMENT_DESC line_input_element_desc[] = {
        {"POSITION", 0, DXGI_FORMAT_R32G32B32A32_FLOAT, 0, 0, D3D11_INPUT_PER_VERTEX_DATA, 0},
        {"COLOR", 0, DXGI_FORMAT_R32G32B32A32_FLOAT, 0, 16, D3D11_INPUT_PER_VERTEX_DATA, 0},
    };

    HandleResult(d3dmodule->m_d3d11_device->CreateInputLayout(
        line_input_element_desc,
        _countof(line_input_element_desc),
        vs_blob->GetBufferPointer(),
        vs_blob->GetBufferSize(),
        m_d3d11_line_input_layout.GetAddressOf()
    ));

    D3D11_RASTERIZER_DESC rasterizer_desc{};
    rasterizer_desc.FillMode = D3D11_FILL_SOLID;
    rasterizer_desc.CullMode = D3D11_CULL_NONE;
    rasterizer_desc.FrontCounterClockwise = false;
    rasterizer_desc.DepthBias = 0;
    rasterizer_desc.DepthClipEnable = false;
    rasterizer_desc.MultisampleEnable = true;

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

    m_is_initialized = true;
}

void PrimitiveRenderingModule::late_init_d3d12(D3DModule* d3dmodule, IDXGISwapChain* swap_chain) {
    if (m_is_initialized) {
        create_frame_contexts(d3dmodule, (IDXGISwapChain3*)swap_chain);
        return;
    }

    load_mesh_d3d12(d3dmodule->m_d3d12_device, "/Resources/Sphere.obj", m_d3d12_sphere);
    load_mesh_d3d12(d3dmodule->m_d3d12_device, "/Resources/Cube.obj", m_d3d12_cube);
    load_mesh_d3d12(d3dmodule->m_d3d12_device, "/Resources/Hemisphere.obj", m_d3d12_hemisphere_top);
    load_mesh_d3d12(d3dmodule->m_d3d12_device, "/Resources/BottomHemisphere.obj", m_d3d12_hemisphere_bottom);
    load_mesh_d3d12(d3dmodule->m_d3d12_device, "/Resources/Cylinder.obj", m_d3d12_cylinder);

    // Mesh Root Signature ----------------------------------------------
    CD3DX12_ROOT_PARAMETER root_parameters[3]{};

    // ViewProj Constant Buffer
    root_parameters[0].InitAsConstantBufferView(0);

    // Vertex Buffer
    root_parameters[1].InitAsShaderResourceView(0);

    // Instance Buffer
    root_parameters[2].InitAsShaderResourceView(1);

    CD3DX12_ROOT_SIGNATURE_DESC root_signature_desc{};
    root_signature_desc.Init(
        _countof(root_parameters),
        root_parameters,
        0,
        nullptr,
        D3D12_ROOT_SIGNATURE_FLAG_ALLOW_INPUT_ASSEMBLER_INPUT_LAYOUT
    );

    ComPtr<ID3DBlob> signature_blob;
    ComPtr<ID3DBlob> error_blob;
    
    const auto serialize_root_signature = (decltype(D3D12SerializeRootSignature)*)GetProcAddress(
        d3dmodule->m_d3d12_module, "D3D12SerializeRootSignature"
    );
    if (!serialize_root_signature) {
        dlog::error("Failed to get D3D12SerializeRootSignature");
    }

    HandleResult(serialize_root_signature(
        &root_signature_desc,
        D3D_ROOT_SIGNATURE_VERSION_1,
        signature_blob.GetAddressOf(),
        error_blob.GetAddressOf()
    ));

    HandleResult(d3dmodule->m_d3d12_device->CreateRootSignature(
        0,
        signature_blob->GetBufferPointer(),
        signature_blob->GetBufferSize(),
        IID_PPV_ARGS(m_d3d12_root_signature.GetAddressOf())
    ));

    // Line Root Signature ----------------------------------------------
    memset(root_parameters, 0, sizeof root_parameters);

    // ViewProj Constant Buffer
    root_parameters[0].InitAsConstantBufferView(0, 0, D3D12_SHADER_VISIBILITY_GEOMETRY);

    // Line Params Constant Buffer
    root_parameters[1].InitAsConstantBufferView(1, 0, D3D12_SHADER_VISIBILITY_GEOMETRY);

    root_signature_desc.Init(
        2,
        root_parameters,
        0,
        nullptr,
        D3D12_ROOT_SIGNATURE_FLAG_ALLOW_INPUT_ASSEMBLER_INPUT_LAYOUT
    );

    signature_blob.Reset();
    error_blob.Reset();

    HandleResult(serialize_root_signature(
        &root_signature_desc,
        D3D_ROOT_SIGNATURE_VERSION_1,
        signature_blob.GetAddressOf(),
        error_blob.GetAddressOf()
    ));

    HandleResult(d3dmodule->m_d3d12_device->CreateRootSignature(
        0,
        signature_blob->GetBufferPointer(),
        signature_blob->GetBufferSize(),
        IID_PPV_ARGS(m_d3d12_line_root_signature.GetAddressOf())
    ));

    // Mesh Pipeline State ----------------------------------------------
    ComPtr<ID3DBlob> vs_blob;
    ComPtr<ID3DBlob> ps_blob;

    const auto& chunk = NativePluginFramework::get_module<ChunkModule>()->request_chunk("Default");
    const auto& vs = chunk->get_file("/Resources/PrimitiveRenderingVS.hlsl");
    const auto& ps = chunk->get_file("/Resources/PrimitiveRenderingPS.hlsl");

    const auto load = [&](const Ref<FileSystemFile>& file, const char* target, ComPtr<ID3DBlob>& blob) {
#ifdef _DEBUG
        constexpr UINT compile_flags = D3DCOMPILE_DEBUG;
#else
        constexpr UINT compile_flags = 0;
#endif

        HandleResult(D3DCompile(
            file->Contents.data(),
            file->size(),
            nullptr,
            nullptr,
            nullptr,
            "main",
            target,
            compile_flags,
            0,
            blob.GetAddressOf(),
            nullptr
        ));
    };

    load(vs, "vs_5_0", vs_blob);
    load(ps, "ps_5_0", ps_blob);

    D3D12_INPUT_ELEMENT_DESC input_element_desc[] = {
        // Per Vertex (Buffer 1)
        {"POSITION", 0, DXGI_FORMAT_R32G32B32A32_FLOAT, 0, 0, D3D12_INPUT_CLASSIFICATION_PER_VERTEX_DATA, 0},
        // Per Instance (Buffer 2)
        {"TRANSFORM", 0, DXGI_FORMAT_R32G32B32A32_FLOAT, 1, 0, D3D12_INPUT_CLASSIFICATION_PER_INSTANCE_DATA, 1},
        {"TRANSFORM", 1, DXGI_FORMAT_R32G32B32A32_FLOAT, 1, 16, D3D12_INPUT_CLASSIFICATION_PER_INSTANCE_DATA, 1},
        {"TRANSFORM", 2, DXGI_FORMAT_R32G32B32A32_FLOAT, 1, 32, D3D12_INPUT_CLASSIFICATION_PER_INSTANCE_DATA, 1},
        {"TRANSFORM", 3, DXGI_FORMAT_R32G32B32A32_FLOAT, 1, 48, D3D12_INPUT_CLASSIFICATION_PER_INSTANCE_DATA, 1},
        {"COLOR", 0, DXGI_FORMAT_R32G32B32A32_FLOAT, 1, 64, D3D12_INPUT_CLASSIFICATION_PER_INSTANCE_DATA, 1},
    };

    // Raseterizer State
    D3D12_RASTERIZER_DESC rasterizer_desc{};
    rasterizer_desc.FillMode = D3D12_FILL_MODE_SOLID;
    rasterizer_desc.CullMode = D3D12_CULL_MODE_NONE;
    rasterizer_desc.FrontCounterClockwise = false;
    rasterizer_desc.DepthClipEnable = false;
    rasterizer_desc.MultisampleEnable = true;

    // Depth Stencil State
    D3D12_DEPTH_STENCIL_DESC depth_stencil_desc{};
    depth_stencil_desc.DepthEnable = false; // TODO
    depth_stencil_desc.DepthWriteMask = D3D12_DEPTH_WRITE_MASK_ALL;
    depth_stencil_desc.DepthFunc = D3D12_COMPARISON_FUNC_LESS;
    depth_stencil_desc.StencilEnable = false;

    // Blend State
    D3D12_BLEND_DESC blend_desc{};
    for (auto& rtv : blend_desc.RenderTarget) {
        rtv.BlendEnable = true;
        rtv.SrcBlend = D3D12_BLEND_SRC_ALPHA;
        rtv.DestBlend = D3D12_BLEND_INV_SRC_ALPHA;
        rtv.BlendOp = D3D12_BLEND_OP_ADD;
        rtv.SrcBlendAlpha = D3D12_BLEND_ONE;
        rtv.DestBlendAlpha = D3D12_BLEND_ZERO;
        rtv.BlendOpAlpha = D3D12_BLEND_OP_ADD;
        rtv.RenderTargetWriteMask = D3D12_COLOR_WRITE_ENABLE_ALL;
    }

    // Pipeline State
    const auto sc3 = (IDXGISwapChain3*)swap_chain;
    DXGI_SWAP_CHAIN_DESC swap_chain_desc{};
    HandleResult(sc3->GetDesc(&swap_chain_desc));

    D3D12_GRAPHICS_PIPELINE_STATE_DESC pso_desc{};
    pso_desc.pRootSignature = m_d3d12_root_signature.Get();
    pso_desc.VS = CD3DX12_SHADER_BYTECODE(vs_blob.Get());
    pso_desc.PS = CD3DX12_SHADER_BYTECODE(ps_blob.Get());
    pso_desc.InputLayout = { input_element_desc, _countof(input_element_desc) };
    pso_desc.RasterizerState = rasterizer_desc;
    pso_desc.DepthStencilState = CD3DX12_DEPTH_STENCIL_DESC(D3D12_DEFAULT);
    //pso_desc.DSVFormat = DXGI_FORMAT_D32_FLOAT;
    pso_desc.BlendState = blend_desc;
    pso_desc.SampleMask = UINT_MAX;
    pso_desc.PrimitiveTopologyType = D3D12_PRIMITIVE_TOPOLOGY_TYPE_TRIANGLE;
    pso_desc.NumRenderTargets = swap_chain_desc.BufferCount;
    for (u32 i = 0; i < swap_chain_desc.BufferCount; ++i) {
        pso_desc.RTVFormats[i] = swap_chain_desc.BufferDesc.Format;
    }
    pso_desc.DSVFormat = DXGI_FORMAT_D32_FLOAT;
    pso_desc.SampleDesc.Count = 1;

    HandleResult(d3dmodule->m_d3d12_device->CreateGraphicsPipelineState(
        &pso_desc,
        IID_PPV_ARGS(m_d3d12_pipeline_state.GetAddressOf())
    ));

    // Line Pipeline State ----------------------------------------------
    vs_blob.Reset();
    ps_blob.Reset();
    ComPtr<ID3DBlob> gs_blob;

    const auto& line_vs = chunk->get_file("/Resources/LineRenderingVS.hlsl");
    const auto& line_gs = chunk->get_file("/Resources/LineRenderingGS.hlsl");
    const auto& line_ps = chunk->get_file("/Resources/LineRenderingPS.hlsl");

    load(line_vs, "vs_5_0", vs_blob);
    load(line_gs, "gs_5_0", gs_blob);
    load(line_ps, "ps_5_0", ps_blob);

    D3D12_INPUT_ELEMENT_DESC line_input_element_desc[] = {
        {"POSITION", 0, DXGI_FORMAT_R32G32B32A32_FLOAT, 0, 0, D3D12_INPUT_CLASSIFICATION_PER_VERTEX_DATA, 0},
        {"COLOR", 0, DXGI_FORMAT_R32G32B32A32_FLOAT, 0, 16, D3D12_INPUT_CLASSIFICATION_PER_VERTEX_DATA, 0}
    };

    D3D12_GRAPHICS_PIPELINE_STATE_DESC line_pso_desc{};
    line_pso_desc.pRootSignature = m_d3d12_line_root_signature.Get();
    line_pso_desc.VS = CD3DX12_SHADER_BYTECODE(vs_blob.Get());
    line_pso_desc.GS = CD3DX12_SHADER_BYTECODE(gs_blob.Get());
    line_pso_desc.PS = CD3DX12_SHADER_BYTECODE(ps_blob.Get());
    line_pso_desc.InputLayout = { line_input_element_desc, _countof(line_input_element_desc) };
    line_pso_desc.RasterizerState = rasterizer_desc;
    line_pso_desc.DepthStencilState = depth_stencil_desc;
    line_pso_desc.BlendState = blend_desc;
    line_pso_desc.SampleMask = UINT_MAX;
    line_pso_desc.PrimitiveTopologyType = D3D12_PRIMITIVE_TOPOLOGY_TYPE_LINE;
    line_pso_desc.NumRenderTargets = swap_chain_desc.BufferCount;
    for (u32 i = 0; i < swap_chain_desc.BufferCount; ++i) {
        line_pso_desc.RTVFormats[i] = swap_chain_desc.BufferDesc.Format;
    }
    line_pso_desc.DSVFormat = DXGI_FORMAT_D32_FLOAT;
    line_pso_desc.SampleDesc.Count = 1;

    HandleResult(d3dmodule->m_d3d12_device->CreateGraphicsPipelineState(
        &line_pso_desc,
        IID_PPV_ARGS(m_d3d12_line_pipeline_state.GetAddressOf())
    ));

    // ViewProj Constant Buffer
    D3D12_HEAP_PROPERTIES heap_properties{};
    heap_properties.Type = D3D12_HEAP_TYPE_UPLOAD;

    D3D12_RESOURCE_DESC resource_desc{};
    resource_desc.Dimension = D3D12_RESOURCE_DIMENSION_BUFFER;
    resource_desc.Width = sizeof ViewProj12;
    resource_desc.Height = 1;
    resource_desc.DepthOrArraySize = 1;
    resource_desc.MipLevels = 1;
    resource_desc.Format = DXGI_FORMAT_UNKNOWN;
    resource_desc.SampleDesc.Count = 1;
    resource_desc.Layout = D3D12_TEXTURE_LAYOUT_ROW_MAJOR;

    HandleResult(d3dmodule->m_d3d12_device->CreateCommittedResource(
        &heap_properties,
        D3D12_HEAP_FLAG_NONE,
        &resource_desc,
        D3D12_RESOURCE_STATE_GENERIC_READ,
        nullptr,
        IID_PPV_ARGS(m_d3d12_viewproj_buffer.GetAddressOf())
    ));

    // Instance Buffer
    resource_desc.Width = sizeof Instance * MAX_INSTANCES;

    HandleResult(d3dmodule->m_d3d12_device->CreateCommittedResource(
        &heap_properties,
        D3D12_HEAP_FLAG_NONE,
        &resource_desc,
        D3D12_RESOURCE_STATE_GENERIC_READ,
        nullptr,
        IID_PPV_ARGS(m_d3d12_transform_buffer.GetAddressOf())
    ));
    HandleResult(d3dmodule->m_d3d12_device->CreateCommittedResource(
        &heap_properties,
        D3D12_HEAP_FLAG_NONE,
        &resource_desc,
        D3D12_RESOURCE_STATE_GENERIC_READ,
        nullptr,
        IID_PPV_ARGS(m_d3d12_htop_transform_buffer.GetAddressOf())
    ));
    HandleResult(d3dmodule->m_d3d12_device->CreateCommittedResource(
        &heap_properties,
        D3D12_HEAP_FLAG_NONE,
        &resource_desc,
        D3D12_RESOURCE_STATE_GENERIC_READ,
        nullptr,
        IID_PPV_ARGS(m_d3d12_hbottom_transform_buffer.GetAddressOf())
    ));

    // Line Params Constant Buffer
    resource_desc.Width = sizeof LineParams;

    HandleResult(d3dmodule->m_d3d12_device->CreateCommittedResource(
        &heap_properties,
        D3D12_HEAP_FLAG_NONE,
        &resource_desc,
        D3D12_RESOURCE_STATE_GENERIC_READ,
        nullptr,
        IID_PPV_ARGS(m_d3d12_line_params_buffer.GetAddressOf())
    ));

    // Line Vertex Buffer
    resource_desc.Width = sizeof LineVertex * MAX_LINES * 2; // 2 vertices per line

    HandleResult(d3dmodule->m_d3d12_device->CreateCommittedResource(
        &heap_properties,
        D3D12_HEAP_FLAG_NONE,
        &resource_desc,
        D3D12_RESOURCE_STATE_GENERIC_READ,
        nullptr,
        IID_PPV_ARGS(m_d3d12_line_vertex_buffer.GetAddressOf())
    ));

    // Create Views
    m_d3d12_transform_buffer_view.BufferLocation = m_d3d12_transform_buffer->GetGPUVirtualAddress();
    m_d3d12_transform_buffer_view.SizeInBytes = sizeof Instance * MAX_INSTANCES;
    m_d3d12_transform_buffer_view.StrideInBytes = sizeof Instance;

    m_d3d12_htop_transform_buffer_view.BufferLocation = m_d3d12_htop_transform_buffer->GetGPUVirtualAddress();
    m_d3d12_htop_transform_buffer_view.SizeInBytes = sizeof Instance * MAX_INSTANCES;
    m_d3d12_htop_transform_buffer_view.StrideInBytes = sizeof Instance;

    m_d3d12_hbottom_transform_buffer_view.BufferLocation = m_d3d12_hbottom_transform_buffer->GetGPUVirtualAddress();
    m_d3d12_hbottom_transform_buffer_view.SizeInBytes = sizeof Instance * MAX_INSTANCES;
    m_d3d12_hbottom_transform_buffer_view.StrideInBytes = sizeof Instance;

    m_d3d12_line_vertex_buffer_view.BufferLocation = m_d3d12_line_vertex_buffer->GetGPUVirtualAddress();
    m_d3d12_line_vertex_buffer_view.SizeInBytes = sizeof LineVertex * MAX_LINES * 2;
    m_d3d12_line_vertex_buffer_view.StrideInBytes = sizeof LineVertex;

    // Depth Stencil
    // Depth Stencil Descriptor Heap
    D3D12_DESCRIPTOR_HEAP_DESC descriptor_heap_desc{};
    descriptor_heap_desc.NumDescriptors = 1;
    descriptor_heap_desc.Type = D3D12_DESCRIPTOR_HEAP_TYPE_DSV;
    descriptor_heap_desc.Flags = D3D12_DESCRIPTOR_HEAP_FLAG_NONE;
    descriptor_heap_desc.NodeMask = 0;

    HandleResult(d3dmodule->m_d3d12_device->CreateDescriptorHeap(
        &descriptor_heap_desc,
        IID_PPV_ARGS(m_d3d12_dsv_heap.GetAddressOf())
    ));

    m_d3d12_depth_stencil_view = m_d3d12_dsv_heap->GetCPUDescriptorHandleForHeapStart();

    RECT rect{};
    if (!GetClientRect(d3dmodule->m_game_window, &rect)) {
        dlog::error("Failed to get client rect");
    }

    D3D12_RESOURCE_DESC texture_desc = CD3DX12_RESOURCE_DESC::Tex2D(
        DXGI_FORMAT_D32_FLOAT,
        rect.right - rect.left,
        rect.bottom - rect.top,
        1, 1, 1, 0,
        D3D12_RESOURCE_FLAG_ALLOW_DEPTH_STENCIL
    );

    m_d3d12_viewport = CD3DX12_VIEWPORT(
        0.0f,
        0.0f,
        (float)texture_desc.Width,
        (float)texture_desc.Height
    );
    m_d3d12_scissor_rect = CD3DX12_RECT(
        0,
        0,
        (LONG)texture_desc.Width,
        (LONG)texture_desc.Height
    );

    D3D12_CLEAR_VALUE depth_clear_value{};
    depth_clear_value.Format = DXGI_FORMAT_D32_FLOAT;
    depth_clear_value.DepthStencil.Depth = 1.0f;
    
    const auto default_heap_properties = CD3DX12_HEAP_PROPERTIES(D3D12_HEAP_TYPE_DEFAULT);
    HandleResult(d3dmodule->m_d3d12_device->CreateCommittedResource(
        &default_heap_properties,
        D3D12_HEAP_FLAG_NONE,
        &texture_desc,
        D3D12_RESOURCE_STATE_DEPTH_WRITE,
        &depth_clear_value,
        IID_PPV_ARGS(m_d3d12_depth_stencil_texture.GetAddressOf())
    ));

    D3D12_DEPTH_STENCIL_VIEW_DESC depth_stencil_view_desc{};
    depth_stencil_view_desc.Format = DXGI_FORMAT_D32_FLOAT;
    depth_stencil_view_desc.ViewDimension = D3D12_DSV_DIMENSION_TEXTURE2D;

    d3dmodule->m_d3d12_device->CreateDepthStencilView(
        m_d3d12_depth_stencil_texture.Get(),
        &depth_stencil_view_desc,
        m_d3d12_depth_stencil_view
    );

    // Command Allocator
    HandleResult(d3dmodule->m_d3d12_device->CreateCommandAllocator(
        D3D12_COMMAND_LIST_TYPE_DIRECT,
        IID_PPV_ARGS(m_d3d12_command_allocator.GetAddressOf())
    ));

    // Command List
    HandleResult(d3dmodule->m_d3d12_device->CreateCommandList(
        0,
        D3D12_COMMAND_LIST_TYPE_DIRECT,
        m_d3d12_command_allocator.Get(),
        m_d3d12_pipeline_state.Get(),
        IID_PPV_ARGS(m_d3d12_command_list.GetAddressOf())
    ));

    HandleResult(m_d3d12_command_list->Close());

    // Backbuffers
    const u32 buffer_count = m_d3d12_back_buffer_count = swap_chain_desc.BufferCount;

    descriptor_heap_desc.NumDescriptors = buffer_count;
    descriptor_heap_desc.Type = D3D12_DESCRIPTOR_HEAP_TYPE_RTV;
    descriptor_heap_desc.Flags = D3D12_DESCRIPTOR_HEAP_FLAG_NONE;
    descriptor_heap_desc.NodeMask = 0;

    HandleResult(d3dmodule->m_d3d12_device->CreateDescriptorHeap(
        &descriptor_heap_desc,
        IID_PPV_ARGS(m_d3d12_rtv_heap.GetAddressOf())
    ));

    create_frame_contexts(d3dmodule, sc3);

    m_is_initialized = true;
}

void PrimitiveRenderingModule::create_frame_contexts(D3DModule* d3dmodule, IDXGISwapChain3* sc3) {
    dlog::debug("[PrimitiveRenderingModule] Creating frame contexts");

    m_d3d12_frame_contexts = std::make_unique<FrameContext[]>(m_d3d12_back_buffer_count);

    const u32 rtv_descriptor_size = d3dmodule->m_d3d12_device->GetDescriptorHandleIncrementSize(D3D12_DESCRIPTOR_HEAP_TYPE_RTV);
    D3D12_CPU_DESCRIPTOR_HANDLE rtv_handle = m_d3d12_rtv_heap->GetCPUDescriptorHandleForHeapStart();

    for (u32 i = 0; i < m_d3d12_back_buffer_count; i++) {
        HandleResult(sc3->GetBuffer(i, IID_PPV_ARGS(m_d3d12_frame_contexts[i].RenderTarget.GetAddressOf())));

        HandleResult(m_d3d12_frame_contexts[i].RenderTarget->SetName(L"SPL: Render Target"));
        d3dmodule->m_d3d12_device->CreateRenderTargetView(
            m_d3d12_frame_contexts[i].RenderTarget.Get(),
            nullptr,
            rtv_handle
        );

        m_d3d12_frame_contexts[i].RenderTargetDescriptor = rtv_handle;
        rtv_handle.ptr += rtv_descriptor_size;
    }
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

void PrimitiveRenderingModule::load_mesh_d3d11(ID3D11Device* device, const std::string& path, Mesh11& out) {
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

void PrimitiveRenderingModule::load_mesh_d3d12(ID3D12Device* device, const std::string& path, Mesh12& out) {
    const CpuMesh mesh = load_mesh(path);

    out.IndexCount = (u32)mesh.Indices.size();

    D3D12_HEAP_PROPERTIES heap_properties{};
    heap_properties.Type = D3D12_HEAP_TYPE_UPLOAD;

    D3D12_RESOURCE_DESC resource_desc{};
    resource_desc.Dimension = D3D12_RESOURCE_DIMENSION_BUFFER;
    resource_desc.Width = sizeof(Vertex) * mesh.Vertices.size();
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

    resource_desc.Width = sizeof(u32) * mesh.Indices.size();

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

    out.VertexBufferView.BufferLocation = out.VertexBuffer->GetGPUVirtualAddress();
    out.VertexBufferView.SizeInBytes = sizeof(Vertex) * (u32)mesh.Vertices.size();
    out.VertexBufferView.StrideInBytes = sizeof(Vertex);

    out.IndexBufferView.BufferLocation = out.IndexBuffer->GetGPUVirtualAddress();
    out.IndexBufferView.SizeInBytes = sizeof(u32) * (u32)mesh.Indices.size();
    out.IndexBufferView.Format = DXGI_FORMAT_R32_UINT;
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

void PrimitiveRenderingModule::render_line_api(const MtLineSegment* line, const MtVector4* color) {
    NativePluginFramework::get_module<PrimitiveRenderingModule>()->render_line(*line, *color);
}
