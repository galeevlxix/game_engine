#version 330
out vec4 outputColor;

in vec2 texCoord;
in vec3 vNormal;

uniform sampler2D texture0;
uniform sampler2D texture1;

void main() 
{ 
    vec3 ambientLightIntensity = vec3(0.1, 0.1, 0.1);
    vec3 sunLightIntensity = vec3(0.8, 0.8, 0.8);
    vec3 sunLightDirection = normalize(vec3(20, 20, 10.0));

    vec4 texel = mix(texture(texture0, texCoord), texture(texture1, texCoord), 0.2);

    if (texel.a < 0.3) discard;

    vec3 lightIntensity = ambientLightIntensity + sunLightIntensity * max(dot(vNormal, sunLightDirection), 0.0f);

	outputColor = vec4(texel.rgb * lightIntensity, texel.a);
}             