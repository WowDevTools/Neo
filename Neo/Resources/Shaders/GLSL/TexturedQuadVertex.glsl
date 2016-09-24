#version 330 core

layout(position = 0) in vec2 vertexPosition;
layout(position = 1) in vec2 vertexUV;

out vec2 UV;

void main()
{
	gl_Position = vec4(vertexPosition, 0, 1);
	UV = vertexUV;
}