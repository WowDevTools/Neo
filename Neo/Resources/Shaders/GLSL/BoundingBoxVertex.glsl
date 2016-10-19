#version 330 core

uniform GlobalParameters
{
	mat4 ModelViewProjection;
	vec4 viewport;

	vec4 ambientLight;
	vec4 diffuseLight;

	vec4 fogColuor;
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

// Vertex input
layout(location = 0) in vec3 vertexPosition;
layout(location = 1) in vec2 vertexUV;

out VertexOutput
{
	vec2 UV;
};

void main()
{
	gl_Position = ModelViewProjection * vec4(position.xyz, 1);

	UV = vertexUV;
}