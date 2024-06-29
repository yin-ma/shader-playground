#version 330 core

in vec2 FragCoord;

out vec4 FragColor;

uniform float u_Time;


void main()
{
    vec2 uv = FragCoord;

	// Time varying pixel color
    vec3 col = 0.5 + 0.5*cos(u_Time + uv.xyx + vec3(0,2,4));

    // Output to screen
    FragColor = vec4(col,1.0);
}