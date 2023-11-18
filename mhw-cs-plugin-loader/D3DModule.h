#pragma once
#include "NativeModule.h"

#include <d3d11.h>
#include <d3d12.h>
#include <dxgi.h>

#include <safetyhook/safetyhook.hpp>

class D3DModule final : public NativeModule {
public:
    void initialize(CoreClr* coreclr) override;
    void shutdown() override;

private:
    void common_initialize();
    void initialize_for_d3d12();
    void initialize_for_d3d11();

    static bool is_d3d12();

    static bool game_present_hook(void* render, UINT sync_interval);

    static void d3d12_present_hook(IDXGISwapChain* swap_chain, UINT sync_interval, UINT flags);
    static void d3d12_execute_command_lists_hook(ID3D12CommandQueue* command_queue, UINT num_command_lists, ID3D12CommandList* const* command_lists);
    static UINT64 d3d12_signal_hook(ID3D12CommandQueue* command_queue, ID3D12Fence* fence, UINT64 value);

    static void d3d11_present_hook(IDXGISwapChain* swap_chain, UINT sync_interval, UINT flags);

    struct FrameContext {
        ID3D12CommandAllocator* CommandAllocator = nullptr;
        ID3D12Resource* RenderTarget = nullptr;
        D3D12_CPU_DESCRIPTOR_HANDLE RenderTargetDescriptor = {};
    };

private:
    bool m_is_d3d12 = false;
    bool m_is_initialized = false;

    safetyhook::InlineHook m_game_present_hook;

    safetyhook::InlineHook m_d3d_present_hook;
    safetyhook::InlineHook m_d3d_execute_command_lists_hook;
    safetyhook::InlineHook m_d3d_signal_hook;

    ID3D12Device* m_d3d12_device = nullptr;
    ID3D12DescriptorHeap* m_d3d12_back_buffers = nullptr;
    ID3D12DescriptorHeap* m_d3d12_render_targets = nullptr;
    ID3D12GraphicsCommandList* m_d3d12_command_list = nullptr;
    ID3D12CommandQueue* m_d3d12_command_queue = nullptr;
    ID3D12Fence* m_d3d12_fence = nullptr;
    UINT64 m_d3d12_fence_value = 0;
    UINT32 m_d3d12_buffer_count = 0;
    FrameContext* m_d3d12_frame_contexts = nullptr;

    ID3D11Device* m_d3d11_device = nullptr;
    ID3D11DeviceContext* m_d3d11_device_context = nullptr;
    IDXGISwapChain* m_d3d11_swap_chain = nullptr;

    HWND m_game_window = nullptr;
    HMODULE m_game_module = nullptr;

    HWND m_temp_window = nullptr;
    WNDCLASSEX* m_temp_window_class = nullptr;

    void(*m_plugin_render)() = nullptr;

    static constexpr const char* s_game_window_name = "MONSTER HUNTER: WORLD(421652)";
};

