#version 330 core

in vec2 FragCoord;

out vec4 FragColor;

uniform float u_Time;

float gridPlate(vec2 uv)
{
    uv.x += 0.50;
    vec2 p = uv;
    
    // plate
    uv = vec2(p.x,1.0)/abs(p.y) + u_Time;
    
    // checker board
    uv *= 12.0;
    float n = sin(uv.x) * sin(uv.y);
    n = step(0.0, n);
    
    float fade = abs(p.y);
    fade = smoothstep(0.2, 1.0, fade);
    return n*fade*0.8;
}

void main()
{
    // Normalized pixel coordinates (from 0 to 1)
    vec2 uv = FragCoord;

    // Time varying pixel color
    vec3 col = vec3(0.1);
    
    col = vec3(gridPlate(uv));

    // Output to screen
    FragColor = vec4(col,1.0);
}