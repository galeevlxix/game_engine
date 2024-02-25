#version 330                                           
layout (location = 0) in vec3 aPosition; 
layout (location = 1) in vec2 aTexCoord;
layout (location = 2) in vec3 aNormal;
 
out vec2 texCoord;
out vec3 Normal0;
out vec3 WorldPos0;

uniform mat4 world;
uniform mat4 campos;
uniform mat4 camrot;
uniform mat4 pers;

void main()                                            
{        
	texCoord = aTexCoord;
	gl_Position = vec4(aPosition, 1.0) * world * campos * camrot * pers;
	Normal0 = normalize((vec4(aNormal, 0.0) * world).xyz);
	WorldPos0 = (vec4(aPosition, 1.0) * world).xyz;
}