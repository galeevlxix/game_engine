#version 330                                           
layout (location = 0) in vec3 aPosition;            
out vec4 vertexColor;                                  
uniform mat4 mvp;                                      
void main()                                            
{                                                      
   gl_Position = vec4(aPosition, 1.0) * mvp;           
   vertexColor = vec4(clamp(aPosition, 0.0, 1.0), 1.0);
}