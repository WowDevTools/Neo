// M2Pixel.hlsl

Texture2D texture1 : register(t0);
Texture2D texture2 : register(t1);
Texture2D texture3 : register(t2);
Texture2D texture4 : register(t3);

SamplerState sampler1 : register(s0);
SamplerState sampler2 : register(s1);
SamplerState sampler3 : register(s2);
SamplerState sampler4 : register(s3);

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

cbuffer PerModelPassBuffer : register(b1)
{
    row_major float4x4 uvAnimation1;
    row_major float4x4 uvAnimation2;
    row_major float4x4 uvAnimation3;
    row_major float4x4 uvAnimation4;
    float4 modelPassParams; // x = unlit, y = unfogged, z = alphakey
    float4 animatedColor;
    float4 transparency;
}

struct PixelInput
{
    float4 position : SV_Position;
    float3 normal : NORMAL0;
    float2 texCoord1 : TEXCOORD0;
    float2 texCoord2 : TEXCOORD1;
    float2 texCoord3 : TEXCOORD2;
    float2 texCoord4 : TEXCOORD3;
    float depth : TEXCOORD4;
    float3 worldPosition : TEXCOORD5;
    float4 color : COLOR0;
    float4 colorMod : COLOR1;
};

float3 getDiffuseLight(float3 normal)
{
    float3 lightDir = float3(-1, 1, -1);
    float light = saturate(dot(normal, normalize(-lightDir)));
    float3 diffuse = diffuseLight.rgb * light;
    diffuse += ambientLight.rgb;
    diffuse = saturate(diffuse);
    return diffuse;
}

static float4 combinedColor = float4(0.0f, 0.0f, 0.0f, 0.0f);
static float4 finalColor = float4(0.0f, 0.0f, 0.0f, 0.0f);

float3 applyFog(float3 finalColor, PixelInput input)
{
    float fogDepth = input.depth - fogParams.x;
    fogDepth /= (fogParams.y - fogParams.x);
    float fog = pow(saturate(fogDepth), 1.5f) * modelPassParams.y;
    return fog * fogColor.rgb + (1.0f - fog) * finalColor.rgb;
}

float4 commonFinalize(float4 finalizeColor, PixelInput input)
{
    if (modelPassParams.x) // not unlit
        finalizeColor.rgb *= getDiffuseLight(input.normal);

    finalizeColor.rgb = applyFog(finalizeColor.rgb, input);
    return finalizeColor * input.colorMod;
}

float4 main_PS_Combiners_Opaque(PixelInput input) : SV_Target
{
    float4 textureColor1 = texture1.Sample(sampler1, input.texCoord1);
    clip((modelPassParams.z && textureColor1.a <= 0.5f) ? -1 : 1);

    float4 r0 = textureColor1;
    r0.rgb *= input.color.rgb;
    r0.rgb *= 2.0f;

    combinedColor.rgb = r0.rgb;
    combinedColor.a = input.color.a;

    return commonFinalize(combinedColor, input);
}

float4 main_PS_Combiners_Mod(PixelInput input) : SV_Target
{
    float4 textureColor1 = texture1.Sample(sampler1, input.texCoord1);
	clip((modelPassParams.z && textureColor1.a <= 0.5f) ? -1 : 1);
    
    textureColor1.a *= transparency.x;

    float4 r0 = textureColor1;
    r0.rgb *= input.color.rgb;
    combinedColor.a = r0.a * input.color.a;
    r0.rgb *= 2.0f;
    combinedColor.rgb = r0.rgb;

    return commonFinalize(combinedColor, input);
}

float4 main_PS_Combiners_Opaque_Alpha(PixelInput input) : SV_Target
{
    float4 textureColor1 = texture1.Sample(sampler1, input.texCoord1);
	clip((modelPassParams.z && textureColor1.a <= 0.5f) ? -1 : 1);
    float4 textureColor2 = texture2.Sample(sampler2, input.texCoord2);

    textureColor1.a *= transparency.x;
    textureColor2.a *= transparency.y;

    float4 r0 = textureColor1; 
    float4 r1 = textureColor2;
    r1.rgb = -r0.rgb + r1.rgb; 
    r0.rgb = r1.a * r1.rgb + r0.rgb; 
    r0.rgb *= input.color.rgb;
    r0.rgb *= 2.0f; 
    combinedColor.rgb = r0.rgb; 

    return commonFinalize(combinedColor, input);
}

