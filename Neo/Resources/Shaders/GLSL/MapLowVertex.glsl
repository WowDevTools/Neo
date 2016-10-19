#version 330 core

uniform GlobalParameters
{
	mat4 ModelViewProjection;
	vec4 viewport;

	vec4 ambientLight;
	vec4 diffuseLight;

	vec4 fogColour;
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

layout(location = 0) in vec3 vertexPosition;

out VertexData
{
	float depth;
};

void main()
{
	gl_Position = ModelViewProjection * vec4(vertexPosition.xyz, 1);
	depth = gl_Position.z / gl_Position.w;
}