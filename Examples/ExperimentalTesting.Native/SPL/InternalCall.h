#pragma once

#ifndef SPL_INTERNAL_CALL
#define SPL_INTERNAL_CALL extern "C" __declspec(dllexport)
#endif

namespace SharpPluginLoader::Native {

struct InternalCall {
    const char* Name;
    void* Function;
};

}