float4 main_PS_Combiners_Opaque_Mod(PixelInput input) : SV_Target
{
    float4 textureColor1 = texture1.Sample(sampler1, input.texCoord1);
	clip((modelPassParams.z && textureColor1.a <= 0.5f) ? -1 : 1);
    float4 textureColor2 = texture2.Sample(sampler2, input.texCoord2);

    textureColor1.a *= transparency.x;
    textureColor2.a *= transparency.y;

    float4 r0 = textureColor1; 
    float4 r1 = textureColor2;
    r0.rgb *= r1.rgb; 
    combinedColor.a = r1.a * input.color.a;
    r0.rgb *= input.color.rgb;
    r0.rgb *= 2.0f;
    combinedColor.rgb = r0.rgb;

    return commonFinalize(combinedColor, input);
}

float4 main_PS_Combiners_Opaque_Mod2x(PixelInput input) : SV_Target
{
    float4 textureColor1 = texture1.Sample(sampler1, input.texCoord1);
	clip((modelPassParams.z && textureColor1.a <= 0.5f) ? -1 : 1);
    float4 textureColor2 = texture2.Sample(sampler2, input.texCoord2);

    textureColor1.a *= transparency.x;
    textureColor2.a *= transparency.y;

    float4 r0 = textureColor1; 
    float4 r1 = textureColor2;
    r0.rgb *= r1.rgb;
    combinedColor.a = dot(input.color.aa, r1.aa);
    r0.rgb *= input.color.rgb;
    r0.rgb *= 4.0f;
    combinedColor.rgb = r0.rgb;

    return commonFinalize(combinedColor, input);
}

float4 main_PS_Combiners_Opaque_Mod2xNA(PixelInput input) : SV_Target
{
    float4 textureColor1 = texture1.Sample(sampler1, input.texCoord1);
	clip((modelPassParams.z && textureColor1.a <= 0.5f) ? -1 : 1);
    float4 textureColor2 = texture2.Sample(sampler2, input.texCoord2);

    textureColor1.a *= transparency.x;
    textureColor2.a *= transparency.y;

    float4 r0 = textureColor1;
    float4 r1 = textureColor2;
    r0.rgb *= r1.rgb;
    r0.rgb *= input.color.rgb;
    r0.rgb *= 4.0f;
    combinedColor.rgb = r0.rgb;
    combinedColor.a = input.color.a; 

    return commonFinalize(combinedColor, input);
}

float4 main_PS_Combiners_Opaque_Opaque(PixelInput input) : SV_Target
{
    float4 textureColor1 = texture1.Sample(sampler1, input.texCoord1);
	clip((modelPassParams.z && textureColor1.a <= 0.5f) ? -1 : 1);
    float4 textureColor2 = texture2.Sample(sampler2, input.texCoord2);

    textureColor1.a *= transparency.x;
    textureColor2.a *= transparency.y;

    float4 r0 = textureColor1;
    float4 r1 = textureColor2;
    r0.rgb *= r1.rgb;
    r0.rgb *= input.color.rgb * 0.5f;
    r0.rgb *= 2.0f;
    combinedColor.rgb = r0.rgb;
    combinedColor.a = input.color.a;

    return commonFinalize(combinedColor, input);
}

float4 main_PS_Combiners_Mod_Mod(PixelInput input) : SV_Target
{
    float4 textureColor1 = texture1.Sample(sampler1, input.texCoord1);
	clip((modelPassParams.z && textureColor1.a <= 0.5f) ? -1 : 1);
    float4 textureColor2 = texture2.Sample(sampler2, input.texCoord2);

    textureColor1.a *= transparency.x;
    textureColor2.a *= transparency.y;

    float4 r0 = textureColor1;
    float4 r1 = textureColor2;
    r0.rgb *= r1.rgb;
    r0.a *= input.color.a;
    combinedColor.a = r1.a * r0.a;
    r0.rgb *= input.color.rgb;
    r0.rgb *= 2.0f;
    combinedColor.rgb = r0.rgb;

    return commonFinalize(combinedColor, input);
}

float4 main_PS_Combiners_Mod_Mod2x(PixelInput input) : SV_Target
{
    float4 textureColor1 = texture1.Sample(sampler1, input.texCoord1);
	clip((modelPassParams.z && textureColor1.a <= 0.5f) ? -1 : 1);
    float4 textureColor2 = texture2.Sample(sampler2, input.texCoord2);

    textureColor1.a *= transparency.x;
    textureColor2.a *= transparency.y;

    float4 r0 = textureColor1;
    float4 r1 = textureColor2;
    r0.rgb *= r1.rgb;
    r0.a *= input.color.a;
    combinedColor.a = dot(r0.aa, r1.aa);
    r0.rgb *= input.color.rgb;
    r0.rgb *= 4.0f;
    combinedColor.rgb = r0.rgb;

    return commonFinalize(combinedColor, input);
}

