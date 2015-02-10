Texture2D skyTexture : register(t0);
SamplerState skySampler : register(s0);

struct PSInput
{
	float4 position : SV_Position;
	float2 texCoord : TEXCOORD0;
};

float4 main(PSInput input) : SV_Target {
	return skyTexture.Sample(skySampler, input.texCoord);
}