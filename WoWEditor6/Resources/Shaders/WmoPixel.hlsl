// WmoPixel.hlsl

cbuffer GlobalParams : register(b0)
{
    row_major float4x4 matView;
    row_major float4x4 matProj;
    float4 viewport;

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

Texture2D batchTexture : register(t0);
SamplerState batchSampler : register(s0);

struct PixelInput
{
    float4 position : SV_Position;
    float3 normal : NORMAL0;
    float2 texCoord : TEXCOORD0;
    float depth : TEXCOORD1;
    float4 color : COLOR0;
    float3 worldPosition : TEXCOORD2;
};

float3 getDiffuseLight(float3 normal)
{
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

float4 main(PixelInput input) : SV_Target
{
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

float4 main_indoor(PixelInput input) : SV_Target
{
    float4 color = batchTexture.Sample(batchSampler, input.texCoord);
    float3 lightColor = getDiffuseLight(input.normal);
    float3 groupColor = input.color.bgr;
    float3 finalColor = input.color.a * lightColor + (1 - input.color.a) * groupColor;
    finalColor = saturate(finalColor);
    color.rgb *= finalColor;

    float fogDepth = input.depth - fogParams.x;
    fogDepth /= (fogParams.y - fogParams.x);
    float fog = 1.0f - pow(saturate(fogDepth), 1.5);

    color.rgb = (1.0 - fog) * fogColor.rgb + fog * color.rgb;
    return color;
}

float4 main_blend(PixelInput input) : SV_Target
{
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

float4 main_blend_indoor(PixelInput input) : SV_Target
{
    float4 color = batchTexture.Sample(batchSampler, input.texCoord);
    if (color.a < (5 / 255.0f))
        discard;

    float3 lightColor = getDiffuseLight(input.normal);
    float3 groupColor = input.color.bgr;
    float3 finalColor = input.color.a * lightColor + (1 - input.color.a) * groupColor;
    finalColor = saturate(finalColor);
    color.rgb *= finalColor;

    float fogDepth = input.depth - fogParams.x;
    fogDepth /= (fogParams.y - fogParams.x);
    float fog = 1.0f - pow(saturate(fogDepth), 1.5);

    color.rgb = (1.0 - fog) * fogColor.rgb + fog * color.rgb;
    return color;
}
