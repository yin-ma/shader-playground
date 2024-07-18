#version 330 core

in vec2 FragCoord;

out vec4 FragColor;

uniform float u_Time;

float hash21(vec2 p)
{
    vec2 temp = fract(p * vec2(123.456, 345.789));
    temp += dot(temp+0.234, temp+337.456);
    return fract(temp.x * temp.y);
}

float valueNoise(vec2 uv)
{
    vec2 gridUv = fract(uv);
    gridUv = smoothstep(0.0, 1.0, gridUv);
    vec2 gridId = floor(uv);
    
    vec3 col = vec3(0.0);
    float topLeft = hash21(gridId + vec2(0.0, 1.0));
    float topRight = hash21(gridId + vec2(1.0, 1.0));
    float botLeft = hash21(gridId + vec2(0.0, 0.0));
    float botRight = hash21(gridId + vec2(1.0, 0.0));
    
    float top = mix(topLeft, topRight, gridUv.x);
    float bot = mix(botLeft, botRight, gridUv.x);
    
    float noise = mix(bot, top, gridUv.y);
    return noise;
}

float valueNoise(vec2 uv, float octaves)
{
    float col = 0.0;
    float amp = 0.0;
    float t = 1.0;
    
    for (float count=0.0; count<octaves; count++)
    {
        amp += t;
        col += valueNoise(uv * pow(2.0, count+2.0)) * t;
        t /= 2.0;
    }
    return col / amp;
}

void main()
{
    vec2 uv = FragCoord;
    
    vec3 col = vec3(0.0);
    col += vec3(valueNoise(uv, 4.0));


    // Output to screen
    FragColor = vec4(col,1.0);
}