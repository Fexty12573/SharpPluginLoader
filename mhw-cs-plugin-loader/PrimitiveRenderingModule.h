#pragma once
#include "NativeModule.h"
#include "Primitives.h"

#include <directxmath/DirectXMath.h>
#include <d3d11.h>
#include <d3d12.h>
#include <wrl/client.h>

#include <array>
#include <dxgi1_4.h>
#include <span>
#include <vector>

struct sMhCamera;
class D3DModule;

class PrimitiveRenderingModule final : public NativeModule {
public:
    PrimitiveRenderingModule();
    void initialize(CoreClr* coreclr) override;
    void shutdown() override;

    void late_init(D3DModule* d3dmodule, IDXGISwapChain* swap_chain);

    void render_sphere(const MtSphere& sphere, MtVector4 color);
    void render_obb(const MtOBB& obb, MtVector4 color);
    void render_capsule(const MtCapsule& capsule, MtVector4 color);
    void render_line(const MtLineSegment& line, MtVector4 color);

    void render_primitives_for_d3d11(ID3D11DeviceContext* context);
    void render_primitives_for_d3d12(IDXGISwapChain3* swap_chain, ID3D12CommandQueue* command_queue);

private:
    template<typename T> using ComPtr = Microsoft::WRL::ComPtr<T>;
    using Vertex = MtVector4;
    struct Mesh11 {
        ComPtr<ID3D11Buffer> VertexBuffer = nullptr;
        ComPtr<ID3D11Buffer> IndexBuffer = nullptr;
        u32 IndexCount = 0;
    };
    struct Mesh12 {
        ComPtr<ID3D12Resource> VertexBuffer = nullptr;
        ComPtr<ID3D12Resource> IndexBuffer = nullptr;
        u32 IndexCount = 0;
        D3D12_VERTEX_BUFFER_VIEW VertexBufferView{};
        D3D12_INDEX_BUFFER_VIEW IndexBufferView{};
    };
    struct CpuMesh {
        std::vector<Vertex> Vertices;
        std::vector<u32> Indices;
    };

    void late_init_d3d11(D3DModule* d3dmodule);
    void late_init_d3d12(D3DModule* d3dmodule, IDXGISwapChain* swap_chain);
    void create_frame_contexts(D3DModule* d3dmodule, IDXGISwapChain3* sc3);

    static CpuMesh load_mesh(const std::string& path);
    static void load_mesh_d3d11(ID3D11Device* device, const std::string& path, Mesh11& out);
    static void load_mesh_d3d12(ID3D12Device* device, const std::string& path, Mesh12& out);

    static DirectX::XMMATRIX XMMatrixAdd(DirectX::FXMMATRIX M1, DirectX::CXMMATRIX M2) {
        DirectX::XMMATRIX m;
        m.r[0] = DirectX::XMVectorAdd(M1.r[0], M2.r[0]);
        m.r[1] = DirectX::XMVectorAdd(M1.r[1], M2.r[1]);
        m.r[2] = DirectX::XMVectorAdd(M1.r[2], M2.r[2]);
        m.r[3] = DirectX::XMVectorAdd(M1.r[3], M2.r[3]);
        return m;
    }

private:
    struct Instance {
        DirectX::XMMATRIX Transform;
        DirectX::XMFLOAT4 Color;
    };
    struct ViewProj {
        DirectX::XMMATRIX View;
        DirectX::XMMATRIX Proj;
    };
    struct alignas(256) ViewProj12 { // Constant buffer must be 256-byte aligned in D3D12
        DirectX::XMMATRIX View;
        DirectX::XMMATRIX Proj;
    };
    struct FrameContext {
        ComPtr<ID3D12Resource> RenderTarget = nullptr;
        D3D12_CPU_DESCRIPTOR_HANDLE RenderTargetDescriptor = { 0 };
    };
    struct alignas(256) LineParams {
        float Thickness;
    };
    struct LineVertex {
        DirectX::XMFLOAT4 Position;
        DirectX::XMFLOAT4 Color;
    };

    static constexpr u32 MAX_INSTANCES = 2048;
    static constexpr u32 MAX_LINES = 2048;

    void(*m_retrieve_primitives)(
        primitives::Sphere** spheres, size_t* sphere_count,
        primitives::OBB** cubes, size_t* cube_count,
        primitives::Capsule** capsules, size_t* capsule_count,
        primitives::Line** lines, size_t* line_count) = nullptr;
    void(*m_release_primitives)() = nullptr;
    void*(*m_get_singleton)(const char* name) = nullptr;
    sMhCamera* m_camera = nullptr;

    std::span<primitives::Sphere> m_spheres;
    std::span<primitives::OBB> m_cubes;
    std::span<primitives::Capsule> m_capsules;
    std::span<primitives::Line> m_lines;

    size_t m_sphere_count = 0;
    size_t m_cube_count = 0;
    size_t m_capsule_count = 0;
    size_t m_line_count = 0;

