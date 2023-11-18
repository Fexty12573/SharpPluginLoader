#include "D3DModule.h"
#include "CoreClr.h"
#include "Log.h"
#include "NativePluginFramework.h"

#include <game_functions.h>

#include <thread>

void D3DModule::initialize(CoreClr* coreclr) {
    m_game_present_hook = safetyhook::create_inline(MH::Render::Present, game_present_hook);
}

void D3DModule::shutdown() {
}

void D3DModule::common_initialize() {
    m_is_d3d12 = is_d3d12();
    m_game_present_hook.reset();

    m_game_window = FindWindowA(nullptr, s_game_window_name);
    if (!m_game_window) {
        dlog::error("Failed to find game window");
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
        dlog::error("Failed to register window class");
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
    if (FAILED(create_dxgi_factory(__uuidof(IDXGIFactory), (void**)&factory))) {
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
    if (FAILED(d3d12_create_device(adapter, D3D_FEATURE_LEVEL_11_0, __uuidof(ID3D12Device), (void**)&device))) {
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
    if (FAILED(device->CreateCommandQueue(&queue_desc, __uuidof(ID3D12CommandQueue), (void**)&command_queue))) {
        dlog::error("Failed to create D3D12 command queue");
        return;
    }

    ID3D12CommandAllocator* command_allocator;
    if (FAILED(device->CreateCommandAllocator(D3D12_COMMAND_LIST_TYPE_DIRECT, __uuidof(ID3D12CommandAllocator), (void**)&command_allocator))) {
        dlog::error("Failed to create D3D12 command allocator");
        return;
    }

    ID3D12CommandList* command_list;
    if (FAILED(device->CreateCommandList(0, D3D12_COMMAND_LIST_TYPE_DIRECT, command_allocator, nullptr, __uuidof(ID3D12GraphicsCommandList), (void**)&command_list))) {
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
    // CommandQueueVFT[10]: ExecuteCommandLists
    // CommandQueueVFT[14]: Signal

    const auto swap_chain_vft = *(void***)swap_chain;
    const auto command_queue_vft = *(void***)command_queue;

    const auto present = swap_chain_vft[8];
    const auto execute_command_lists = command_queue_vft[10];
    const auto signal = command_queue_vft[14];

    m_d3d_present_hook = safetyhook::create_inline(present, d3d12_present_hook);
    m_d3d_execute_command_lists_hook = safetyhook::create_inline(execute_command_lists, d3d12_execute_command_lists_hook);
    m_d3d_signal_hook = safetyhook::create_inline(signal, d3d12_signal_hook);

    device->Release();
    command_queue->Release();
    command_allocator->Release();
    command_list->Release();
    swap_chain->Release();
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

bool D3DModule::is_d3d12() {
    return *(bool*)0x1451c9e40;
}

bool D3DModule::game_present_hook(void* render, UINT sync_interval) {
    const auto self = NativePluginFramework::get_module<D3DModule>();

    std::thread(D3DModule::common_initialize, self).detach();

    return self->m_game_present_hook.call<bool>(render, sync_interval);
}

void D3DModule::d3d12_present_hook(IDXGISwapChain* swap_chain, UINT sync_interval, UINT flags) {
    const auto self = NativePluginFramework::get_module<D3DModule>();

    if (!self->m_is_initialized) {
        if (FAILED(swap_chain->GetDevice(__uuidof(ID3D12Device), (void**)&self->m_d3d12_device))) {
            dlog::error("Failed to get D3D12 device in present hook");
            return;
        }

        // ImGui::CreateContext();

        // unsigned char* pixels = nullptr;
        // int width = 0, height = 0;
           
        // ImGuiIO& io = ImGui::GetIO(); (void)io;
           
        // ImGui::StyleColorsDark();
        // io.Fonts->AddFontDefault();
        // io.Fonts->GetTexDataAsRGBA32(&pixels, &width, &height);
        // io.IniFilename = nullptr;

        CreateEvent(nullptr, FALSE, FALSE, nullptr);


    }
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

void D3DModule::d3d11_present_hook(IDXGISwapChain* swap_chain, UINT sync_interval, UINT flags) {
}
