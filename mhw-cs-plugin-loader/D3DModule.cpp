#include "D3DModule.h"

#include <dxgi1_4.h>

#include "CoreClr.h"
#include "Log.h"
#include "NativePluginFramework.h"

#include <Windows.h>
#include <imgui_impl.h>
#include <game_functions.h>

#include "imgui_impl_dx12.h"
#include "imgui_impl_dx11.h"
#include "imgui_impl_win32.h"

#include <thread>

void D3DModule::initialize(CoreClr* coreclr) {
    // Directory for delay loaded DLLs
    AddDllDirectory(TEXT("nativePC/plugins/CSharp/Loader"));

    m_core_render = coreclr->get_method<ImDrawData * ()>(
        ASSEMBLY_NAME(L"SharpPluginLoader.Core"),
        L"SharpPluginLoader.Core.Rendering.Renderer",
        L"Render"
    );
    m_core_initialize_imgui = coreclr->get_method<ImGuiContext * ()>(
        ASSEMBLY_NAME(L"SharpPluginLoader.Core"),
        L"SharpPluginLoader.Core.Rendering.Renderer",
        L"Initialize"
    );

    m_title_menu_ready_hook = safetyhook::create_inline(MH::uGUITitle::play, title_menu_ready_hook);
}

void D3DModule::shutdown() {
    m_d3d_present_hook.reset();

    if (m_is_d3d12) {
        m_d3d_execute_command_lists_hook.reset();
        m_d3d_signal_hook.reset();
    }
}

void D3DModule::common_initialize() {
    m_is_d3d12 = is_d3d12();
    m_title_menu_ready_hook.reset();

    dlog::debug("Initializing D3D module for {}", m_is_d3d12 ? "D3D12" : "D3D11");

    m_game_window = FindWindowA(nullptr, s_game_window_name);
    if (!m_game_window) {
        dlog::error("Failed to find game window ({})", GetLastError());
        return;
    }

    

    const auto window_class = new WNDCLASSEX;
    window_class->cbSize = sizeof(WNDCLASSEX);
    window_class->style = CS_HREDRAW | CS_VREDRAW;
    window_class->lpfnWndProc = DefWindowProc;
    window_class->cbClsExtra = 0;
    window_class->cbWndExtra = 0;
    window_class->hInstance = GetModuleHandle(nullptr);
    window_class->hIcon = nullptr;
    window_class->hCursor = nullptr;
    window_class->hbrBackground = nullptr;
    window_class->lpszMenuName = nullptr;
    window_class->lpszClassName = TEXT("SharpPluginLoader");
    window_class->hIconSm = nullptr;

    if (!RegisterClassEx(window_class)) {
        dlog::error("Failed to register window class ({})", GetLastError());
        return;
    }

    m_temp_window_class = window_class;

    m_temp_window = CreateWindow(
        window_class->lpszClassName,
        TEXT("SharpPluginLoader DX Hook"),
        WS_OVERLAPPEDWINDOW,
        CW_USEDEFAULT, CW_USEDEFAULT,
        100, 100,
        nullptr,
        nullptr,
        window_class->hInstance,
        nullptr
    );

    if (!m_temp_window) {
        dlog::error("Failed to create temporary window ({})", GetLastError());
        return;
    }

    if (m_is_d3d12) {
        initialize_for_d3d12();
    } else {
        initialize_for_d3d11();
    }

    DestroyWindow(m_temp_window);
    UnregisterClass(window_class->lpszClassName, window_class->hInstance);
}

