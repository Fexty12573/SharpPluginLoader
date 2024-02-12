
struct VS_INPUT
{
    float4 Position : POSITION;
    float4 Color : COLOR;
};

struct VS_OUTPUT
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR;
};

VS_OUTPUT main(VS_INPUT input)
{
    // The vertex shader doesn't do anything,
    // it just passes the input to the next stage (geometry shader)
    VS_OUTPUT output;
    output.Position = input.Position;
    output.Color = input.Color;
    return output;
}