#include "NativePluginFramework.h"
#include "CoreClr.h"
#include "Log.h"
#include <dinput.h>
#include <thread>

namespace {

CoreClr* s_coreclr = nullptr;
NativePluginFramework* s_framework = nullptr;

void initialize_loader() {
    //for (int i = 0; i < 10; ++i) {
    //    dlog::info("Initializing Loader... {}", 10 - i);
    //    Sleep(1000);
    //}

    s_coreclr = new CoreClr();
    s_framework = new NativePluginFramework(s_coreclr);
    
    dlog::info("Initialized");
}

}


BOOL APIENTRY DllMain(HMODULE hModule, DWORD  ul_reason_for_call, LPVOID lpReserved) {
    if (ul_reason_for_call == DLL_PROCESS_ATTACH) {
        std::thread(initialize_loader).detach();
    }

    return TRUE;
}
