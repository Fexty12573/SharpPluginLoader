#include "CoreClr.h"
#include "Log.h"

#include <thread>


static CoreClr* s_coreclr = nullptr;

static void initialize_loader() {
    dlog::info("Initializing Loader...");

    s_coreclr = new CoreClr();
    dlog::info("Loader initialized");
}


BOOL APIENTRY DllMain(HMODULE hModule, DWORD  ul_reason_for_call, LPVOID lpReserved) {
    if (ul_reason_for_call == DLL_PROCESS_ATTACH) {
        std::thread(initialize_loader).detach();
    }

    return TRUE;
}
