// TexturedQuadPixel.hlsl

struct PixelInput
{
    float4 position : SV_Position;
    float2 texCoord : TEXCOORD0;
};

Texture2D quadTexture : register(t0);
SamplerState quadSampler : register(s0);

float4 main(PixelInput input) : SV_Target
{
    return quadTexture.Sample(quadSampler, input.texCoord);
}
