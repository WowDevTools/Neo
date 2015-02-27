// M2VertexInstanced.hlsl

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

cbuffer PerModelPassBuffer : register(b2)
{
    row_major float4x4 uvAnimation1;
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
    float2 texCoord1 : TEXCOORD0;
    float2 texCoord2 : TEXCOORD1;

    float4 mat0 : TEXCOORD2;
    float4 mat1 : TEXCOORD3;
    float4 mat2 : TEXCOORD4;
    float4 mat3 : TEXCOORD5;
    float4 colorMod : COLOR0;
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
    float4 colorMod : COLOR1;
};

static float4x4 worldViewMat = (float4x4)0;

VertexOutput fillCommonOutput(VertexInput input)
{
    float4 position = float4(input.position, 1.0f);

    float4x4 bonematrix = input.boneWeights.x * Bones[input.bones.x];
    bonematrix += input.boneWeights.y * Bones[input.bones.y];
    bonematrix += input.boneWeights.z * Bones[input.bones.z];
    bonematrix += input.boneWeights.w * Bones[input.bones.w];

    // x World matrix from instance data
    float4x4 worldMatrixInstance = mul(bonematrix, float4x4(
        input.mat0, input.mat1, input.mat2, input.mat3));

    // Transform via combined bones + world matrix
    position = mul(position, worldMatrixInstance);
    
    // static for env (pass in bool to skip?)
    worldViewMat = mul(worldMatrixInstance, matView);

    float3 worldPos = position.xyz;
    position = mul(position, matView);
    position = mul(position, matProj);

    VertexOutput output = (VertexOutput) 0;
    output.position = position;
    output.depth = distance(worldPos, eyePosition);
    output.normal = mul(input.normal, worldMatrixInstance);
    output.worldPosition = worldPos;
    output.color = float4(animatedColor.rgb * 0.5f, animatedColor.a);
    output.colorMod = input.colorMod;
    return output;
}

float2 getEnv(VertexInput input)
{
    float3 u = normalize(mul(input.position, worldViewMat));
    float3 n = normalize(mul(input.normal, worldViewMat));
    float3 r = reflect(u, n);
    float m = 2.0 * sqrt(r.x*r.x + r.y*r.y + (r.z+1.0)*(r.z+1.0));

    float2 env;
    env.x = r.x / m + 0.5f;
    env.y = r.y / m + 0.5f;

    return env;
}

VertexOutput getEdgeFade( VertexOutput vo )
{
	float d = dot( -normalize( vo.position ), normalize( vo.normal ) );
	d = clamp( ( d * d ) * 2.7f - 0.4f, 0.0, 1.0f );
	vo.color.a *= d;
	return vo;
}

VertexOutput main_VS_Diffuse_T1(VertexInput input)
{
    VertexOutput output = fillCommonOutput(input);
    
    float4 texCoord1Alt = mul(float4(input.texCoord1, 0, 1), uvAnimation1);
    output.texCoord1 = texCoord1Alt.xy / texCoord1Alt.w;
    
    return output;
}

VertexOutput main_VS_Diffuse_EdgeFade_T1(VertexInput input)
{
    VertexOutput output = fillCommonOutput(input);
    
	output = getEdgeFade( output );

    float4 texCoord1Alt = mul(float4(input.texCoord1, 0, 1), uvAnimation1);
    output.texCoord1 = texCoord1Alt.xy / texCoord1Alt.w;
    
    return output;
}

VertexOutput main_VS_Diffuse_T1_T2(VertexInput input)
{
    VertexOutput output = fillCommonOutput(input);
    
    float4 texCoord1Alt = mul(float4(input.texCoord1, 0, 1), uvAnimation1);
    output.texCoord1 = texCoord1Alt.xy / texCoord1Alt.w;

    float4 texCoord2Alt = mul(float4(input.texCoord2, 0, 1), uvAnimation2);
    output.texCoord2 = texCoord2Alt.xy / texCoord2Alt.w;
    
    return output;
}

