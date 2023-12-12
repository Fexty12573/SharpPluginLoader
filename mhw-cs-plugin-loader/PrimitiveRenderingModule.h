#pragma once
#include "NativeModule.h"
#include "Primitives.h"

#include <directxmath/DirectXMath.h>
#include <d3d11.h>
#include <d3d12.h>
#include <wrl/client.h>

#include <array>
#include <vector>

class D3DModule;

class PrimitiveRenderingModule final : public NativeModule {
public:
    PrimitiveRenderingModule();
    void initialize(CoreClr* coreclr) override;
    void shutdown() override;

    void late_init(D3DModule* d3dmodule);

    void render_sphere(const MtSphere& sphere, MtVector4 color);
    void render_obb(const MtOBB& obb, MtVector4 color);
    void render_capsule(const MtCapsule& capsule, MtVector4 color);

    void render_primitives_for_d3d11(ID3D11DeviceContext* context);
    void render_primitives_for_d3d12();

private:
    template<typename T> using ComPtr = Microsoft::WRL::ComPtr<T>;
    using Vertex = MtVector4;
    template<class T> struct Mesh {
        ComPtr<T> VertexBuffer = nullptr;
        ComPtr<T> IndexBuffer = nullptr;
        u32 IndexCount = 0;
    };
    struct CpuMesh {
        std::vector<Vertex> Vertices;
        std::vector<u32> Indices;
    };

    void late_init_d3d11(D3DModule* d3dmodule);
    void late_init_d3d12(D3DModule* d3dmodule);

    static CpuMesh load_mesh(const std::string& path);
    static void load_mesh_d3d11(ID3D11Device* device, const std::string& path, Mesh<ID3D11Buffer>& out);
    static void load_mesh_d3d12(ID3D12Device* device, const std::string& path, Mesh<ID3D12Resource>& out);

    static void render_sphere_api(const MtSphere* sphere, const MtVector4* color);
    static void render_obb_api(const MtOBB* obb, const MtVector4* color);
    static void render_capsule_api(const MtCapsule* capsule, const MtVector4* color);

private:
    struct Instance {
        DirectX::XMMATRIX Transform;
        DirectX::XMFLOAT4 Color;
    };
    struct ViewProj {
        DirectX::XMMATRIX View;
        DirectX::XMMATRIX Proj;
    };

    static constexpr u32 MAX_INSTANCES = 128;

    std::vector<primitives::Sphere> m_spheres;
    std::vector<primitives::OBB> m_cubes;
    std::vector<primitives::Capsule> m_capsules;

    std::array<Instance, MAX_INSTANCES> m_instances{};

    #pragma region D3D11

    Mesh<ID3D11Buffer> m_d3d11_sphere{};
    Mesh<ID3D11Buffer> m_d3d11_cube{};
    Mesh<ID3D11Buffer> m_d3d11_capsule{};
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

    #pragma endregion
    #pragma region D3D12

    Mesh<ID3D12Resource> m_d3d12_sphere;
    Mesh<ID3D12Resource> m_d3d12_cube;
    Mesh<ID3D12Resource> m_d3d12_capsule;
    ComPtr<ID3D12Resource> m_d3d12_transform_buffer = nullptr;
    ComPtr<ID3D12Resource> m_d3d12_vp_matrix_buffer = nullptr;

    #pragma endregion
};

