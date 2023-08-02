#version 330
out vec4 outputColor;

in vec2 texCoord;

uniform sampler2D texture0;

void main() 
{ 
	vec4 tex = texture(texture0, texCoord);

	vec3 light = vec3(1, 1, 1);

	outputColor = vec4(tex.rgb * light, tex.a);
}      