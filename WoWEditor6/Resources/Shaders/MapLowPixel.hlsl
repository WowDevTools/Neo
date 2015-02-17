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

float4 main(float4 position : SV_Position, float depth : TEXCOORD0) : SV_Target{
    clip(depth - (fogParams.y / fogParams.z));
    return float4(fogColor.rgb, 1.0);
}