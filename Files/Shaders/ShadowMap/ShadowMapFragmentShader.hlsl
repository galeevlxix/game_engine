#version 330

in vec2 TexCoord0;
uniform sampler2D gShadowMap;

out vec4 FragColor;

void main()
{
    float Depth = texture2D(gShadowMap, TexCoord0).x;
    Depth = 1.0 - (1.0 - Depth) * 25.0;
    FragColor = vec4(Depth);
}