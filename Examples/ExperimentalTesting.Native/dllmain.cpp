#include <corecrt_wstdio.h>
#include <cstdint>
#include <Windows.h>

struct InternalCall {
    const char* Name;
    void* Function;
};

int64_t sum(int64_t a, int64_t b) {
    return a + b;
}

void modify_value(int* ptr) {
    *ptr *= 2;
}

void log_message(int level, const wchar_t* message) {
    wchar_t buf[1024];
    (void)swprintf_s(buf, L"LogLevel: %d", level);
    MessageBoxW(nullptr, message, buf, MB_OK);
}

extern "C" __declspec(dllexport) int get_internal_call_count() {
    return 3;
}

extern "C" __declspec(dllexport) void collect_internal_calls(InternalCall * icalls) {
    icalls[0] = { "Sum", (void*)sum };
    icalls[1] = { "ModifyValue", (void*)modify_value };
    icalls[2] = { "LogMessage", (void*)log_message };
}

BOOL WINAPI DllMain(HINSTANCE, DWORD, LPVOID) {
    return TRUE;
}

