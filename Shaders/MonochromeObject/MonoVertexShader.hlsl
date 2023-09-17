﻿#version 330                                           
layout (location = 0) in vec3 aPosition; 

uniform mat4 world;
uniform mat4 campos;
uniform mat4 camrot;
uniform mat4 pers;

void main()                                            
{        
	gl_Position = vec4(aPosition, 1.0) * world * campos * camrot * pers;
}