#pragma once
#include "NativeModule.h"
#include "PrimitiveRenderingModule.h"

#include <d3d11.h>
#include <d3d12.h>
#include <dxgi.h>
#include <IconsFontAwesome6.h>
#include <wrl.h>

#include <imgui_impl.h>
#include <safetyhook/safetyhook.hpp>

#include <vector>

class D3DModule final : public NativeModule {
    template<typename T> using ComPtr = Microsoft::WRL::ComPtr<T>;

public:
    void initialize(CoreClr* coreclr) override;
    void shutdown() override;

private:
    void common_initialize();
    void initialize_for_d3d12();
    void initialize_for_d3d11();

    void d3d12_initialize_imgui(IDXGISwapChain* swap_chain);
    void d3d11_initialize_imgui(IDXGISwapChain* swap_chain);

    void d3d12_deinitialize_imgui();
    void d3d11_deinitialize_imgui();
    void imgui_load_fonts();

    static bool is_d3d12();

    static void title_menu_ready_hook(void* gui);

    static HRESULT d3d12_present_hook(IDXGISwapChain* swap_chain, UINT sync_interval, UINT flags);
    static void d3d12_execute_command_lists_hook(ID3D12CommandQueue* command_queue, UINT num_command_lists, ID3D12CommandList* const* command_lists);
    static UINT64 d3d12_signal_hook(ID3D12CommandQueue* command_queue, ID3D12Fence* fence, UINT64 value);

    static HRESULT d3d11_present_hook(IDXGISwapChain* swap_chain, UINT sync_interval, UINT flags);

    static HRESULT d3d_resize_buffers_hook(IDXGISwapChain* swap_chain, UINT buffer_count, UINT w, UINT h, DXGI_FORMAT format, UINT flags);
    static LRESULT my_window_proc(HWND hwnd, UINT msg, WPARAM wparam, LPARAM lparam);

    struct FrameContext {
        ComPtr<ID3D12CommandAllocator> CommandAllocator = nullptr;
        ComPtr<ID3D12Resource> RenderTarget = nullptr;
        D3D12_CPU_DESCRIPTOR_HANDLE RenderTargetDescriptor = { 0 };
    };

private:
    bool m_is_d3d12 = false;
    bool m_is_initialized = false;
    bool m_is_inside_present = false;

    safetyhook::InlineHook m_title_menu_ready_hook;

    safetyhook::InlineHook m_d3d_present_hook;
    safetyhook::InlineHook m_d3d_execute_command_lists_hook;
    safetyhook::InlineHook m_d3d_signal_hook;
    safetyhook::InlineHook m_d3d_resize_buffers_hook;

    ID3D12Device* m_d3d12_device = nullptr;
    ComPtr<ID3D12DescriptorHeap> m_d3d12_back_buffers = nullptr;
    ComPtr<ID3D12DescriptorHeap> m_d3d12_render_targets = nullptr;
    ComPtr<ID3D12GraphicsCommandList> m_d3d12_command_list = nullptr;
    ID3D12CommandQueue* m_d3d12_command_queue = nullptr;
    ID3D12Fence* m_d3d12_fence = nullptr;
    UINT64 m_d3d12_fence_value = 0;
    UINT32 m_d3d12_buffer_count = 0;
    std::vector<FrameContext> m_d3d12_frame_contexts;

    ID3D11Device* m_d3d11_device = nullptr;
    ID3D11DeviceContext* m_d3d11_device_context = nullptr;
    IDXGISwapChain* m_d3d11_swap_chain = nullptr;

    HMODULE m_d3d12_module = nullptr;
    HMODULE m_d3d11_module = nullptr;

    HWND m_game_window = nullptr;
    HMODULE m_game_module = nullptr;
    WNDPROC m_game_window_proc = nullptr;

    HWND m_temp_window = nullptr;
    WNDCLASSEX* m_temp_window_class = nullptr;

    ImGuiContext*(*m_core_initialize_imgui)() = nullptr;
    ImDrawData*(*m_core_imgui_render)() = nullptr;
    void(*m_core_render)() = nullptr;

    friend class PrimitiveRenderingModule;

    static constexpr const char* s_game_window_name = "MONSTER HUNTER: WORLD(421652)";

    static constexpr ImWchar s_japanese_glyph_ranges[] = {
        0x3000, 0x30FF, // CJK Symbols and Punctuations, Hiragana, Katakana
        0x31F0, 0x31FF, // Katakana Phonetic Extensions
        0x4e00, 0x9FAF, // CJK Ideograms
        0xFF00, 0xFFEF, // Half-width characters
        0,
    };
    static constexpr ImWchar icons_ranges[] = { ICON_MIN_FA, ICON_MAX_FA, 0 };
};

