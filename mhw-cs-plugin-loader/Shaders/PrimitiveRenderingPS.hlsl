
struct VSInput {
    float4 Position : SV_POSITION;
    float4 Color : COLOR;
};

float4 main(VSInput input) : SV_TARGET {
    return input.Color;
}
