#version 330
out vec4 outputColor;

uniform vec3 color;
uniform vec3 light;

void main() 
{ 
	outputColor = vec4(color * light, 1.0);
}