void D3DModule::initialize_for_d3d12() {
    HMODULE dxgi;
    HMODULE d3d12;

    if ((dxgi = GetModuleHandleA("dxgi.dll")) == nullptr) {
        dlog::error("Failed to find dxgi.dll");
        return;
    }

    if ((d3d12 = GetModuleHandleA("d3d12.dll")) == nullptr) {
        dlog::error("Failed to find d3d12.dll");
        return;
    }

    decltype(CreateDXGIFactory)* create_dxgi_factory;
    if ((create_dxgi_factory = (decltype(create_dxgi_factory))GetProcAddress(dxgi, "CreateDXGIFactory")) == nullptr) {
        dlog::error("Failed to find CreateDXGIFactory");
        return;
    }

    IDXGIFactory* factory;
    if (FAILED(create_dxgi_factory(IID_PPV_ARGS(&factory)))) {
        dlog::error("Failed to create DXGI factory");
        return;
    }

    IDXGIAdapter* adapter;
    if (FAILED(factory->EnumAdapters(0, &adapter))) {
        dlog::error("Failed to enumerate DXGI adapters");
        return;
    }

    decltype(D3D12CreateDevice)* d3d12_create_device;
    if ((d3d12_create_device = (decltype(d3d12_create_device))GetProcAddress(d3d12, "D3D12CreateDevice")) == nullptr) {
        dlog::error("Failed to find D3D12CreateDevice");
        return;
    }

    ID3D12Device* device;
    if (FAILED(d3d12_create_device(adapter, D3D_FEATURE_LEVEL_11_0, IID_PPV_ARGS(&device)))) {
        dlog::error("Failed to create D3D12 device");
        return;
    }

    constexpr D3D12_COMMAND_QUEUE_DESC queue_desc = {
        .Type = D3D12_COMMAND_LIST_TYPE_DIRECT,
        .Priority = 0,
        .Flags = D3D12_COMMAND_QUEUE_FLAG_NONE,
        .NodeMask = 0
    };

    ID3D12CommandQueue* command_queue;
    if (FAILED(device->CreateCommandQueue(&queue_desc, IID_PPV_ARGS(&command_queue)))) {
        dlog::error("Failed to create D3D12 command queue");
        return;
    }

    ID3D12CommandAllocator* command_allocator;
    if (FAILED(device->CreateCommandAllocator(D3D12_COMMAND_LIST_TYPE_DIRECT, IID_PPV_ARGS(&command_allocator)))) {
        dlog::error("Failed to create D3D12 command allocator");
        return;
    }

    ID3D12CommandList* command_list;
    if (FAILED(device->CreateCommandList(0, D3D12_COMMAND_LIST_TYPE_DIRECT, command_allocator, nullptr, IID_PPV_ARGS(&command_list)))) {
        dlog::error("Failed to create D3D12 command list");
        return;
    }

    constexpr DXGI_RATIONAL refresh_rate = {
        .Numerator = 60,
        .Denominator = 1
    };

    DXGI_MODE_DESC buffer_desc = {
        .Width = 100,
        .Height = 100,
        .RefreshRate = refresh_rate,
        .Format = DXGI_FORMAT_R8G8B8A8_UNORM,
        .ScanlineOrdering = DXGI_MODE_SCANLINE_ORDER_UNSPECIFIED,
        .Scaling = DXGI_MODE_SCALING_UNSPECIFIED
    };

    constexpr DXGI_SAMPLE_DESC sample_desc = {
        .Count = 1,
        .Quality = 0
    };

    DXGI_SWAP_CHAIN_DESC sd = {
        .BufferDesc = buffer_desc,
        .SampleDesc = sample_desc,
        .BufferUsage = DXGI_USAGE_RENDER_TARGET_OUTPUT,
        .BufferCount = 2,
        .OutputWindow = m_temp_window,
        .Windowed = TRUE,
        .SwapEffect = DXGI_SWAP_EFFECT_FLIP_DISCARD,
        .Flags = DXGI_SWAP_CHAIN_FLAG_ALLOW_MODE_SWITCH
    };

    IDXGISwapChain* swap_chain;
    if (FAILED(factory->CreateSwapChain(command_queue, &sd, &swap_chain))) {
        dlog::error("Failed to create DXGI swap chain");
        return;
    }

    // SwapChainVFT[8]: Present
    // SwapChainVFT[13]: ResizeBuffers
    // CommandQueueVFT[10]: ExecuteCommandLists
    // CommandQueueVFT[14]: Signal

    const auto swap_chain_vft = *(void***)swap_chain;
    const auto command_queue_vft = *(void***)command_queue;

    const auto present = swap_chain_vft[8];
    const auto resize_buffers = swap_chain_vft[13];
    const auto execute_command_lists = command_queue_vft[10];
    const auto signal = command_queue_vft[14];

    m_d3d_present_hook = safetyhook::create_inline(present, d3d12_present_hook);
    m_d3d_execute_command_lists_hook = safetyhook::create_inline(execute_command_lists, d3d12_execute_command_lists_hook);
    m_d3d_signal_hook = safetyhook::create_inline(signal, d3d12_signal_hook);
    m_d3d_resize_buffers_hook = safetyhook::create_inline(resize_buffers, d3d_resize_buffers_hook);

    device->Release();
    command_queue->Release();
    command_allocator->Release();
    command_list->Release();
    swap_chain->Release();
    factory->Release();
}

