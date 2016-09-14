// SkyPixel.hlsl

Texture2D skyTexture : register(t0);
SamplerState skySampler : register(s0);

struct PixelInput
{
    float4 position : SV_Position;
    float2 texCoord : TEXCOORD0;
};

float4 main(PixelInput input) : SV_Target
{
    return skyTexture.Sample(skySampler, input.texCoord);
}
