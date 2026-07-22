#pragma once

enum ImGui_ImplDXGI_ColorSpace
{
    ImGui_ImplDXGI_ColorSpace_SDR = 0,
    ImGui_ImplDXGI_ColorSpace_scRGB,
    ImGui_ImplDXGI_ColorSpace_HDR10,
};

inline const char* ImGui_ImplDXGI_GetColorSpaceShaderDefine(ImGui_ImplDXGI_ColorSpace color_space)
{
    switch (color_space)
    {
    case ImGui_ImplDXGI_ColorSpace_scRGB:
        return "1";
    case ImGui_ImplDXGI_ColorSpace_HDR10:
        return "2";
    default:
        return "0";
    }
}

// ImGui colors and textures are treated as sRGB/Rec.709. HDR output uses a
// 203-nit graphics white, as recommended by BT.2408, while SDR output remains
// byte-for-byte compatible with the stock Dear ImGui renderer backend.
inline constexpr const char* ImGui_ImplDXGI_PixelShader = R"(
#ifndef IMGUI_COLOR_SPACE
#define IMGUI_COLOR_SPACE 0
#endif

struct PS_INPUT
{
    float4 pos : SV_POSITION;
    float4 col : COLOR0;
    float2 uv  : TEXCOORD0;
};

SamplerState sampler0 : register(s0);
Texture2D texture0 : register(t0);

static const float GRAPHICS_WHITE_NITS = 203.0;

float3 SrgbToLinear(float3 color)
{
    float3 low = color / 12.92;
    float3 high = pow((color + 0.055) / 1.055, 2.4);
    return lerp(low, high, step(0.04045, color));
}

float3 Rec709ToRec2020(float3 color)
{
    const float3x3 transform =
    {
        0.6274040, 0.3292820, 0.0433136,
        0.0690970, 0.9195400, 0.0113612,
        0.0163916, 0.0880132, 0.8955950
    };
    return mul(transform, color);
}

float3 LinearToPq(float3 color)
{
    const float m1 = 2610.0 / 16384.0;
    const float m2 = 2523.0 / 32.0;
    const float c1 = 3424.0 / 4096.0;
    const float c2 = 2413.0 / 128.0;
    const float c3 = 2392.0 / 128.0;

    float3 p = pow(max(color, 0.0), m1);
    return pow((c1 + c2 * p) / (1.0 + c3 * p), m2);
}

float4 main(PS_INPUT input) : SV_Target
{
    float4 out_col = input.col * texture0.Sample(sampler0, input.uv);

#if IMGUI_COLOR_SPACE == 1
    // scRGB is linear Rec.709 and defines 1.0 as 80 nits.
    out_col.rgb = SrgbToLinear(saturate(out_col.rgb)) * (GRAPHICS_WHITE_NITS / 80.0);
#elif IMGUI_COLOR_SPACE == 2
    // HDR10 uses Rec.2020 primaries and the absolute ST.2084/PQ transfer curve.
    float3 linear_rec709 = SrgbToLinear(saturate(out_col.rgb));
    float3 linear_rec2020 = Rec709ToRec2020(linear_rec709);
    out_col.rgb = LinearToPq(linear_rec2020 * (GRAPHICS_WHITE_NITS / 10000.0));
#endif

    return out_col;
}
)";