VertexOutput main_VS_Diffuse_EdgeFade_T1_T2(VertexInput input)
{
    VertexOutput output = fillCommonOutput(input);
    
	output = getEdgeFade( output );

    float4 texCoord1Alt = mul(float4(input.texCoord1, 0, 1), uvAnimation1);
    output.texCoord1 = texCoord1Alt.xy / texCoord1Alt.w;

    float4 texCoord2Alt = mul(float4(input.texCoord2, 0, 1), uvAnimation2);
    output.texCoord2 = texCoord2Alt.xy / texCoord2Alt.w;
    
    return output;
}

VertexOutput main_VS_Diffuse_T1_Env(VertexInput input)
{
    VertexOutput output = fillCommonOutput(input);
    
    float4 texCoord1Alt = mul(float4(input.texCoord1, 0, 1), uvAnimation1);
    output.texCoord1 = texCoord1Alt.xy / texCoord1Alt.w;

    float4 texCoord2Alt = mul(float4(getEnv(input), 0, 1), uvAnimation2);
    output.texCoord2 = texCoord2Alt.xy / texCoord2Alt.w;
    
    return output;
}

VertexOutput main_VS_Diffuse_T1_Env_T1(VertexInput input)
{
    VertexOutput output = fillCommonOutput(input);
    
    float4 texCoord1Alt = mul(float4(input.texCoord1, 0, 1), uvAnimation1);
    output.texCoord1 = texCoord1Alt.xy / texCoord1Alt.w;

    float4 texCoord2Alt = mul(float4(getEnv(input), 0, 1), uvAnimation2);
    output.texCoord2 = texCoord2Alt.xy / texCoord2Alt.w;

    float4 texCoord3Alt = mul(float4(input.texCoord1, 0, 1), uvAnimation3);
    output.texCoord3 = texCoord3Alt.xy / texCoord3Alt.w;
    
    return output;
}

VertexOutput main_VS_Diffuse_T1_T1(VertexInput input)
{
    VertexOutput output = fillCommonOutput(input);
    
    float4 texCoord1Alt = mul(float4(input.texCoord1, 0, 1), uvAnimation1);
    output.texCoord1 = texCoord1Alt.xy / texCoord1Alt.w;

    float4 texCoord2Alt = mul(float4(input.texCoord1, 0, 1), uvAnimation2);
    output.texCoord2 = texCoord2Alt.xy / texCoord2Alt.w;

    return output;
}

VertexOutput main_VS_Diffuse_T1_Env_T2(VertexInput input)
{
    VertexOutput output = fillCommonOutput(input);
    
    float4 texCoord1Alt = mul(float4(input.texCoord1, 0, 1), uvAnimation1);
    output.texCoord1 = texCoord1Alt.xy / texCoord1Alt.w;

    float4 texCoord2Alt = mul(float4(getEnv(input), 0, 1), uvAnimation2);
    output.texCoord2 = texCoord2Alt.xy / texCoord2Alt.w;

    float4 texCoord3Alt = mul(float4(input.texCoord2, 0, 1), uvAnimation3);
    output.texCoord3 = texCoord3Alt.xy / texCoord3Alt.w;
    
    return output;
}

VertexOutput main_VS_Diffuse_Env(VertexInput input)
{
    VertexOutput output = fillCommonOutput(input);
    
    float4 texCoord1Alt = mul(float4(getEnv(input), 0, 1), uvAnimation1);
    output.texCoord1 = texCoord1Alt.xy / texCoord1Alt.w;
    
    return output;
}

VertexOutput main_VS_Diffuse_EdgeFade_Env(VertexInput input)
{
    VertexOutput output = fillCommonOutput(input);
    
	output = getEdgeFade( output );

    float4 texCoord1Alt = mul(float4(getEnv(input), 0, 1), uvAnimation1);
    output.texCoord1 = texCoord1Alt.xy / texCoord1Alt.w;
    
    return output;
}

