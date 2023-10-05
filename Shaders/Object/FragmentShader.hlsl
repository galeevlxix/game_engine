#version 330
out vec4 outputColor;

const int MAX_POINT_LIGHTS = 10;
const int MAX_SPOT_LIGHTS = 10;

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
    float Cutoff;
};

uniform sampler2D gDiffuseMap;

uniform DirectionalLight gDirectionalLight;

uniform vec3 gCameraPos;
uniform float gMatSpecularIntensity;
uniform float gMatSpecularPower;

uniform PointLight gPointLights[MAX_POINT_LIGHTS];
uniform int gNumPointLights;

uniform SpotLight gSpotLights[MAX_SPOT_LIGHTS];
uniform int gNumSpotLights;

vec4 CalcLightInternal(BaseLight Light, vec3 LightDirection, vec3 Normal);
vec4 CalcDirectionalLight(vec3 Normal);
vec4 CalcPointLight(PointLight pLight, vec3 Normal);
vec4 CalcSpotLight(SpotLight sLight, vec3 Normal);

void main() 
{ 
    vec4 texel = texture2D(gDiffuseMap, texCoord.xy);

    if (texel.a < 0.3) discard;

    vec4 TotalLight = CalcDirectionalLight(Normal0);

    for (int i = 0; i < gNumPointLights; i++)
    {
        TotalLight += CalcPointLight(gPointLights[i], Normal0);
    }

    for (int i = 0; i < gNumSpotLights; i++)
    {
        TotalLight += CalcSpotLight(gSpotLights[i], Normal0);
    }

	outputColor = texel * TotalLight;
}             

vec4 CalcLightInternal(BaseLight Light, vec3 pLightDirection, vec3 Normal)
{
    vec4 AmbientColor = vec4(Light.Color, 1.0) * Light.AmbientIntensity;

    vec3 LightDirection = normalize(pLightDirection);

    float DiffuseFactor = dot(Normal, -LightDirection);

    vec4 DiffuseColor = vec4(0, 0, 0, 0);
    vec4 SpecularColor = vec4(0, 0, 0, 0);

    if (DiffuseFactor > 0)
    {
        DiffuseColor = vec4(Light.Color, 1.0) * Light.DiffuseIntensity * DiffuseFactor;

        vec3 VertexToEye = normalize(gCameraPos - WorldPos0);
        vec3 LightReflect = normalize(reflect(LightDirection, Normal));
        float SpecularFactor = dot(VertexToEye, LightReflect);
        SpecularFactor = pow(SpecularFactor, gMatSpecularPower);

        if (SpecularFactor > 0) 
        {
            SpecularColor = vec4(Light.Color, 1.0f) * gMatSpecularIntensity * SpecularFactor;
        }
    }
    return (AmbientColor + DiffuseColor + SpecularColor);
}

vec4 CalcDirectionalLight(vec3 Normal)
{
    return CalcLightInternal(gDirectionalLight.Base, gDirectionalLight.Direction, Normal);
}

vec4 CalcPointLight(PointLight pLight, vec3 Normal)
{
    vec3 LightDirection = normalize(WorldPos0 - pLight.Position);
    float Distance = length(LightDirection);

    vec4 Color = CalcLightInternal(pLight.Base, LightDirection, Normal);
    float Attenuation =  pLight.Atten.Constant + 
                         pLight.Atten.Linear * Distance +
                         pLight.Atten.Exp * Distance * Distance;
    return Color / Attenuation;
}

vec4 CalcSpotLight(SpotLight sLight, vec3 Normal) 
{
    vec3 LightToPixel = normalize(WorldPos0 - sLight.Base.Position);
    float SpotFactor = dot(LightToPixel, sLight.Direction);

    if (SpotFactor > sLight.Cutoff)
    {
        vec4 Color = CalcPointLight(sLight.Base, Normal);
        return Color * (1.0 - (1.0 - SpotFactor) * 1.0 / (1.0 - sLight.Cutoff));
    }

    return vec4(0, 0, 0, 0);
}