#version 330 core

layout(position = 0) in vec3 vertexPosition;
layout(position = 1) in vec3 vertexNormal;
layout(position = 2) in vec2 vertexUV;
layout(position = 3) in vec2 vertexUVAlpha;
layout(position = 4) in vec4 vertexColour;
layout(position = 5) in vec4 vertexAddColour;

uniform GlobalParameters
{
	mat4 ModelViewProjection;
	vec4 viewport;

	vec4 ambientLight;
	vec4 diffuseLight;

	vec4 fogColor;
	// x -> fogStart
	// y -> fogEnd
	// z -> farClip
	vec4 fogParams;

	vec4 mousePosition;
	vec4 eyePosition;

	// x -> innerRadius
	// y -> outerRadius
	// z -> brushTime
	vec4 brushParams;
};

uniform TexAnimBuffer
{
	mat4 textureAnimation0;
	mat4 textureAnimation1;
	mat4 textureAnimation2;
	mat4 textureAnimation3;
};

out vec3 Normal;
out vec2 textureCoordinate0;
out vec2 textureCoordinate1;
out vec2 textureCoordinate2;
out vec2 textureCoordinate3;
out vec2 textureCoordinateAlpha;
out vec4 vertexShadingColour;
out vec4 vertexShadingAddColour;
out float depth;
out vec3 worldPosition;


void main()
{
	vec4 position = ModelViewProjection * vec4(vertexPosition.xyz, 1);

	gl_Position = position;
	Normal = vertexNormal;

	vec4 texCoord0 = vec4(vertexUV, 0, 1) * textureAnimation0;
	vec4 texCoord1 = vec4(vertexUV, 0, 1) * textureAnimation1;
	vec4 texCoord2 = vec4(vertexUV, 0, 1) * textureAnimation2;
	vec4 texCoord3 = vec4(vertexUV, 0, 1) * textureAnimation3;

	textureCoordinate0 = texCoord0.xy / texCoord0.w;
	textureCoordinate1 = texCoord1.xy / texCoord1.w;
	textureCoordinate2 = texCoord2.xy / texCoord2.w;
	textureCoordinate3 = texCoord3.xy / texCoord3.w;

	textureCoordinateAlpha = vertexUVAlpha;
	vertexShadingColour = vertexColour;
	vertexShadingAddColour = vertexAddColour;

	depth = length(vertexPosition - eyePosition);
	worldPosition = position.xyz;
}