VertexOutput main_VS_Diffuse_Env_T1(VertexInput input)
{
    VertexOutput output = fillCommonOutput(input);
    
    float4 texCoord1Alt = mul(float4(getEnv(input), 0, 1), uvAnimation1);
    output.texCoord1 = texCoord1Alt.xy / texCoord1Alt.w;

    float4 texCoord2Alt = mul(float4(input.texCoord1, 0, 1), uvAnimation2);
    output.texCoord2 = texCoord2Alt.xy / texCoord2Alt.w;
    
    return output;
}

VertexOutput main_VS_Diffuse_Env_Env(VertexInput input)
{
    VertexOutput output = fillCommonOutput(input);
    
    float4 texCoord1Alt = mul(float4(getEnv(input), 0, 1), uvAnimation1);
    output.texCoord1 = texCoord1Alt.xy / texCoord1Alt.w;

    float4 texCoord2Alt = mul(float4(getEnv(input), 0, 1), uvAnimation2);
    output.texCoord2 = texCoord2Alt.xy / texCoord2Alt.w;
    
    return output;
}

VertexOutput main_VS_Diffuse_T2(VertexInput input)
{
    VertexOutput output = fillCommonOutput(input);
    
    float4 texCoord1Alt = mul(float4(input.texCoord2, 0, 1), uvAnimation1);
    output.texCoord1 = texCoord1Alt.xy / texCoord1Alt.w;
    
    return output;
}

VertexOutput main_VS_Diffuse_T1_T1_T1(VertexInput input)
{
    VertexOutput output = fillCommonOutput(input);
    
    float4 texCoord1Alt = mul(float4(input.texCoord1, 0, 1), uvAnimation1);
    output.texCoord1 = texCoord1Alt.xy / texCoord1Alt.w;

    float4 texCoord2Alt = mul(float4(input.texCoord1, 0, 1), uvAnimation2);
    output.texCoord2 = texCoord2Alt.xy / texCoord2Alt.w;

    float4 texCoord3Alt = mul(float4(input.texCoord1, 0, 1), uvAnimation3);
    output.texCoord3 = texCoord3Alt.xy / texCoord3Alt.w;
    
    return output;
}

VertexOutput main_VS_Diffuse_T1_T2_T1(VertexInput input)
{
    VertexOutput output = fillCommonOutput(input);
    
    float4 texCoord1Alt = mul(float4(input.texCoord1, 0, 1), uvAnimation1);
    output.texCoord1 = texCoord1Alt.xy / texCoord1Alt.w;

    float4 texCoord2Alt = mul(float4(input.texCoord2, 0, 1), uvAnimation2);
    output.texCoord2 = texCoord2Alt.xy / texCoord2Alt.w;

    float4 texCoord3Alt = mul(float4(input.texCoord1, 0, 1), uvAnimation3);
    output.texCoord3 = texCoord3Alt.xy / texCoord3Alt.w;
    
    return output;
}

VertexOutput main_VS_Diffuse_T1_T1_T1_T2(VertexInput input)
{
    VertexOutput output = fillCommonOutput(input);
    
    float4 texCoord1Alt = mul(float4(input.texCoord1, 0, 1), uvAnimation1);
    output.texCoord1 = texCoord1Alt.xy / texCoord1Alt.w;

    float4 texCoord2Alt = mul(float4(input.texCoord1, 0, 1), uvAnimation2);
    output.texCoord2 = texCoord2Alt.xy / texCoord2Alt.w;

    float4 texCoord3Alt = mul(float4(input.texCoord1, 0, 1), uvAnimation3);
    output.texCoord3 = texCoord3Alt.xy / texCoord3Alt.w;

    float4 texCoord4Alt = mul(float4(input.texCoord2, 0, 1), uvAnimation4);
    output.texCoord4 = texCoord4Alt.xy / texCoord4Alt.w;
    
    return output;
}
