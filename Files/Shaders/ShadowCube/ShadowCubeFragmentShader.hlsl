#version 330 core

in vec4 WorldPos0;

uniform vec3 gLightPos;

out float outputColor;

void main()
{
    outputColor = length(WorldPos0.xyz - gLightPos);
}