cbuffer GlobalParamsBuffer : register(b0)
{
    float4 ambientLight;
    float4 diffuseLight;
    float4 fogColor;
    // x -> fogStart
    // y -> fotEnd
    // z -> farClip
    float4 fogParams;
};

float4 main(float4 position : SV_Position, float depth : TEXCOORD0) : SV_Target{
    clip(depth - (fogParams.y / fogParams.z));
    return float4(fogColor.rgb, 1.0);
}