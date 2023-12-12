#pragma once
#include "Log.h"

#include <intsafe.h>

class HResultHandler {
public:
    static inline void Handle(HRESULT hr, const char* file, int line) {
        if (FAILED(hr)) {
            dlog::error("HRESULT failed: 0x{:X} at {}:{}", hr, file, line);
            std::terminate();
        }
    }

    static inline void Handle(HRESULT hr, const char* file, int line, const char* msg) {
        if (FAILED(hr)) {
            dlog::error("HRESULT failed: 0x{:X} at {}:{}", hr, file, line, msg);
            dlog::error("Message: {}", msg);
            std::terminate();
        }
    }
};

#define HandleResult(hr) HResultHandler::Handle(hr, __FILE__, __LINE__)
#define HandleResultMsg(hr, msg) HResultHandler::Handle(hr, __FILE__, __LINE__, msg)
