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

cbuffer AnimationMatrices : register(b2)
{
    row_major float4x4 Bones[256];
}

cbuffer PerModelPassBuffer : register(b3)
{
    row_major float4x4 uvAnimation;
    float4 modelPassParams;
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
    float4 modelPassParams : TEXCOORD1;
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

    position = mul(position, matView);
    position = mul(position, matProj);

    VertexOutput output = (VertexOutput) 0;
    output.position = position;
    output.normal = normal;
    float4 tcTransform = mul(float4(input.texCoord, 0, 1), uvAnimation);
    output.texCoord = tcTransform.xy / tcTransform.w;
    output.modelPassParams = modelPassParams;

    return output;
}