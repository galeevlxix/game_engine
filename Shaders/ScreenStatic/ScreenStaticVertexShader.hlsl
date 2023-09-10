#version 330                                           
layout (location = 0) in vec3 aPosition; 
layout (location = 1) in vec2 aTexCoord;
 
out vec2 texCoord;

uniform mat4 mvp;
uniform mat4 pers;

void main()                                            
{        
	texCoord = aTexCoord;
	gl_Position = vec4(aPosition, 1.0) * mvp * pers;
}