float4 main_PS_Combiners_Mod_Add(PixelInput input) : SV_Target
{
    float4 textureColor1 = texture1.Sample(sampler1, input.texCoord1);
	clip((modelPassParams.z && textureColor1.a <= 0.5f) ? -1 : 1);
    float4 textureColor2 = texture2.Sample(sampler2, input.texCoord2);

    textureColor1.a *= transparency.x;
    textureColor2.a *= transparency.y;

    float4 r0 = textureColor1;
    r0.rgb *= input.color.rgb;
    float4 r1 = textureColor2;
    r0.rgb = r0.rgb * 2.0f + r1.rgb;
    combinedColor.a = input.color.a * r0.a * r1.a;
    combinedColor.rgb = r0.rgb; 

    return commonFinalize(combinedColor, input);
}

float4 main_PS_Combiners_Mod_Mod2xNA(PixelInput input) : SV_Target
{
    float4 textureColor1 = texture1.Sample(sampler1, input.texCoord1);
	clip((modelPassParams.z && textureColor1.a <= 0.5f) ? -1 : 1);
    float4 textureColor2 = texture2.Sample(sampler2, input.texCoord2);

    textureColor1.a *= transparency.x;
    textureColor2.a *= transparency.y;

    float4 r0 = textureColor2;
    float4 r1 = textureColor1;
    r0.rgb *= r1.rgb;
    combinedColor.a = r1.a * input.color.a;
    r0.rgb *= input.color.rgb;
    r0.rgb *= 4.0f;
    combinedColor.rgb = r0.rgb;

    return commonFinalize(combinedColor, input);
}

float4 main_PS_Combiners_Mod_AddNA(PixelInput input) : SV_Target
{
    float4 textureColor1 = texture1.Sample(sampler1, input.texCoord1);
	clip((modelPassParams.z && textureColor1.a <= 0.5f) ? -1 : 1);
    float4 textureColor2 = texture2.Sample(sampler2, input.texCoord2);

    textureColor1.a *= transparency.x;
    textureColor2.a *= transparency.y;

    float4 r0 = textureColor2;
    float4 r1 = textureColor1;
    r1.rgb *= input.color.rgb;
    combinedColor.a = r1.a * input.color.a;
    r0.rgb = r1.rgb * 2.0f + r0.rgb;
    combinedColor.rgb = r0.rgb;

    return commonFinalize(combinedColor, input);
}

float4 main_PS_Combiners_Mod_Opaque(PixelInput input) : SV_Target
{
    float4 textureColor1 = texture1.Sample(sampler1, input.texCoord1);
	clip((modelPassParams.z && textureColor1.a <= 0.5f) ? -1 : 1);
    float4 textureColor2 = texture2.Sample(sampler2, input.texCoord2);

    textureColor1.a *= transparency.x;
    textureColor2.a *= transparency.y;

    float4 r0 = textureColor2;
    float4 r1 = textureColor1;
    r0.rgb *= r1.rgb;
    combinedColor.a = r1.a * input.color.a;
    r0.rgb *= input.color.rgb;
    r0.rgb *= 2.0f;
    combinedColor.rgb = r0.rgb;

    return commonFinalize(combinedColor, input);
}

float4 main_PS_Combiners_Opaque_AddAlpha(PixelInput input) : SV_Target
{
    float4 textureColor1 = texture1.Sample(sampler1, input.texCoord1);
	clip((modelPassParams.z && textureColor1.a <= 0.5f) ? -1 : 1);
    float4 textureColor2 = texture2.Sample(sampler2, input.texCoord2);

    textureColor1.a *= transparency.x;
    textureColor2.a *= transparency.y;

    float4 r0 = textureColor1;
    r0.rgb *= input.color.rgb;
    r0.rgb += r0.rgb;
    float4 r1 = textureColor2;
    r0.rgb = r1.rgb * r1.a + r0.rgb;
    combinedColor.rgb = r0.rgb;
    combinedColor.a = input.color.a;

    return commonFinalize(combinedColor, input);
}

float4 main_PS_Combiners_Opaque_Alpha_Alpha(PixelInput input) : SV_Target
{
    float4 textureColor1 = texture1.Sample(sampler1, input.texCoord1);
	clip((modelPassParams.z && textureColor1.a <= 0.5f) ? -1 : 1);
    float4 textureColor2 = texture2.Sample(sampler2, input.texCoord2);

    textureColor1.a *= transparency.x;
    textureColor2.a *= transparency.y;

    float4 r0 = textureColor2;
    float4 r1 = textureColor1;
    r0.rgb = r0.rgb - r1.rgb;
    r0.rgb = r0.a * r0.rgb + r1.rgb;
    r1.rgb = -r0.rgb + r1.rgb;
    r0.rgb = r1.a * r1.rgb + r0.rgb;
    r0.rgb *= input.color.rgb;
    r0.rgb *= 2.0f;
    combinedColor.rgb = r0.rgb;
    combinedColor.a = input.color.a; 

    return commonFinalize(combinedColor, input);
}


