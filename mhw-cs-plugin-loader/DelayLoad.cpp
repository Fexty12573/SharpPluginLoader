#include "Log.h"

#if defined(__cplusplus)
#define ExternC extern "C"
#else
#define ExternC extern
#endif

typedef IMAGE_THUNK_DATA* PImgThunkData;
typedef const IMAGE_THUNK_DATA* PCImgThunkData;
typedef DWORD                       RVA;

typedef struct ImgDelayDescr {
    DWORD           grAttrs;        // attributes
    RVA             rvaDLLName;     // RVA to dll name
    RVA             rvaHmod;        // RVA of module handle
    RVA             rvaIAT;         // RVA of the IAT
    RVA             rvaINT;         // RVA of the INT
    RVA             rvaBoundIAT;    // RVA of the optional bound IAT
    RVA             rvaUnloadIAT;   // RVA of optional copy of original IAT
    DWORD           dwTimeStamp;    // 0 if not bound,
    // O.W. date/time stamp of DLL bound to (Old BIND)
} ImgDelayDescr, * PImgDelayDescr;

typedef const ImgDelayDescr* PCImgDelayDescr;

enum DLAttr {                   // Delay Load Attributes
    dlattrRva = 0x1,                // RVAs are used instead of pointers
    // Having this set indicates a VC7.0
    // and above delay load descriptor.
};

//
// Delay load import hook notifications
//
enum {
    dliStartProcessing,             // used to bypass or note helper only
    dliNoteStartProcessing = dliStartProcessing,

    dliNotePreLoadLibrary,          // called just before LoadLibrary, can
    //  override w/ new HMODULE return val
    dliNotePreGetProcAddress,       // called just before GetProcAddress, can
    //  override w/ new FARPROC return value
    dliFailLoadLib,                 // failed to load library, fix it by
    //  returning a valid HMODULE
    dliFailGetProc,                 // failed to get proc address, fix it by
    //  returning a valid FARPROC
    dliNoteEndProcessing,           // called after all processing is done, no
    //  bypass possible at this point except
    //  by longjmp()/throw()/RaiseException.
};

typedef struct DelayLoadProc {
    BOOL                fImportByName;
    union {
        LPCSTR          szProcName;
        DWORD           dwOrdinal;
    };
} DelayLoadProc;

typedef struct DelayLoadInfo {
    DWORD               cb;         // size of structure
    PCImgDelayDescr     pidd;       // raw form of data (everything is there)
    FARPROC* ppfn;       // points to address of function to load
    LPCSTR              szDll;      // name of dll
    DelayLoadProc       dlp;        // name or ordinal of procedure
    HMODULE             hmodCur;    // the hInstance of the library we have loaded
    FARPROC             pfnCur;     // the actual function that will be called
    DWORD               dwLastError;// error received (if an error notification)
} DelayLoadInfo, * PDelayLoadInfo;

typedef FARPROC(WINAPI* PfnDliHook)(
    unsigned        dliNotify,
    PDelayLoadInfo  pdli
);

static FARPROC WINAPI DelayLoadFailureHook(unsigned dliNotify, PDelayLoadInfo pdli) {
    if (dliNotify == dliFailLoadLib) {
        dlog::error("Failed to load library: {}", pdli->szDll);
    } else if (dliNotify == dliFailGetProc) {
        dlog::error("Failed to get procedure: {}", pdli->dlp.szProcName);
    }

    return nullptr;
}

static FARPROC WINAPI DelayLoadNotifyHook(unsigned dliNotify, PDelayLoadInfo pdli) {
    if (dliNotify == dliNotePreLoadLibrary && pdli->szDll == std::string("cimgui.dll")) {
        dlog::debug("Loading cimgui.dll");

#ifdef _DEBUG
        return (FARPROC)LoadLibraryA("nativePC/plugins/CSharp/Loader/cimgui.debug.dll");
#else
        return (FARPROC)LoadLibraryA("nativePC/plugins/CSharp/Loader/cimgui.dll");
#endif
    }

    return nullptr;
}

ExternC PfnDliHook __pfnDliNotifyHook2 = DelayLoadNotifyHook;
ExternC PfnDliHook __pfnDliFailureHook2 = DelayLoadFailureHook;
