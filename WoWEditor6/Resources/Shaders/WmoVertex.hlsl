cbuffer MatrixBuffer : register(b0)
{
	float4x4 matView;
	float4x4 matProj;
	float4 eyePosition;
};

cbuffer InstanceBuffer : register(b2)
{
	float4x4 matInstance;
};

struct VSInput
{
	float3 position : POSITION0;
	float3 normal : NORMAL0;
	float2 texCoord : TEXCOORD0;
	float4 color : COLOR0;
};

struct VSOutput
{
	float4 position : SV_Position;
	float3 normal : NORMAL0;
	float2 texCoord : TEXCOORD0;
	float depth : TEXCOORD1;
	float4 color : COLOR0;
};

VSOutput main(VSInput input) {
	VSOutput output = (VSOutput) 0;

	output.position = float4(input.position, 1.0);
	float4 posTransformed = mul(output.position, matInstance);
	output.position = posTransformed;
	output.position = mul(output.position, matView);
	output.position = mul(output.position, matProj);

	output.normal = normalize(mul(input.normal, (float3x3) matInstance));
	output.texCoord = input.texCoord;
	output.color = input.color;

	output.depth = length(input.position - eyePosition.xyz);

	return output;
}