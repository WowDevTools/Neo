struct PixelInput
{
	float4 position : SV_Position;
	float3 normal : NORMAL0;
	float2 texCoord : TEXCOORD0;
};

float3 getDiffuseLight(float3 normal) {
	float light = dot(normal, normalize(-float3(-1, 1, -1)));
	if (light < 0.0)
		light = 0.0;
	if (light > 0.5)
		light = 0.5 + (light - 0.5) * 0.65;

	float3 diffuse = float3(0.7, 0.7, 0.7) * light;
	diffuse += float3(0.3, 0.3, 0.3);
	diffuse = saturate(diffuse);
	return diffuse;
}

Texture2D baseTexture : register(t0);
SamplerState baseSampler : register(s0);

float4 main(PixelInput input) : SV_Target{
	float4 color = baseTexture.Sample(baseSampler, input.texCoord);
	float3 lightColor = getDiffuseLight(input.normal);
	lightColor.rgb = saturate(lightColor.rgb);
	color.rgb *= lightColor;

	return color;

}

float4 main_blend(PixelInput input) : SV_Target{
	float4 color = baseTexture.Sample(baseSampler, input.texCoord);
	if (color.a < 3.0f / 255.0f)
		discard;

	float3 lightColor = getDiffuseLight(input.normal);
	lightColor.rgb = saturate(lightColor.rgb);
	color.rgb *= lightColor;

	return color;

}