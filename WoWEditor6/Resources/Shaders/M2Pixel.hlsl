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
    float4 modelPassParams;
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
    nointerpolation float4 modelPassParams : TEXCOORD6; // x = unlit, y = unfogged, z = alphakey
};

float3 getDiffuseLight(float3 normal)
{
    float light = saturate( dot( normal, normalize( -float3( -1, 1, -1 ) ) ) );
    float3 diffuse = diffuseLight.rgb * light;
    diffuse += ambientLight.rgb;
    diffuse = saturate(diffuse);
    return diffuse;
}

static float4 combinedColor = float4( 0.0f, 0.0f, 0.0f, 0.0f );
static float4 finalColor = float4( 0.0f, 0.0f, 0.0f, 0.0f );

float3 applyFog( float3 textureColor, PixelInput input )
{
	float fogDepth = input.depth - fogParams.x;
	fogDepth /= ( fogParams.y - fogParams.x );
	float fog = pow( saturate( fogDepth ), 1.5f ) * input.modelPassParams.y;
	return ( fog * fogColor.rgb + ( 1.0f - fog ) * textureColor.rgb );
}

float4 commonFinalize( float4 finalColor, PixelInput input )
{
	if( input.modelPassParams.x ) /* not unlit */
	{
		finalColor.rgb *= getDiffuseLight( input.normal );
	}

	finalColor.rgb = applyFog( finalColor.rgb, input );
	
	return finalColor;
}

float4 main_PS_Combiners_Opaque( PixelInput input ) : SV_Target
{
	float4 textureColor1 = texture1.Sample( sampler1, input.texCoord1 );

	float4 r0 = textureColor1;
	r0.rgb *= input.color.rgb;
	r0.rgb *= 2.0f;

	combinedColor.rgb = r0.rgb;
	combinedColor.a = input.color.a;

	return commonFinalize( combinedColor, input );
}

float4 main_PS_Combiners_Mod( PixelInput input ) : SV_Target
{
	float4 textureColor1 = texture1.Sample( sampler1, input.texCoord1 );
	clip( ( textureColor1.a <= 0.5f ) ? -1 : 1  );

	float4 r0 = textureColor1;
	r0.rgb *= input.color.rgb;
	combinedColor.a = r0.a * input.color.a;
	r0.rgb *= 2.0f;
	combinedColor.rgb = r0.rgb;

	return commonFinalize( combinedColor, input );
}

