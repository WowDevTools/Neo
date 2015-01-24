struct PixelInput
{
	float4 position : SV_Position;
	float3 normal : NORMAL0;
	float2 texCoord : TEXCOORD0;
	float depth : TEXCOORD1;
};

float4 main(PixelInput input) : SV_Target {
	return float4(1, 1, 1, 1);
}