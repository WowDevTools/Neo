// TexturedQuadVertex.hlsl

struct VertexInput
{
    float2 position : POSITION0;
    float2 texCoord : TEXCOORD0;
};

struct VertexOutput
{
    float4 position : SV_Position;
    float2 texCoord : TEXCOORD0;
};

VertexOutput main(VertexInput input)
{
    VertexOutput output = (VertexOutput) 0;
    output.position = float4(input.position, 0, 1);
    output.texCoord = input.texCoord;

    return output;
}
