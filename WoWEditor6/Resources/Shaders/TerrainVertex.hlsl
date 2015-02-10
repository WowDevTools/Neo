struct VertexInput
{
	float3 position : POSITION0;
	float3 normal : NORMAL0;
	float2 texCoord : TEXCOORD0;
	float2 texCoordAlpha : TEXCOORD1;
	float4 color : COLOR0;
};

struct VertexOutput
{
	float4 position : SV_Position;
	float3 normal : NORMAL0;
	float2 texCoord : TEXCOORD0;
	float2 texCoordAlpha : TEXCOORD1;
	float4 color : COLOR0;
	float depth : TEXCOORD2;
	float3 worldPosition : TEXCOORD3;
};

cbuffer GlobalParams : register(b0)
{
	float4x4 matView;
	float4x4 matProj;
	float4 eyePosition;
};

VertexOutput main(VertexInput input) {
	VertexOutput output = (VertexOutput) 0;

	float4 position = float4(input.position, 1.0);
	position = mul(position, matView);
	position = mul(position, matProj);

	output.position = position;
	output.normal = input.normal;
	output.texCoord = input.texCoord;
	output.texCoordAlpha = input.texCoordAlpha;
	output.color = input.color;
	output.depth = length(input.position - eyePosition.xyz);
	output.worldPosition = input.position;

	return output;
}