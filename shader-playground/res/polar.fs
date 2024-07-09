#version 330 core

in vec2 FragCoord;

out vec4 FragColor;

uniform float u_Time;


void main()
{
    vec2 uv = FragCoord;

    float r = length(uv);
    float a = atan(uv.x, uv.y);

    float f = r * cos(a*3);

    
	// uv = vec2(atan(uv.x, uv.y), length(uv));

    float m = fract(uv.x * 1.0);
    float d = smoothstep(cos(3.1415/4), cos(3.1415/4)+0.01, length(uv));
    vec3 col = vec3(f); 
    

    // Output to screen
    FragColor = vec4(col,1.0);
}