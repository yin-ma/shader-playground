#version 330 core

in vec2 FragCoord;

out vec4 FragColor;

uniform float u_Time;

vec2 hash22(vec2 p)
{
  p = p + 0.02;
  float x = dot(p, vec2(123.4, 234.5));
  float y = dot(p, vec2(234.5, 345.6));
  vec2 gradient = vec2(x, y);
  gradient = sin(gradient);
  gradient = gradient * 43758.5453;

  gradient = sin(gradient);
  return gradient;
}

float hash21(vec2 p)
{
    vec2 temp = fract(p * vec2(123.456, 345.789));
    temp += dot(temp+1.234, temp+337.456);
    return fract(temp.x * temp.y);
}

float perlinNoise(vec2 uv)
{
    vec2 gridUv = fract(uv);
    vec2 gridId = floor(uv);
    
    vec2 botLeft  = hash22(gridId + vec2(0.0, 0.0));
    vec2 botRight = hash22(gridId + vec2(1.0, 0.0));
    vec2 topLeft  = hash22(gridId + vec2(0.0, 1.0));
    vec2 topRight = hash22(gridId + vec2(1.0, 1.0));
    
    float dotBotLeft = dot(botLeft, gridUv - vec2(0.0, 0.0));
    float dotBotRight = dot(botRight, gridUv - vec2(1.0, 0.0));
    float dotTopLeft = dot(topLeft, gridUv - vec2(0.0, 1.0));
    float dotTopRight = dot(topRight, gridUv - vec2(1.0, 1.0));
    
    gridUv = smoothstep(0.0, 1.0, gridUv);
    float b = mix(dotBotLeft, dotBotRight, gridUv.x);
    float t = mix(dotTopLeft, dotTopRight, gridUv.x);
    
    return mix(b, t, gridUv.y);
}

float perlinNoise(vec2 uv, float octaves)
{
    float col = 0.0;
    float t = 1.0;
    
    for (float count=0.0; count<octaves; count++)
    {
        col += perlinNoise(uv * pow(2.0, count+2.0)) * t;
        t /= 2.0;
    }
    return col;
}

void main()
{
    // Normalized pixel coordinates (from 0 to 1)
    vec2 uv = FragCoord;
    
    vec3 col = vec3(0.0);
    col += vec3(perlinNoise(uv, 4.0));


    // Output to screen
    FragColor = vec4(col,1.0);
}