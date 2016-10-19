#version 330 core

// Input interface
in VertexOutput
{
	vec2 UV;
};

// Output variables
out vec4 color;

void main()
{
	color = vec4(1, 1, 1, 1);

	if ((UV.x < 0.02 || UV.x > 0.98) &&
		(UV.y < 0.02 || UV.y > 0.98))
	{
		color = vec4(1, 1, 1, 0.7);
	}
	else if ((UV.z < 0.02 || UV.z > 0.98) &&
			 (UV.x < 0.02 || UV.x > 0.98 || UV.y < 0.02 || UV.y > 0.98))
	{
		color = vec4(1, 1, 1, 0.7);
	}
	else
	{
		discard;
	}
}