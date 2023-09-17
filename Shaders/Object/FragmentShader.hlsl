#version 330
out vec4 outputColor;

in vec2 texCoord;
in vec3 Normal0;
in vec3 WorldPos0;

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

uniform BaseLight gBaseLight;
uniform DirectionalLight gDirectionalLight;

uniform vec3 gCameraPos;
uniform float gMatSpecularIntensity;
uniform float gMatSpecularPower;

void main() 
{ 
    vec4 texel = texture(gDiffuseMap, texCoord);

    if (texel.a < 0.3) discard;

    vec4 AmbientColor = vec4(gBaseLight.Color, 1.0) * gBaseLight.AmbientIntensity;

    float DiffuseFactor = dot(Normal0, -gDirectionalLight.Direction);

    vec4 DiffuseColor = vec4(0, 0, 0, 0);
    vec4 SpecularColor = vec4(0, 0, 0, 0);

    if (DiffuseFactor > 0)
    {
        DiffuseColor = 
            vec4(gDirectionalLight.Base.Color, 1.0) * 
            gDirectionalLight.Base.DiffuseIntensity *
            DiffuseFactor;

        vec3 VertexToEye = normalize(gCameraPos - WorldPos0);
        vec3 LightReflect = normalize(reflect(gDirectionalLight.Direction, Normal0));
        float SpecularFactor = dot(VertexToEye, LightReflect);
        SpecularFactor = pow(SpecularFactor, gMatSpecularPower);

        if (SpecularFactor > 0) {
            SpecularColor = 
                vec4(gDirectionalLight.Base.Color, 1.0f) * 
                gMatSpecularIntensity *
                SpecularFactor;
        }
    }

	outputColor = texel * (AmbientColor + DiffuseColor + SpecularColor);
}             