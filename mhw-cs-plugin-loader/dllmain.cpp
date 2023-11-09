#include "NativePluginFramework.h"
#include "CoreClr.h"
#include "Log.h"

#include <thread>


static CoreClr* s_coreclr = nullptr;
static NativePluginFramework* s_framework = nullptr;

static void initialize_loader() {
    /*for (int i = 0; i < 5; ++i) {
        dlog::info("Initializing Loader... {}", 10 - i);
        Sleep(1000);
    }*/

    s_coreclr = new CoreClr();
    dlog::info("Loader initialized");

    s_framework = new NativePluginFramework(s_coreclr);
}


BOOL APIENTRY DllMain(HMODULE hModule, DWORD  ul_reason_for_call, LPVOID lpReserved) {
    if (ul_reason_for_call == DLL_PROCESS_ATTACH) {
        std::thread(initialize_loader).detach();
    }

    return TRUE;
}
