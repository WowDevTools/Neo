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

in VertexData
{
	float depth;
};

out vec4 finalColour;

void main()
{
	// Discard the fragment if it's beyond the fog end
	if (depth - (fogParams.y / fogParms.z) < 0)
	{
		discard;
	}
	else
	{
		finalColour = vec4(fogColour.xyz, 1);
	}
}