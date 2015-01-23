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

Texture2D batchTexture : register(t0);
SamplerState batchSampler : register(s0);

struct PSInput
{
	float4 position : SV_Position;
	float3 normal : NORMAL0;
	float2 texCoord : TEXCOORD0;
	float depth : TEXCOORD1;
	float4 color : COLOR0;
};

float3 getDiffuseLight(float3 normal) {
	float light = dot(normal, normalize(-float3(-1, 1, -1)));
	if (light < 0.0)
		light = 0.0;
	if (light > 0.5)
		light = 0.5 + (light - 0.5) * 0.65;

	float3 diffuse = diffuseLight.rgb * light;
	diffuse += ambientLight.rgb;
	diffuse = saturate(diffuse);
	return diffuse;
}

float4 main(PSInput input) : SV_Target{
	float4 color = batchTexture.Sample(batchSampler, input.texCoord);
	float3 lightColor = getDiffuseLight(input.normal);
	lightColor.rgb += input.color.bgr * 2;
	lightColor.rgb = saturate(lightColor.rgb);
	color.rgb *= lightColor;

	float fogDepth = input.depth - fogParams.x;
	fogDepth /= (fogParams.y - fogParams.x);
	float fog = 1.0f - pow(saturate(fogDepth), 1.5);

	color.rgb = (1.0 - fog) * fogColor.rgb + fog * color.rgb;

	return color;
}

float4 main_blend(PSInput input) : SV_Target {
	float4 color = batchTexture.Sample(batchSampler, input.texCoord);
	if (color.a < (5.0f / 255.0f))
		discard;

	float3 lightColor = getDiffuseLight(input.normal);
	lightColor.rgb += input.color.bgr * 2;
	lightColor.rgb = saturate(lightColor.rgb);
	color.rgb *= lightColor;

	float fogDepth = input.depth - fogParams.x;
	fogDepth /= (fogParams.y - fogParams.x);
	float fog = 1.0f - pow(saturate(fogDepth), 1.5);

	color.rgb = (1.0 - fog) * fogColor.rgb + fog * color.rgb;

	return color;
}