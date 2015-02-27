// MapLowPixel.hlsl

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

struct PixelInput
{
    float4 position : SV_Position;
    float depth : TEXCOORD0;
};

float4 main(PixelInput input) : SV_Target
{
    clip(input.depth - (fogParams.y / fogParams.z));
    return float4(fogColor.rgb, 1.0);
}