float4 main_PS_Combiners_Opaque_AddAlpha_Alpha(PixelInput input) : SV_Target
{
    float4 textureColor1 = texture1.Sample(sampler1, input.texCoord1);
	clip((modelPassParams.z && textureColor1.a <= 0.5f) ? -1 : 1);
    float4 textureColor2 = texture2.Sample(sampler2, input.texCoord2);

    textureColor1.a *= transparency.x;
    textureColor2.a *= transparency.y;

    float4 r0 = textureColor2;
    r0.rgb = r0.a * r0.rgb;
    float4 r1 = textureColor1;
    r1.rgb *= input.color.rgb;
    r0.a = -r1.a + 1;
    r1.rgb += r1.rgb;
    r0.rgb = r0.rgb * r0.a + r1.rgb;
    combinedColor.rgb = r0.rgb;
    combinedColor.a = input.color.a;

    return commonFinalize(combinedColor, input);
}

float4 main_PS_Combiners_Mod_AddAlpha(PixelInput input) : SV_Target
{
    float4 textureColor1 = texture1.Sample(sampler1, input.texCoord1);
	clip((modelPassParams.z && textureColor1.a <= 0.5f) ? -1 : 1);
    float4 textureColor2 = texture2.Sample(sampler2, input.texCoord2);

    textureColor1.a *= transparency.x;
    textureColor2.a *= transparency.y;

    float4 r0 = textureColor2;
    float4 r1 = textureColor1;
    r1.rgb *= input.color.rgb;
    combinedColor.a = r1.a * input.color.a;
    r1.rgb += r1.rgb;
    r0.rgb = r0.rgb * r0.a + r1.rgb;
    combinedColor.rgb = r0.rgb;

    return commonFinalize(combinedColor, input);
}

float4 main_PS_Combiners_Mod_Add_Alpha(PixelInput input) : SV_Target
{
    float4 textureColor1 = texture1.Sample(sampler1, input.texCoord1);
	clip((modelPassParams.z && textureColor1.a <= 0.5f) ? -1 : 1);
    float4 textureColor2 = texture2.Sample(sampler2, input.texCoord2);

    textureColor1.a *= transparency.x;
    textureColor2.a *= transparency.y;

    float4 r0 = textureColor1;
    r0.rgb *= input.color.rgb;
    r0.rgb += r0.rgb;
    float4 r1 = textureColor2;
    float4 r2;
    r2.x = -r0.a + 1;
    combinedColor.a = input.color.a * r0.a * r1.a;
    r0.rgb = r1.rgb * r2.x + r0.rgb;
    combinedColor.rgb = r0.rgb;

    return commonFinalize(combinedColor, input);
}

float4 main_PS_Combiners_Mod_AddAlpha_Alpha(PixelInput input) : SV_Target
{
    float4 textureColor1 = texture1.Sample(sampler1, input.texCoord1);
	clip((modelPassParams.z && textureColor1.a <= 0.5f) ? -1 : 1);
    float4 textureColor2 = texture2.Sample(sampler2, input.texCoord2);

    textureColor1.a *= transparency.x;
    textureColor2.a *= transparency.y;

    float4 r0 = textureColor1;
    float4 r1;
    r1.x = -r0.w + 1;
    r0 = r0.wxyz * input.color.wxyz * 0.5f;
    r0.yzw = r0.yzw + r0.yzw;
    float4 r2 = textureColor2;
    r1.yzw = r2.www * r2.xyz;
    r0.yzw = r1.yzw * r1.xxx + r0.yzw;
    combinedColor.rgb = r0.yzw;
    r0.y = dot(r2.xyz, float4(0.3f, 0.59f, 0.11f, 0.0f).rgb); // uhhh
    combinedColor.a = r2.w * r0.y + r0.x;

    return commonFinalize(combinedColor, input);
}

