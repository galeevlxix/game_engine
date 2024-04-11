#version 330
layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec2 aTexCoord;
layout (location = 2) in vec3 aNormal;
 
uniform mat4 wvp;
 
out vec2 TexCoord0;
 
void main()
{
    gl_Position = vec4(aPosition, 1.0) * wvp;
    TexCoord0 = aTexCoord;
}