#version 330 core

out uvec4 FragColor;

uniform int gDrawIndex;

uniform int gObjectIndex;

void main()
{
    FragColor = uvec4(gObjectIndex, gDrawIndex, gl_PrimitiveID + 1, 1.0f);
}