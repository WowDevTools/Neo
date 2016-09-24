#version 330 core

in vec2 UV;

uniform sampler2D textTexture;

out vec4 colour;

void main()
{
	colour = texture(textTexture, UV);
}