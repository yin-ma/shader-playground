#version 330 core

in vec2 FragCoord;

out vec4 FragColor;

uniform float u_Time;

void main()
{
    vec2 uv = FragCoord;
    
    vec3 finalColor = vec3(0.0);
    vec2 st = uv;
    
    for(float i=0.0; i<4.0; i++)
    {
        st = fract(st*1.5) - 0.5;

        float d = length(st) * exp(-length(uv));
        vec3 col = vec3(0.5);

        d = sin(8.0 * d + u_Time * 0.4 + i*0.4) / 8.0;
        d = abs(d);

        d = 0.005 / d; 
        finalColor += col * d;
    }

    FragColor = vec4(finalColor,1.0);
}