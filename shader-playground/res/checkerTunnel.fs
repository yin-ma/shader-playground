#version 330 core

in vec2 FragCoord;

out vec4 FragColor;

uniform float u_Time;

float grid(vec2 uv)
{
    vec2 st = uv;
    uv = vec2(1.0/length(uv), atan(uv.y, uv.x));
    uv *= 12.0;
    uv.x += u_Time*6.0;
    
    float n = sin(uv.x) * sin(uv.y);
    n = step(0.0, n);
    
    float d = length(st);
    d = pow(d, 1.2);
    
    return n*d;
}

void main()
{
    // Normalized pixel coordinates (from 0 to 1)
    vec2 uv = FragCoord;

    // Time varying pixel color
    vec3 col = vec3(0.1);
    
    col = vec3(grid(uv));

    // Output to screen
    FragColor = vec4(col,1.0);
}