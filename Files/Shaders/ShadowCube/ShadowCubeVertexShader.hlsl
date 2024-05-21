#version 330 core

layout (location = 0) in vec3 aPosition;

uniform mat4 wvp;
uniform mat4 world;

out vec4 WorldPos0;
                  
void main()
{
    gl_Position = vec4(aPosition, 1.0) * wvp;
    WorldPos0 = vec4(aPosition, 1.0) * world;
}