    std::array<Instance, MAX_INSTANCES> m_instances{};
    std::array<Instance, MAX_INSTANCES> m_instances_hemisphere_top{};
    std::array<Instance, MAX_INSTANCES> m_instances_hemisphere_bottom{};

    bool m_is_initialized = false;
    float m_line_thickness = 3.0f;
    bool m_draw_primitives_as_lines = true;

    #pragma region D3D11

    Mesh11 m_d3d11_cylinder{};
    Mesh11 m_d3d11_hemisphere_top{};
    Mesh11 m_d3d11_hemisphere_bottom{};
    Mesh11 m_d3d11_sphere{};
    Mesh11 m_d3d11_cube{};
    ComPtr<ID3D11Buffer> m_d3d11_htop_transform_buffer = nullptr;
    ComPtr<ID3D11Buffer> m_d3d11_hbottom_transform_buffer = nullptr;
    ComPtr<ID3D11Buffer> m_d3d11_transform_buffer = nullptr;
    ComPtr<ID3D11Buffer> m_d3d11_viewproj_buffer = nullptr;
    ComPtr<ID3D11VertexShader> m_d3d11_vertex_shader = nullptr;
    ComPtr<ID3D11PixelShader> m_d3d11_pixel_shader = nullptr;
    ComPtr<ID3D11InputLayout> m_d3d11_input_layout = nullptr;
    ComPtr<ID3D11RasterizerState> m_d3d11_rasterizer_state = nullptr;
    ComPtr<ID3D11DepthStencilState> m_d3d11_depth_stencil_state = nullptr;
    ComPtr<ID3D11Texture2D> m_d3d11_depth_stencil_texture = nullptr;
    ComPtr<ID3D11DepthStencilView> m_d3d11_depth_stencil_view = nullptr;
    ComPtr<ID3D11BlendState> m_d3d11_blend_state = nullptr;

    ComPtr<ID3D11Buffer> m_d3d11_line_vertex_buffer = nullptr;
    ComPtr<ID3D11Buffer> m_d3d11_line_params_buffer = nullptr;
    ComPtr<ID3D11VertexShader> m_d3d11_line_vertex_shader = nullptr;
    ComPtr<ID3D11GeometryShader> m_d3d11_line_geometry_shader = nullptr;
    ComPtr<ID3D11PixelShader> m_d3d11_line_pixel_shader = nullptr;
    ComPtr<ID3D11InputLayout> m_d3d11_line_input_layout = nullptr;

    #pragma endregion
    #pragma region D3D12

    Mesh12 m_d3d12_cylinder{};
    Mesh12 m_d3d12_hemisphere_top{};
    Mesh12 m_d3d12_hemisphere_bottom{};
    Mesh12 m_d3d12_sphere{};
    Mesh12 m_d3d12_cube{};
    ComPtr<ID3D12Resource> m_d3d12_htop_transform_buffer = nullptr;
    ComPtr<ID3D12Resource> m_d3d12_hbottom_transform_buffer = nullptr;
    ComPtr<ID3D12Resource> m_d3d12_transform_buffer = nullptr;
    ComPtr<ID3D12Resource> m_d3d12_viewproj_buffer = nullptr;
    ComPtr<ID3D12Resource> m_d3d12_depth_stencil_texture = nullptr;
    ComPtr<ID3D12RootSignature> m_d3d12_root_signature = nullptr;
    ComPtr<ID3D12PipelineState> m_d3d12_pipeline_state = nullptr;

    ComPtr<ID3D12RootSignature> m_d3d12_line_root_signature = nullptr;
    ComPtr<ID3D12PipelineState> m_d3d12_line_pipeline_state = nullptr;
    ComPtr<ID3D12Resource> m_d3d12_line_vertex_buffer = nullptr;
    ComPtr<ID3D12Resource> m_d3d12_line_params_buffer = nullptr;

    ComPtr<ID3D12CommandAllocator> m_d3d12_command_allocator = nullptr;
    ComPtr<ID3D12GraphicsCommandList> m_d3d12_command_list = nullptr;
    std::unique_ptr<FrameContext[]> m_d3d12_frame_contexts;
    u32 m_d3d12_back_buffer_count = 0;
    ComPtr<ID3D12DescriptorHeap> m_d3d12_rtv_heap = nullptr;
    ComPtr<ID3D12DescriptorHeap> m_d3d12_dsv_heap = nullptr;

    D3D12_VIEWPORT m_d3d12_viewport{};
    D3D12_RECT m_d3d12_scissor_rect{};
    D3D12_CPU_DESCRIPTOR_HANDLE m_d3d12_depth_stencil_view = { 0 };
    D3D12_VERTEX_BUFFER_VIEW m_d3d12_htop_transform_buffer_view{};
    D3D12_VERTEX_BUFFER_VIEW m_d3d12_hbottom_transform_buffer_view{};
    D3D12_VERTEX_BUFFER_VIEW m_d3d12_transform_buffer_view{};
    D3D12_VERTEX_BUFFER_VIEW m_d3d12_line_vertex_buffer_view{};

    #pragma endregion
};

