struct PixelInput
{
    float4 position : SV_Position;
    float3 normal : NORMAL0;
    float2 texCoord : TEXCOORD0;
    float2 texCoordAlpha : TEXCOORD1;
    float4 color : COLOR0;
    float depth : TEXCOORD2;
    float3 worldPosition : TEXCOORD3;
};

SamplerState alphaSampler : register(s1);
SamplerState colorSampler : register(s0);

Texture2D alphaTexture : register(t0);
Texture2D texture0 : register(t1);
Texture2D texture1 : register(t2);
Texture2D texture2 : register(t3);
Texture2D texture3 : register(t4);

float3 sunDirection = float3(1, 1, -1);

cbuffer GlobalParamsBuffer : register(b0)
{
    float4 ambientLight;
    float4 diffuseLight;
    float4 fogColor;
    float4 fogParams;
    float4 mousePosition;
    float4 brushParams;
};

cbuffer TextureParamsBuffer : register(b2)
{
    float4 texScales;
};

float4 sinusInterpolate(float4 src, float4 dst, float pct) {
    float sinval = sin(pct * 3.1415926 / 2.0f);
    return sinval * dst + (1 - sinval) * src;
}

float4 applyBrush(float4 color, float3 worldPos) {
    float3 dirVec = worldPos - mousePosition.xyz;
    float dsq = dot(dirVec.xy, dirVec.xy);

    float innerRadius = brushParams.x * brushParams.x;
    float outerRadius = brushParams.y * brushParams.y;
    float angle = atan2(dirVec.y, dirVec.x) + 3.141592;
    angle = fmod(degrees(angle), 36.0f);

    // Antialiasing between the circle segments
    float fac = 1.0;
    fac *= clamp((18.0 - angle) / 0.4, 0, 1);
    fac *= clamp((angle - 0.4)  / 0.4, 0, 1);

    // Antialiasing for the circle borders
    if (dsq < innerRadius && innerRadius * 0.95 < dsq) {
        float antiAliasSize = innerRadius * 0.01;
        fac *= clamp((dsq - innerRadius * 0.95) / antiAliasSize, 0, 1);
        fac *= clamp((innerRadius - dsq) / antiAliasSize, 0, 1);
    }
    else if (dsq < outerRadius && outerRadius * 0.95 < dsq) {
        float antiAliasSize = outerRadius * 0.01;
        fac *= clamp((dsq - outerRadius * 0.95) / antiAliasSize, 0, 1);
        fac *= clamp((outerRadius - dsq) / antiAliasSize, 0, 1);
    }
    else {
        fac = 0.0;
    }

    float4 brushColor = float4(1, 1, 1, 1);
    brushColor.rgb -= color.rgb;
    return sinusInterpolate(color, brushColor, fac);
}

float3 getDiffuseLight(float3 normal) {
    float light = dot(normalize(normal), normalize(-float3(-1, 1, -1)));
    if (light < 0.0)
        light = 0.0;
    if (light > 0.5)
        light = 0.5 + (light - 0.5) * 0.65;

    float3 diffuse = diffuseLight.rgb * light;
    diffuse += ambientLight.rgb;
    diffuse = saturate(diffuse);
    return diffuse;
}

float4 main(PixelInput input) : SV_Target{
    float4 alpha = alphaTexture.Sample(alphaSampler, input.texCoordAlpha);
    float4 c0 = texture0.Sample(colorSampler, input.texCoord * texScales.x);
    float4 c1 = texture1.Sample(colorSampler, input.texCoord * texScales.y);
    float4 c2 = texture2.Sample(colorSampler, input.texCoord * texScales.z);
    float4 c3 = texture3.Sample(colorSampler, input.texCoord * texScales.a);

    float4 color = c0 * alpha.r;
    color = c1 * alpha.g + (1.0 - alpha.g) * color;
    color = c2 * alpha.b + (1.0 - alpha.b) * color;
    color = c3 * alpha.a + (1.0 - alpha.a) * color;

    color.rgb *= input.color.bgr * 2;
    color.rgb *= getDiffuseLight(input.normal);

    float fogDepth = input.depth - fogParams.x;
    fogDepth /= (fogParams.y - fogParams.x);
    float fog = 1.0f - pow(saturate(fogDepth), 1.5);

    color.rgb = (1.0 - fog) * fogColor.rgb + fog * color.rgb;

    color = applyBrush(color, input.worldPosition);

    return color;
}