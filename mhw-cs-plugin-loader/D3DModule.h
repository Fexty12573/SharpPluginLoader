#pragma once
#include "NativeModule.h"

class D3DModule final : NativeModule {
public:
    void initialize(CoreClr* coreclr) override;
    void shutdown() override;

private:
    bool m_is_d3d12 = false;

    static constexpr const char* s_game_window_name = "MONSTER HUNTER: WORLD(421652)";
};

