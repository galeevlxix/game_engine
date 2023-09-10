#version 330                                           
layout (location = 0) in vec3 aPosition; 
layout (location = 1) in vec2 aTexCoord;
layout (location = 2) in vec3 aNormal;
layout (location = 3) in vec3 aTangent;
 
out vec2 texCoord;
out vec3 Normal0;
out vec3 Tangent0;

uniform mat4 mvp;
uniform mat4 campos;
uniform mat4 camrot;
uniform mat4 pers;

void main()                                            
{        
	texCoord = aTexCoord;
	gl_Position = vec4(aPosition, 1.0) * mvp * campos * camrot * pers;
	Normal0 = (vec4(aNormal, 0.0) * mvp).xyz;
	Tangent0 = (vec4(aTangent, 0.0) * mvp).xyz;
}