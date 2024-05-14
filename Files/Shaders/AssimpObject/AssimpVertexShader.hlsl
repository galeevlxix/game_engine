#version 330 core

layout (location = 0) in vec3 aPosition; 
layout (location = 1) in vec2 aTexCoord;
layout (location = 2) in vec3 aNormal;
layout (location = 3) in vec3 aTangent;
 
out vec2 texCoord;
out vec3 Normal0;
out vec3 WorldPos0;
out vec3 Tangent0;
out vec4 LightPos0;
out vec4 Position0;

uniform mat4 world;
uniform mat4 wvp;

void main()                                            
{        
	texCoord = aTexCoord;
    Position0 = vec4(aPosition, 1.0);
    gl_Position = vec4(aPosition, 1.0) * wvp;
    Normal0 = normalize((vec4(aNormal, 0.0) * world).xyz);
    Tangent0 = normalize((vec4(aTangent, 0.0) * world).xyz);
	WorldPos0 = (vec4(aPosition, 1.0) * world).xyz;
}