void D3DModule::initialize_for_d3d11() {
    HMODULE d3d11;
    if ((d3d11 = GetModuleHandleA("d3d11.dll")) == nullptr) {
        dlog::error("Failed to find d3d11.dll");
        return;
    }

    decltype(D3D11CreateDeviceAndSwapChain)* d3d11_create_device_and_swap_chain;
    if ((d3d11_create_device_and_swap_chain = (decltype(d3d11_create_device_and_swap_chain))GetProcAddress(d3d11, "D3D11CreateDeviceAndSwapChain")) == nullptr) {
        dlog::error("Failed to find D3D11CreateDeviceAndSwapChain");
        return;
    }

    constexpr D3D_FEATURE_LEVEL feature_levels[] = { D3D_FEATURE_LEVEL_11_0, D3D_FEATURE_LEVEL_10_1 };
    D3D_FEATURE_LEVEL feature_level;

    constexpr DXGI_RATIONAL refresh_rate = {
        .Numerator = 60,
        .Denominator = 1
    };

    constexpr DXGI_MODE_DESC buffer_desc = {
        .Width = 100,
        .Height = 100,
        .RefreshRate = refresh_rate,
        .Format = DXGI_FORMAT_R8G8B8A8_UNORM,
        .ScanlineOrdering = DXGI_MODE_SCANLINE_ORDER_UNSPECIFIED,
        .Scaling = DXGI_MODE_SCALING_UNSPECIFIED
    };

    constexpr DXGI_SAMPLE_DESC sample_desc = {
        .Count = 1,
        .Quality = 0
    };

    DXGI_SWAP_CHAIN_DESC sd = {
        .BufferDesc = buffer_desc,
        .SampleDesc = sample_desc,
        .BufferUsage = DXGI_USAGE_RENDER_TARGET_OUTPUT,
        .BufferCount = 1,
        .OutputWindow = m_temp_window,
        .Windowed = TRUE,
        .SwapEffect = DXGI_SWAP_EFFECT_DISCARD,
        .Flags = DXGI_SWAP_CHAIN_FLAG_ALLOW_MODE_SWITCH
    };

    IDXGISwapChain* swap_chain;
    ID3D11Device* device;
    ID3D11DeviceContext* device_context;

    if (FAILED(d3d11_create_device_and_swap_chain(
        nullptr, 
        D3D_DRIVER_TYPE_HARDWARE, 
        nullptr, 0, 
        feature_levels, 
        _countof(feature_levels), 
        D3D11_SDK_VERSION, 
        &sd, &swap_chain, &device, &feature_level, &device_context))) {
        dlog::error("Failed to create D3D11 device and swap chain");
        return;
    }

    // SwapChainVFT[8]: Present
    const auto swap_chain_vft = *(void***)swap_chain;
    const auto present = swap_chain_vft[8];

    m_d3d_present_hook = safetyhook::create_inline(present, d3d11_present_hook);
}

