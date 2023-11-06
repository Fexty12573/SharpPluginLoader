#pragma once
#include <any>
#include <memory>

class CoreClr;

class NativeModule {
public:
    virtual void initialize(CoreClr* coreclr) = 0;
    virtual void shutdown() = 0;
    virtual ~NativeModule() = default;

protected:
    NativeModule() = default;
    NativeModule(const NativeModule&) = default;
    NativeModule(NativeModule&&) = default;
    NativeModule& operator=(const NativeModule&) = default;
    NativeModule& operator=(NativeModule&&) = default;
};
