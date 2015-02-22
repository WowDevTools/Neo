cbuffer GlobalParams : register(b0)
{
    float4x4 matView;
    float4x4 matProj;

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

cbuffer PerDrawCallBuffer : register(b1)
{
    row_major float4x4 matTransform;
}

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
    float4 position = float4(input.position, 1);
    position = mul(position, matTransform);
    position = mul(position, matView);
    position = mul(position, matProj);

    VertexOutput output = (VertexOutput) 0;
    output.position = position;
    output.texCoord = input.texCoord;
    return output;
}