void D3DModule::d3d12_initialize_imgui(IDXGISwapChain* swap_chain) {
    if (FAILED(swap_chain->GetDevice(IID_PPV_ARGS(&m_d3d12_device)))) {
        dlog::error("Failed to get D3D12 device in present hook");
        return;
    }

    const auto context = m_core_initialize_imgui();
    igSetCurrentContext(context);

    CreateEvent(nullptr, FALSE, FALSE, nullptr);

    DXGI_SWAP_CHAIN_DESC desc;
    if (FAILED(swap_chain->GetDesc(&desc))) {
        dlog::error("Failed to get DXGI swap chain description");
        return;
    }

    desc.Flags = DXGI_SWAP_CHAIN_FLAG_ALLOW_MODE_SWITCH;
    m_game_window = desc.OutputWindow;
    desc.Windowed = GetWindowLongPtr(desc.OutputWindow, GWL_STYLE) & WS_POPUP ? FALSE : TRUE;

    m_d3d12_buffer_count = desc.BufferCount;
    m_d3d12_frame_contexts.resize(desc.BufferCount, FrameContext{});

    const D3D12_DESCRIPTOR_HEAP_DESC dp_imgui_desc = {
        .Type = D3D12_DESCRIPTOR_HEAP_TYPE_CBV_SRV_UAV,
        .NumDescriptors = desc.BufferCount,
        .Flags = D3D12_DESCRIPTOR_HEAP_FLAG_SHADER_VISIBLE,
        .NodeMask = 0
    };

    if (FAILED(m_d3d12_device->CreateDescriptorHeap(&dp_imgui_desc, IID_PPV_ARGS(m_d3d12_render_targets.GetAddressOf())))) {
        dlog::error("Failed to create D3D12 descriptor heap for back buffers");
        return;
    }

    ComPtr<ID3D12CommandAllocator> command_allocator;
    if (FAILED(m_d3d12_device->CreateCommandAllocator(D3D12_COMMAND_LIST_TYPE_DIRECT, IID_PPV_ARGS(command_allocator.GetAddressOf())))) {
        dlog::error("Failed to create D3D12 command allocator");
        return;
    }

    for (auto i = 0u; i < desc.BufferCount; ++i) {
        m_d3d12_frame_contexts[i].CommandAllocator = command_allocator;
    }

    if (FAILED(m_d3d12_device->CreateCommandList(0, D3D12_COMMAND_LIST_TYPE_DIRECT, 
        command_allocator.Get(), nullptr, IID_PPV_ARGS(m_d3d12_command_list.GetAddressOf())))) {
        dlog::error("Failed to create D3D12 command list");
        return;
    }

    if (FAILED(m_d3d12_command_list->Close())) {
        dlog::error("Failed to close D3D12 command list");
        return;
    }

    const D3D12_DESCRIPTOR_HEAP_DESC back_buffer_desc = {
        .Type = D3D12_DESCRIPTOR_HEAP_TYPE_RTV,
        .NumDescriptors = desc.BufferCount,
        .Flags = D3D12_DESCRIPTOR_HEAP_FLAG_NONE,
        .NodeMask = 1
    };

    if (FAILED(m_d3d12_device->CreateDescriptorHeap(&back_buffer_desc, IID_PPV_ARGS(m_d3d12_back_buffers.GetAddressOf())))) {
        dlog::error("Failed to create D3D12 descriptor heap for back buffers");
        return;
    }

    const auto rtv_descriptor_size = m_d3d12_device->GetDescriptorHandleIncrementSize(D3D12_DESCRIPTOR_HEAP_TYPE_RTV);
    D3D12_CPU_DESCRIPTOR_HANDLE rtv_handle = m_d3d12_back_buffers->GetCPUDescriptorHandleForHeapStart();

    for (auto i = 0u; i < desc.BufferCount; ++i) {
        ComPtr<ID3D12Resource> back_buffer;
        if (FAILED(swap_chain->GetBuffer(i, IID_PPV_ARGS(back_buffer.GetAddressOf())))) {
            dlog::error("Failed to get DXGI swap chain buffer");
            return;
        }

        m_d3d12_device->CreateRenderTargetView(back_buffer.Get(), nullptr, rtv_handle);
        m_d3d12_frame_contexts[i].RenderTargetDescriptor = rtv_handle;
        m_d3d12_frame_contexts[i].RenderTarget = back_buffer;

        rtv_handle.ptr += rtv_descriptor_size;
    }

    if (!ImGui_ImplWin32_Init(m_game_window)) {
        dlog::error("Failed to initialize ImGui Win32");
        return;
    }

    if (!ImGui_ImplDX12_Init(m_d3d12_device, desc.BufferCount,
        DXGI_FORMAT_R8G8B8A8_UNORM, m_d3d12_render_targets.Get(),
        m_d3d12_render_targets->GetCPUDescriptorHandleForHeapStart(),
        m_d3d12_render_targets->GetGPUDescriptorHandleForHeapStart())) {
        dlog::error("Failed to initialize ImGui D3D12");
        return;
    }

    if (!ImGui_ImplDX12_CreateDeviceObjects()) {
        dlog::error("Failed to create ImGui D3D12 device objects");
        return;
    }

    m_game_window_proc = (WNDPROC)SetWindowLongPtr(m_game_window, GWLP_WNDPROC, (LONG_PTR)my_window_proc);
    m_is_initialized = true;

    dlog::debug("Initialized D3D12");
}

