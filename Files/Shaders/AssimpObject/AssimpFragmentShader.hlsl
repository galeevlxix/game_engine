#version 330 core
out vec4 outputColor;

in vec2 texCoord;
in vec3 Normal0;
in vec3 WorldPos0;
in vec3 Tangent0;
in vec4 LightPos0;

struct Material
{
    sampler2D DiffuseMap;
    sampler2D NormalMap;
    sampler2D SpecularMap;
    
    float SpecularPower;
};

struct BaseLight
{
    vec3 Color;
    float Intensity;
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
    float Cutoff1;
    float Cutoff2;
};

const int MAX_POINT_LIGHTS = 10;
const int MAX_SPOT_LIGHTS = 10;

uniform BaseLight gBaseLight;
uniform DirectionalLight gDirectionalLight;

uniform PointLight gPointLights[MAX_POINT_LIGHTS];
uniform int gNumPointLights;

uniform SpotLight gSpotLights[MAX_SPOT_LIGHTS];
uniform int gNumSpotLights;

uniform vec3 gCameraPos;

uniform Material gMaterial;

uniform sampler2D gShadowMap;

vec4 CalcLightInternal(BaseLight Light, vec3 LightDirection, vec3 Normal);
vec4 CalcDirectionalLight(vec3 Normal);
vec4 CalcPointLight(PointLight pLight, vec3 Normal);
vec4 CalcSpotLight(SpotLight sLight, vec3 Normal);
vec3 CalcBumpedNormal();
float CalcShadowFactor(vec4 LightSpacePos);
 
void main()
{
    vec3 Normal = CalcBumpedNormal();
    Normal = normalize(Normal0);
    float shadowFactor = 0;
    
    vec4 texel = texture2D(gMaterial.DiffuseMap, texCoord.xy);

    if (texel.a < 0.3)
        discard;
    
    vec4 AmbientColor = vec4(gBaseLight.Color, 1.0) * gBaseLight.Intensity;

    vec4 TotalLight = CalcDirectionalLight(Normal) + AmbientColor;

    for (int i = 0; i < gNumPointLights; i++)
    {
        TotalLight += CalcPointLight(gPointLights[i], Normal);
    }
    
    if (gSpotLights[0].Base.Base.Intensity > 0)
    {
        shadowFactor = CalcShadowFactor(LightPos0);
    }
    
    if (shadowFactor != 0)
    {
        TotalLight += CalcSpotLight(gSpotLights[0], Normal);
    }
    TotalLight += CalcSpotLight(gSpotLights[1], Normal);
    
    outputColor = texel * TotalLight;
}

float CalcShadowFactor(vec4 LightSpacePos)
{
    // perform perspective divide
    vec3 ProjCoords = LightSpacePos.xyz / LightSpacePos.w;
    // transform to [0,1] range
    ProjCoords = 0.5 * ProjCoords + 0.5;
    // get closest depth value from light's perspective (using [0,1] range fragPosLight as coords)
    float Depth = texture2D(gShadowMap, ProjCoords.xy).r;
    // get depth of current fragment from light's perspective
    // check whether current frag pos is in shadow
    //float bias = max(0.05 * (1.0 - dot(Normal, lightDir)), 0.005);
    if (Depth + 0.0001 < ProjCoords.z)
        return 0;
    else
        return 1.0;
}

vec3 CalcBumpedNormal()
{
    vec3 Normal = normalize(Normal0); 
    vec3 Tangent = normalize(Tangent0);
    Tangent = normalize(Tangent - dot(Tangent, Normal) * Normal);
    vec3 Bitangent = cross(Tangent, Normal);
    vec3 BumpMapNormal = (texture2D(gMaterial.NormalMap, texCoord.xy)).xyz;
    BumpMapNormal = 2.0 * BumpMapNormal - vec3(1.0, 1.0, 1.0);
    vec3 NewNormal;                              
    mat3 TBN = mat3(Tangent, Bitangent, Normal);    
    NewNormal = TBN * BumpMapNormal;                    
    NewNormal = normalize(NewNormal);               
    return NewNormal;  
}

vec4 CalcLightInternal(BaseLight Light, vec3 pLightDirection, vec3 Normal)
{
    vec3 LightDirection = normalize(pLightDirection);

    float DiffuseFactor = dot(Normal, -LightDirection);

    vec4 DiffuseColor = vec4(0, 0, 0, 0);
    vec4 SpecularColor = vec4(0, 0, 0, 0);

    if (DiffuseFactor > 0)
    {
        DiffuseColor = vec4(Light.Color, 1.0) * Light.Intensity * DiffuseFactor;

        vec3 VertexToEye = normalize(gCameraPos - WorldPos0);
        vec3 LightReflect = normalize(reflect(LightDirection, Normal));
        float SpecularFactor = dot(VertexToEye, LightReflect);

        if (SpecularFactor > 0) 
        {
            SpecularFactor = pow(SpecularFactor, gMaterial.SpecularPower);
            SpecularColor = vec4(Light.Color, 1.0f) * Light.Intensity * SpecularFactor * texture2D(gMaterial.SpecularMap, texCoord.xy);
        }
    }
    return (DiffuseColor + SpecularColor);
}

vec4 CalcDirectionalLight(vec3 Normal)
{
    return CalcLightInternal(gDirectionalLight.Base, gDirectionalLight.Direction, Normal);
}

vec4 CalcPointLight(PointLight pLight, vec3 Normal)
{
    vec3 LightDirection = WorldPos0 - pLight.Position;
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

    if (SpotFactor > sLight.Cutoff1)
    {
        vec4 Color = CalcPointLight(sLight.Base, Normal);
        return Color * (1.0 - (1.0 - SpotFactor) * 1.0 / (1.0 - sLight.Cutoff1));
    }

    return vec4(0, 0, 0, 0);
}