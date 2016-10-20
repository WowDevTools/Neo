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

// INVESTIGATE: It's highly likely that this matrix is not required anymore
uniform InstanceParameters
{
	mat4 instanceMatrix;
};

layout(location = 0) in vec3 vertexPosition;
layout(location = 1) in vec3 vertexNormal;
layout(location = 2) in vec2 vertexUV;
layout(location = 3) in vec4 vertexColour;

out FragmentData
{
	vec4 fragmentPosition;
	vec3 fragmentNormal;
	vec2 fragmentUV;
	float fragmentDepth;
	vec4 fragmentColour;
	vec3 fragmentWorldPosition;
};

void main()
{
	gl_Position = vec4(vertexPosition, 1.0f) * instanceMatrix * ModelViewProjection;

	fragmentPosition = gl_Position;
	fragmentNormal = normalize(vertexNormal * mat3(instanceMatrix));
	fragmentUV = vertexUV;
	fragmentDepth = length(gl_Position.xyz - eyePosition.xyz);
	fragmentColour = vertexColour;
	fragmentWorldPosition = gl_Position.xyz;

}