void D3DModule::d3d11_initialize_imgui(IDXGISwapChain* swap_chain) {
    throw std::exception("Not implemented");
}

void D3DModule::d3d12_deinitialize_imgui() {
    ImGui_ImplDX12_Shutdown();
    ImGui_ImplWin32_Shutdown();
    m_d3d12_frame_contexts.clear();
    m_d3d12_back_buffers = nullptr;
    m_d3d12_render_targets = nullptr;
    m_d3d12_command_list = nullptr;
    m_d3d12_command_queue = nullptr;
    m_d3d12_fence = nullptr;
    m_d3d12_fence_value = 0;
    m_d3d12_buffer_count = 0;
}

void D3DModule::d3d11_deinitialize_imgui() {
    throw std::exception("Not implemented");
}

bool D3DModule::is_d3d12() {
    return *(bool*)0x1451c9e40;
}

void D3DModule::title_menu_ready_hook(void* gui) {
    const auto self = NativePluginFramework::get_module<D3DModule>();

    std::thread t(&D3DModule::common_initialize, self);
    self->m_title_menu_ready_hook.call(gui);
    t.join();
}

HRESULT D3DModule::d3d12_present_hook(IDXGISwapChain* swap_chain, UINT sync_interval, UINT flags) {
    const auto self = NativePluginFramework::get_module<D3DModule>();

    if (!self->m_is_initialized) {
        self->d3d12_initialize_imgui(swap_chain);
    }

    if (!self->m_d3d12_command_queue) {
        return self->m_d3d_present_hook.call<HRESULT>(swap_chain, sync_interval, flags);
    }

    // Start new frame
    ImGui_ImplDX12_NewFrame();
    ImGui_ImplWin32_NewFrame();

    ImDrawData* draw_data = self->m_core_render();

    const auto swap_chain3 = (IDXGISwapChain3*)swap_chain;
    const FrameContext& frame_ctx = self->m_d3d12_frame_contexts[swap_chain3->GetCurrentBackBufferIndex()];
    frame_ctx.CommandAllocator->Reset();

    D3D12_RESOURCE_BARRIER barrier = {
        .Type = D3D12_RESOURCE_BARRIER_TYPE_TRANSITION,
        .Flags = D3D12_RESOURCE_BARRIER_FLAG_NONE,
        .Transition = {
            .pResource = frame_ctx.RenderTarget.Get(),
            .Subresource = D3D12_RESOURCE_BARRIER_ALL_SUBRESOURCES,
            .StateBefore = D3D12_RESOURCE_STATE_PRESENT,
            .StateAfter = D3D12_RESOURCE_STATE_RENDER_TARGET
        }
    };

    self->m_d3d12_command_list->Reset(frame_ctx.CommandAllocator.Get(), nullptr);
    self->m_d3d12_command_list->ResourceBarrier(1, &barrier);
    self->m_d3d12_command_list->OMSetRenderTargets(1, &frame_ctx.RenderTargetDescriptor, FALSE, nullptr);
    self->m_d3d12_command_list->SetDescriptorHeaps(1, self->m_d3d12_render_targets.GetAddressOf());

    ImGui_ImplDX12_RenderDrawData(draw_data, self->m_d3d12_command_list.Get());

    barrier.Transition.StateBefore = D3D12_RESOURCE_STATE_RENDER_TARGET;
    barrier.Transition.StateAfter = D3D12_RESOURCE_STATE_PRESENT;

    self->m_d3d12_command_list->ResourceBarrier(1, &barrier);
    self->m_d3d12_command_list->Close();

    self->m_d3d12_command_queue->ExecuteCommandLists(1, (ID3D12CommandList* const*)self->m_d3d12_command_list.GetAddressOf());

    if (igGetIO()->ConfigFlags & ImGuiConfigFlags_ViewportsEnable)
    {
        igUpdatePlatformWindows();
        igRenderPlatformWindowsDefault(nullptr, self->m_d3d12_command_list.Get());
    }

    return self->m_d3d_present_hook.call<HRESULT>(swap_chain, sync_interval, flags);
}

