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

struct PixelInput
{
    float4 position : SV_Position;
    float3 normal : NORMAL0;
    float2 texCoord : TEXCOORD0;
    float2 texCoordAlpha : TEXCOORD1;
    float4 color : COLOR0;
    float4 addColor : COLOR1;
    float depth : TEXCOORD2;
    float3 worldPosition : TEXCOORD3;
};

SamplerState alphaSampler : register(s1);
SamplerState colorSampler : register(s0);

Texture2D alphaTexture : register(t0);
Texture2D holeTexture : register(t1);
Texture2D texture0 : register(t2);
Texture2D texture1 : register(t3);
Texture2D texture2 : register(t4);
Texture2D texture3 : register(t5);

float3 sunDirection = float3(1, 1, -1);

cbuffer TextureScaleParams : register(b2)
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
    float dz = dirVec.z * dirVec.z;

    float innerRadius = brushParams.x * brushParams.x;
    float outerRadius = brushParams.y * brushParams.y;

    float fac = 1.0;
    float brushRotation = 0.0;
    float radius = outerRadius;

    // Is part of the inner circle?
    if (dsq < innerRadius && innerRadius * 0.95 < dsq) {
        brushRotation = 1.0;
        radius = innerRadius;
    }
    // Is part of the outer circle?
    else if (dsq < outerRadius && outerRadius * 0.95 < dsq) {
        brushRotation = -1.0;
        radius = outerRadius;
    }
    // Not part of anything
    else {
        fac = 0.0;
    }

    // Antialiasing for the circle borders
    float antiAliasSize = radius * 0.01;
    fac *= clamp((dsq - radius * 0.95) / antiAliasSize, 0, 1);
    fac *= clamp((radius - dsq) / antiAliasSize, 0, 1);

    float angle = atan2(dirVec.y, dirVec.x) + 3.1415926 * brushRotation;
    float brushTime = brushParams.z * brushRotation * 10;
    angle = fmod(abs(degrees(angle) + brushTime), 36.0f);

    // Antialiasing between the circle segments
    fac *= clamp((18.0 - angle) / 0.4, 0, 1);
    fac *= clamp((angle - 0.4)  / 0.4, 0, 1);

    fac *= clamp(1 - dz / 2000, 0, 1);

    float4 brushColor = float4(1, 1, 1, 1);
    brushColor.rgb -= color.rgb;
    return sinusInterpolate(color, brushColor, fac);
}

float3 getDiffuseLight(float3 normal, float3 worldPos) {
    float3 lightDir = normalize(-float3(-1, 1, -1));
    normal = normalize(normal);
    float light = dot(normal, lightDir);
    if (light < 0.0)
        light = 0.0;
    if (light > 0.5)
        light = 0.5 + (light - 0.5) * 0.65;

    float3 r = normalize(2 * dot(normal, -lightDir) * normal - lightDir);
    float3 v = normalize(normalize(worldPos - eyePosition.xyz));

    float rdv = dot(r, v);
    float3 specular = float3(1, 1, 1) * max(pow(rdv, 8), 0) * 0.2;

    float3 diffuse = diffuseLight.rgb * light;
    diffuse += ambientLight.rgb;
    diffuse += specular;
    diffuse = saturate(diffuse);
    return diffuse;
}

float4 main(PixelInput input) : SV_Target{
    float4 alpha = alphaTexture.Sample(alphaSampler, input.texCoordAlpha);
    float holeValue = holeTexture.Sample(alphaSampler, input.texCoordAlpha).r;
    if (holeValue < 0.5)
        discard;

    float4 c0 = texture0.Sample(colorSampler, input.texCoord.yx * texScales.x);
    float4 c1 = texture1.Sample(colorSampler, input.texCoord.yx * texScales.y);
    float4 c2 = texture2.Sample(colorSampler, input.texCoord.yx * texScales.z);
    float4 c3 = texture3.Sample(colorSampler, input.texCoord.yx * texScales.a);

    float4 color = (1.0 - (alpha.g + alpha.b + alpha.a)) * c0;
    color += alpha.g * c1;
    color += alpha.b * c2;
    color += alpha.a * c3;

    float4 textureColor = color;

    color.rgb *= getDiffuseLight(input.normal, input.worldPosition);
    color.rgb *= input.color.bgr * 2;
    color.rgb += input.addColor.bgr * textureColor;
    color.rgb *= alpha.r;
    color.rgb = saturate(color.rgb);

    float fogDepth = input.depth - fogParams.x;
    fogDepth /= (fogParams.y - fogParams.x);
    float fog = 1.0f - pow(saturate(fogDepth), 1.5);

    color.rgb = (1.0 - fog) * fogColor.rgb + fog * color.rgb;

    color = applyBrush(color, input.worldPosition);
    color.a = holeValue;

    return color;
}