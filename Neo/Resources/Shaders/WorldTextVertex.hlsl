// WorldTextVertex.hlsl

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

VertexOutput main(VertexInput input)
{
    float4 position = float4(input.position, 1);
    position = mul(position, matTransform);
    position = mul(position, matView);
    position = mul(position, matProj);

    VertexOutput output = (VertexOutput) 0;
    output.position = position;
    output.texCoord.x = input.texCoord.x;
    output.texCoord.y = 1.0f - input.texCoord.y;
    return output;
}

VertexOutput main_orthographic(VertexInput input)
{
    float4 position = float4(input.position, 1);
    position = mul(position, matTransform);
    position.x = (position.x * 2.0f) / viewport.x - 1.0f;
    position.y = 1.0f - (position.y * 2.0f) / viewport.y;

    VertexOutput output = (VertexOutput) 0;
    output.position = position;
    output.texCoord = input.texCoord;
    return output;
}
