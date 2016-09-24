#version 330 core

layout(location = 0) in vec3 vertexPosition;
layout(location = 1) in vec2 vertexUV;

uniform mat4 ModelViewProjection;

out vec2 UV;

void main()
{
	gl_Position = ModelViewProjection * vec4(position.xyz, 1);

	UV = vertexUV;
}