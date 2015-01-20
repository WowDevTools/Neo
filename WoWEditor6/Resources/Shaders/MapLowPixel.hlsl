cbuffer GlobalParamsBuffer : register(b0)
{
	float4 ambientLight;
	float4 diffuseLight;
	float4 fogColor;
	float4 fogParams;
};

float4 main(float4 position : SV_Position, float depth : TEXCOORD0) : SV_Target{
	clip((depth < fogParams.y) ? -1 : 1);
	return float4(fogColor.rgb, 1.0);
}