void D3DModule::d3d12_execute_command_lists_hook(ID3D12CommandQueue* command_queue, UINT num_command_lists, ID3D12CommandList* const* command_lists) {
    const auto self = NativePluginFramework::get_module<D3DModule>();

    if (!self->m_d3d12_command_queue && command_queue->GetDesc().Type == D3D12_COMMAND_LIST_TYPE_DIRECT) {
        dlog::debug("Found D3D12 command queue");
        self->m_d3d12_command_queue = command_queue;
    }

    return self->m_d3d_execute_command_lists_hook.call<void>(command_queue, num_command_lists, command_lists);
}

UINT64 D3DModule::d3d12_signal_hook(ID3D12CommandQueue* command_queue, ID3D12Fence* fence, UINT64 value) {
    const auto self = NativePluginFramework::get_module<D3DModule>();

    if (self->m_d3d12_command_queue == command_queue) {
        self->m_d3d12_fence = fence;
        self->m_d3d12_fence_value = value;
    }

    return self->m_d3d_signal_hook.call<UINT64>(command_queue, fence, value);
}

HRESULT D3DModule::d3d_resize_buffers_hook(IDXGISwapChain* swap_chain, UINT buffer_count, UINT w, UINT h, DXGI_FORMAT format, UINT flags) {
    const auto self = NativePluginFramework::get_module<D3DModule>();

    dlog::debug("ResizeBuffers called, resetting...");

    self->m_is_initialized = false;
    if (self->m_is_d3d12) {
        self->d3d12_deinitialize_imgui();
    } else {
        self->d3d11_deinitialize_imgui();
    }

    return self->m_d3d_resize_buffers_hook.call<HRESULT>(swap_chain, buffer_count, w, h, format, flags);
}

HRESULT D3DModule::d3d11_present_hook(IDXGISwapChain* swap_chain, UINT sync_interval, UINT flags) {
    const auto self = NativePluginFramework::get_module<D3DModule>();
    return self->m_d3d_present_hook.call<HRESULT>(swap_chain, sync_interval, flags);
}


extern IMGUI_IMPL_API LRESULT ImGui_ImplWin32_WndProcHandler(HWND hWnd, UINT msg, WPARAM wParam, LPARAM lParam);

LRESULT D3DModule::my_window_proc(HWND hwnd, UINT msg, WPARAM wparam, LPARAM lparam) {
    ImGui_ImplWin32_WndProcHandler(hwnd, msg, wparam, lparam);
    return NativePluginFramework::get_module<D3DModule>()->m_game_window_proc(hwnd, msg, wparam, lparam);
}
