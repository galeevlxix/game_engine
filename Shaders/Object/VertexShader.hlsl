#version 330                                           
layout (location = 0) in vec3 aPosition; 
layout (location = 1) in vec2 aTexCoord;
layout (location = 2) in vec3 aNormal;
 
out vec2 texCoord;
out vec3 vNormal;

uniform mat4 mvp;
uniform mat4 campos;
uniform mat4 camrot;
uniform mat4 pers;

void main()                                            
{        
	texCoord = aTexCoord;
	gl_Position = vec4(aPosition, 1.0) * mvp * campos * camrot * pers;
	vNormal = (vec4(aNormal, 0.0) * mvp).xyz;
}