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

struct PixelInput
{
    float4 position : SV_Position;
    float3 normal : NORMAL0;
    float2 texCoord : TEXCOORD0;
    float depth : TEXCOORD1;
    float3 worldPosition : TEXCOORD2;
    float4 color : COLOR0;
    float4 modelPassParams : TEXCOORD3; // x = unlit, y = unfogged
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

Texture2D baseTexture : register(t0);
SamplerState baseSampler : register(s0);

float4 main(PixelInput input) : SV_Target {
    float4 color = baseTexture.Sample(baseSampler, input.texCoord);
    float3 lightColor = getDiffuseLight(input.normal);
    lightColor.rgb = saturate(lightColor.rgb);
   
    float unlit = input.modelPassParams.x;
    color.rgb *= unlit * lightColor + (1.0 - unlit) * float4(1, 1, 1, 1);

    float fogDepth = input.depth - fogParams.x;
    fogDepth /= (fogParams.y - fogParams.x);
    float fog = pow(saturate(fogDepth), 1.5) * input.modelPassParams.y;

    color.rgb = fog * fogColor.rgb + (1.0 - fog) * color.rgb;
	return input.color * color;
}

float4 main_blend(PixelInput input) : SV_Target{
    float4 color = baseTexture.Sample(baseSampler, input.texCoord);
    if (color.a < 3.0f / 255.0f)
        discard;

    float3 lightColor = getDiffuseLight(input.normal);
    lightColor.rgb = saturate(lightColor.rgb);

    float unlit = input.modelPassParams.x;
    color.rgb *= unlit * lightColor + (1.0 - unlit) * float4(1, 1, 1, 1);

    float fogDepth = input.depth - fogParams.x;
    fogDepth /= (fogParams.y - fogParams.x);
    float fog = pow(saturate(fogDepth), 1.5) * input.modelPassParams.y;

    color.rgb = fog * fogColor.rgb + (1.0 - fog) * color.rgb;
	return input.color * color;
}

float4 main_blend_alpha_test(PixelInput input) : SV_Target{
    float4 color = baseTexture.Sample(baseSampler, input.texCoord);
    if (color.a < 0.5)
        discard;

    float3 lightColor = getDiffuseLight(input.normal);
    lightColor.rgb = saturate(lightColor.rgb);

    float unlit = input.modelPassParams.x;
    color.rgb *= unlit * lightColor + (1.0 - unlit) * float4(1, 1, 1, 1);

    float fogDepth = input.depth - fogParams.x;
    fogDepth /= (fogParams.y - fogParams.x);
    float fog = pow(saturate(fogDepth), 1.5) * input.modelPassParams.y;

    color.rgb = fog * fogColor.rgb + (1.0 - fog) * color.rgb;
    return input.color * color;
}
