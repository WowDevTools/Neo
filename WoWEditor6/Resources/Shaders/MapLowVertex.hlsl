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