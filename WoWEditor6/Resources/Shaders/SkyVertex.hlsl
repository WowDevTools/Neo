cbuffer GlobalBuffer : register(b0)
{
    float4x4 matView;
    float4x4 matProj;
};

cbuffer MatrixBuffer : register(b1)
{
    float4 translation;
};

struct VertexInput
{
    float3 position : POSITION0;
    float2 texCoord : TEXCOORD0;
};

struct VertexOutput
{
    float4 position : SV_Position;
    float2 texCoord : TEXCOORD0;
};

VertexOutput main(VertexInput input) {
    float4 position = float4(input.position + translation.xyz, 1.0);
    position = mul(position, matView);
    position = mul(position, matProj);

    VertexOutput output = (VertexOutput) 0;
    output.position = position;
    output.texCoord = input.texCoord;

    return output;
}