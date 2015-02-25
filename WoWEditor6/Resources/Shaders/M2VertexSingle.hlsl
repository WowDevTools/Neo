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

cbuffer AnimationMatrices : register(b1)
{
    row_major float4x4 Bones[256];
}

cbuffer PerDrawCallBuffer : register(b2)
{
    row_major float4x4 matInstance;
    float4 colorMod;
}

cbuffer PerModelPassBuffer : register(b3)
{
    row_major float4x4 uvAnimation1;
	row_major float4x4 uvAnimation2;
	row_major float4x4 uvAnimation3;
	row_major float4x4 uvAnimation4;
    float4 modelPassParams;
    float4 animatedColor;
}

struct VertexInput
{
    float3 position : POSITION0;
    float4 boneWeights : BLENDWEIGHT0;
    int4 bones : BLENDINDEX0;
    float3 normal : NORMAL0;
    float2 texCoord1 : TEXCOORD0;
    float2 texCoord2 : TEXCOORD1;
};

struct VertexOutput
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
    float4 modelPassParams : TEXCOORD6;
};

VertexOutput fillCommonOutput( VertexInput input )
{
	float4 basePosition = float4(input.position, 1.0);
    float4 position = mul(basePosition, Bones[input.bones.x]) * input.boneWeights.x;
    position += mul(basePosition, Bones[input.bones.y]) * input.boneWeights.y;
    position += mul(basePosition, Bones[input.bones.z]) * input.boneWeights.z;
    position += mul(basePosition, Bones[input.bones.w]) * input.boneWeights.w;

    float3 normal = float3(0, 0, 0);
    normal += mul(input.normal, (float3x3)Bones[input.bones.x]) * input.boneWeights.x;
    normal += mul(input.normal, (float3x3)Bones[input.bones.y]) * input.boneWeights.y;
    normal += mul(input.normal, (float3x3)Bones[input.bones.z]) * input.boneWeights.z;
    normal += mul(input.normal, (float3x3)Bones[input.bones.w]) * input.boneWeights.w;

    position = mul(position, matInstance);
    normal = mul(normal, (float3x3)matInstance);

    float3 worldPos = position;
    position = mul(position, matView);
    position = mul(position, matProj);

	VertexOutput output = (VertexOutput) 0;
    output.position = position;
    output.depth = distance(worldPos, eyePosition);
    output.normal = normal;
    output.worldPosition = worldPos;

    output.color = float4( colorMod.rgb * animatedColor.rgb * 0.5f, colorMod.a * animatedColor.a );
    output.modelPassParams = modelPassParams;

	return output;
}

VertexOutput main_VS_Diffuse_T1( VertexInput input )
{
	VertexOutput output = fillCommonOutput( input );
	output.texCoord1 = input.texCoord1;

	float4 texCoord1Alt = mul( float4( input.texCoord1, 0, 1), uvAnimation1 );
	output.texCoord1 = texCoord1Alt.xy / texCoord1Alt.w;
	
	return output;
}