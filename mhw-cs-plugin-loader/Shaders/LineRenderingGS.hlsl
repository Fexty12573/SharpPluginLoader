
struct GS_INPUT
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR;
};

typedef GS_INPUT GS_OUTPUT;

cbuffer ViewProj
{
	matrix View;
	matrix Proj;
};

cbuffer LineParams
{
	float LineWidth;
};

[maxvertexcount(4)]
void main(line GS_INPUT input[2], inout TriangleStream<GS_OUTPUT> stream)
{
    float4 p0 = mul(mul(input[0].Position, View), Proj);
	float4 p1 = mul(mul(input[1].Position, View), Proj);
	
	float2 dir = normalize(p1.xy - p0.xy);
	const float2 normal = float2(-dir.y, dir.x);
	const float4 offset = float4(normal * LineWidth, 0, 0);

	const float4 quad_vertices[4] = {
		p0 + offset,
		p0 - offset,
		p1 + offset,
		p1 - offset
    };

	GS_OUTPUT output;
	output.Color = input[0].Color;
	output.Position = quad_vertices[0];
	stream.Append(output);

	output.Position = quad_vertices[1];
	stream.Append(output);
	
	output.Color = input[1].Color;
	output.Position = quad_vertices[2];
	stream.Append(output);

	output.Position = quad_vertices[3];
	stream.Append(output);
}