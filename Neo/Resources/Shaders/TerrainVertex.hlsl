// TerrainVertex.hlsl

cbuffer GlobalParams : register(b0)
{
    row_major float4x4 matView;
    row_major float4x4 matProj;
    float4 viewport;

    float4 ambientLight;
    float4 diffuseLight;

    float4 fogColor;
    // x -> fogStart
    // y -> fotEnd
    // z -> farClip
    float4 fogParams;

    float4 mousePosition;
    float4 eyePosition;

    // x -> innerRadius
    // y -> outerRadius
    // z -> brushTime
    float4 brushParams;
};

cbuffer TexAnimBuffer : register(b1)
{
	row_major float4x4 texAnim0;
	row_major float4x4 texAnim1;
	row_major float4x4 texAnim2;
	row_major float4x4 texAnim3;
};

struct VertexInput
{
    float3 position : POSITION0;
    float3 normal : NORMAL0;
    float2 texCoord : TEXCOORD0;
    float2 texCoordAlpha : TEXCOORD1;
    float4 color : COLOR0;
    float4 addColor : COLOR1;
};

struct VertexOutput
{
    float4 position : SV_Position;
    float3 normal : NORMAL0;
    float2 texCoord0 : TEXCOORD0;
	float2 texCoord1 : TEXCOORD1;
	float2 texCoord2 : TEXCOORD2;
	float2 texCoord3 : TEXCOORD3;
    float2 texCoordAlpha : TEXCOORD4;
    float4 color : COLOR0;
    float4 addColor : COLOR1;
    float depth : TEXCOORD5;
    float3 worldPosition : TEXCOORD6;
};

VertexOutput main(VertexInput input)
{
    VertexOutput output = (VertexOutput) 0;

    float4 position = float4(input.position, 1.0);
    position = mul(position, matView);
    position = mul(position, matProj);

    output.position = position;
    output.normal = input.normal;

	float4 tc0 = mul(float4(input.texCoord, 0, 1), texAnim0);
	float4 tc1 = mul(float4(input.texCoord, 0, 1), texAnim1);
	float4 tc2 = mul(float4(input.texCoord, 0, 1), texAnim2);
	float4 tc3 = mul(float4(input.texCoord, 0, 1), texAnim3);
    output.texCoord0 = tc0.xy / tc0.w;
	output.texCoord1 = tc1.xy / tc1.w;
	output.texCoord2 = tc2.xy / tc2.w;
	output.texCoord3 = tc3.xy / tc3.w;

    output.texCoordAlpha = input.texCoordAlpha;
    output.color = input.color;
    output.depth = length(input.position - eyePosition.xyz);
    output.worldPosition = input.position;
    output.addColor = input.addColor;

    return output;
}
