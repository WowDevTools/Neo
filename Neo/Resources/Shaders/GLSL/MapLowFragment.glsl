#version 330 core

in float depth;

out vec4 color;

uniform vec4 fogParams;
uniform vec4 fogColour;

void main()
{
	// Discard the fragment if it's beyond the fog end
	if (depth - (fogParams.y / fogParms.z) < 0)
	{
		discard;
	}
	else
	{
		color = vec4(fogColour.xyz, 1);
	}
}