#version 330 core

layout(position = 0) in vec3 vertexPosition;
layout(position = 1) in vec2 vertexUV;

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

uniform PerDrawCallBuffer
{
	mat4 transformMatrix;
};

out vec2 UV;

void main()
{
	vec4 position = vec4(vertexPositon, 1);
	position *= transformMatrix;
	position.x = (position.x * 2.0f) / viewport.x - 1.0f;
	position.y = 1.0f - (position.y * 2.0f) / viewport.y;

	gl_Position = position;
	UV = vertexUV;
}