cbuffer GlobalParams : register(b0)
{
	float4x4 matView;
	float4x4 matProj;
	float4 eyePosition;
};

struct VSInput
{
	float3 position : POSITION0;
};

struct VSOutput
{
	float4 position : SV_Position;
	float depth : TEXCOORD0;
};

VSOutput main(VSInput input) {
	VSOutput output = (VSOutput) 0;
	output.position = float4(input.position, 1.0);
	output.position = mul(output.position, matView);
	output.position = mul(output.position, matProj);

	output.depth = output.position.z / output.position.w;

	return output;
}