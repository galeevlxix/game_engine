#version 330
out vec4 outputColor;

in vec2 texCoord;
in vec3 vNormal;

uniform sampler2D texture0;

void main() 
{ 
    vec3 ambientLightIntensity = vec3(0.3, 0.3, 0.3);
    vec3 sunLightIntensity = vec3(1, 1, 1);
    vec3 sunLightDirection = normalize(vec3(-20, 20, 20.0));

    vec4 texel = texture(texture0, texCoord);

    vec3 lightIntensity = ambientLightIntensity + sunLightIntensity * max(dot(vNormal, sunLightDirection), 0.0f);

	outputColor = vec4(texel.rgb * lightIntensity, texel.a);
}             