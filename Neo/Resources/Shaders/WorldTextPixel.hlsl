// WorldTextPixel.hlsl

struct PixelInput
{
    float4 position : SV_Position;
    float2 texCoord : TEXCOORD0;
};

Texture2D textTexture : register(t0);
SamplerState textSampler : register(s0);

float4 main(PixelInput input) : SV_Target
{
    return textTexture.Sample(textSampler, input.texCoord);
}
