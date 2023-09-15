#version 330
out vec4 outputColor;

in vec2 texCoord;
in vec3 Normal0;
in vec3 Tangent0;

struct BaseLight
{
    vec3 Color;
    float AmbientIntensity;
    float DiffuseIntensity;
};

struct DirectionalLight
{
    vec3 Direction;
    BaseLight Base;
};

struct Attenuation 
{
    float Constant;
    float Linear;
    float Exp;
};

struct PointLight
{
    BaseLight Base;
    vec3 Position;
    Attenuation Atten;
};

struct SpotLight
{
    PointLight Base;
    vec3 Direction;
    float Cuttoff;
};

uniform sampler2D gDiffuseMap;
uniform sampler2D gNormalMap;

uniform BaseLight gBaseLight;
uniform DirectionalLight gDirectionalLight;

void main() 
{ 
    vec4 texel = texture(gDiffuseMap, texCoord);

    if (texel.a < 0.3) discard;

	outputColor = texel * vec4(gBaseLight.Color, 1.0) * gBaseLight.AmbientIntensity;
}             