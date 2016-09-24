#version 330 core

in vec2 UV;

uniform sampler2D quadTexture;

out vec4 colour;

void main()
{
	colour = texture(quadTexture, UV);
}