
struct VSInput {
    float4 Position : POSITION;
    matrix Transform : TRANSFORM;
    float4 Color : COLOR;
};

struct VSOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR;
};

cbuffer ViewProj {
    matrix View;
    matrix Proj;
};

VSOutput main(VSInput input) {
    VSOutput output;
    const float4 pos = mul(input.Position, input.Transform);
    output.Position = mul(pos, View);
    output.Position = mul(output.Position, Proj);
    output.Color = input.Color;

    return output;
}