float4 main_PS_Combiners_Opaque_Mod2xNA_Alpha(PixelInput input) : SV_Target
{
    float4 textureColor1 = texture1.Sample(sampler1, input.texCoord1);
	clip((modelPassParams.z && textureColor1.a <= 0.5f) ? -1 : 1);
    float4 textureColor2 = texture2.Sample(sampler2, input.texCoord2);

    textureColor1.a *= transparency.x;
    textureColor2.a *= transparency.y;

    float4 r0 = textureColor2;
    float4 r1 = textureColor1;
    r0.rgb *= r1.rgb;
    r1.rgb = -r0.rgb * 2.0f + r1.rgb;
    r0.rgb += r0.rgb;
    r0.rgb = r1.a * r1.rgb + r0.rgb;
    r0.rgb *= input.color.a * 0.5f;
    r0.rgb *= 2.0f;
    combinedColor.rgb = r0.rgb;
    combinedColor.a = input.color.a;

    return commonFinalize(combinedColor, input);
}

float4 main_PS_Combiners_Opaque_ModNA_Alpha(PixelInput input) : SV_Target
{
    float4 textureColor1 = texture1.Sample(sampler1, input.texCoord1);
	clip((modelPassParams.z && textureColor1.a <= 0.5f) ? -1 : 1);
    float4 textureColor2 = texture2.Sample(sampler2, input.texCoord2);

    textureColor1.a *= transparency.x;
    textureColor2.a *= transparency.y;

    float4 r0 = textureColor2;
    float4 r1 = textureColor1;
    float3 r2 = r0.rgb * r1.rgb;
    r0.rgb = -r1.rgb * r0.rgb + r1.rgb;
    r0.rgb = r1.a * r0.rgb + r2.rgb;
    r0.rgb *= input.color.rgb;
    r0.rgb *= 2.0f;
    combinedColor.rgb = r0.rgb;
    combinedColor.a = input.color.a;

    return commonFinalize(combinedColor, input);
}

float4 main_PS_Combiners_Opaque_AddAlpha_Wgt(PixelInput input) : SV_Target
{
    float4 textureColor1 = texture1.Sample(sampler1, input.texCoord1);
	clip((modelPassParams.z && textureColor1.a <= 0.5f) ? -1 : 1);
    float4 textureColor2 = texture2.Sample(sampler2, input.texCoord2);

    textureColor1.a *= transparency.x;
    textureColor2.a *= transparency.y;

    float4 r0 = textureColor1;
    r0.rgb *= input.color.rgb;
    r0.rgb *= r0.rgb;
    float4 r1 = textureColor2;
    r1.rgb *= r1.a;
    r0.rgb = r1.rgb + r0.rgb;
    combinedColor.rgb = r0.rgb;
    combinedColor.a = input.color.a;

    return commonFinalize(combinedColor, input);
}

float4 main_PS_Combiners_Mod_Dual_Crossfade(PixelInput input) : SV_Target
{
    float4 textureColor1 = texture1.Sample(sampler1, input.texCoord1);
	clip((modelPassParams.z && textureColor1.a <= 0.5f) ? -1 : 1);
    float4 textureColor2 = texture2.Sample(sampler2, input.texCoord2);
    float4 textureColor3 = texture3.Sample(sampler2, input.texCoord3);

    float4 r0 = textureColor3;
    float4 r1 = textureColor2;
    float4 r2 = textureColor1;
    r1 -= r2;
    float4 r3;
    r3.xy = transparency.xy;
    r1 = r3.x * r1 + r2;
    r0 -= r1;
    r0 = r3.y * r0 + r1;
    r0.rgb *= input.color.rgb;
    combinedColor.a = r0.a * input.color.a;
    r0.rgb *= 2.0f;
    combinedColor.rgb = r0.rgb;

    return commonFinalize(combinedColor, input);
}

float4 main_PS_Combiners_Mod_Masked_Dual_Crossfade(PixelInput input) : SV_Target
{
    float4 textureColor1 = texture1.Sample(sampler1, input.texCoord1);
	clip((modelPassParams.z && textureColor1.a <= 0.5f) ? -1 : 1);
    float4 textureColor2 = texture2.Sample(sampler2, input.texCoord2);
    float4 textureColor3 = texture3.Sample(sampler3, input.texCoord3);
    float4 textureColor4 = texture4.Sample(sampler4, input.texCoord4);

    float4 r0 = textureColor3;
    float4 r1 = textureColor2;
    float4 r2 = textureColor1;
    r1.wxyz -= r2.wxyz;
    float4 r3;
    r3.xy = transparency.xy;
    r1 = r3.x * r1 + r2.wxyz;
    r0 = r0.wxyz + -r1;
    r0 = r3.y * r0 + r1;
    r0 *= input.color;
    r1 = textureColor4;
    combinedColor.a = r0.x * r1.w;
    r0.rgb *= 2.0f;
    combinedColor.rgb = r0.rgb; 

    return commonFinalize(combinedColor, input);
}

