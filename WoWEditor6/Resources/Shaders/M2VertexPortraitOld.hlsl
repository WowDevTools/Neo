cbuffer MatrixBuffer : register(b0)
{
    row_major float4x4 matViewProj;
};

cbuffer AnimationMatrices : register(b1)
{
    row_major float4x4 Bones[256];
}

cbuffer PerModelPassBuffer : register(b2)
{
	row_major float4x4 uvAnimation;
	row_major float4x4 uvAnimation2;
	row_major float4x4 uvAnimation3;
	row_major float4x4 uvAnimation4;
	float4 modelPassParams; // x = unlit, y = unfogged, z = alphakey
	float4 animatedColor;
}

struct VertexInput
{
    float3 position : POSITION0;
    float4 boneWeights : BLENDWEIGHT0;
    int4 bones : BLENDINDEX0;
    float3 normal : NORMAL0;
    float2 texCoord : TEXCOORD0;
    float2 texCoord2 : TEXCOORD1;
};

struct VertexOutput
{
    float4 position : SV_Position;
    float3 normal : NORMAL0;
    float2 texCoord : TEXCOORD0;
    float2 texCoord1 : TEXCOORD1;
    float4 modelPassParams : TEXCOORD2;
};

VertexOutput main(VertexInput input) {

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

    position = mul(position, matViewProj);

    VertexOutput output = (VertexOutput) 0;
    output.position = position;
    output.normal = normal;
    float4 tcTransform = mul(float4(input.texCoord, 0, 1), uvAnimation);
    output.texCoord = tcTransform.xy / tcTransform.w;
    output.texCoord1 = input.texCoord2;
    output.modelPassParams = modelPassParams;

    return output;
}
