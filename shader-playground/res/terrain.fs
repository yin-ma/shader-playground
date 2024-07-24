#version 330 core

in vec2 FragCoord;

out vec4 FragColor;

uniform float u_Time;

float PI = acos(-1.0);

mat2 rotate(float a)
{
    float ca = cos(a);
    float sa = sin(a);
    return mat2(ca, sa, -sa, ca);
}

float hash21(vec2 p)
{
    return fract(sin(dot(p, vec2(4677.456, 777.345)))*4443.5432);
}

float valueNoise(vec2 uv)
{
    vec2 gridId = floor(uv);
    vec2 u = fract(uv);
    u = smoothstep(0.0, 1.0, u);

    float a = hash21(gridId+vec2(0,0));
    float b = hash21(gridId+vec2(1,0));
    float c = hash21(gridId+vec2(0,1));
    float d = hash21(gridId+vec2(1,1));
    
    float top = mix(c, d, u.x);
    float bot = mix(a, b, u.x);
    
    return mix(bot, top, u.y);
}

float fbm(vec2 uv, float t)
{
    float a = 1.0;
    float f = 1.0;
    float amb = 0.0;
    mat2 m = rotate(PI/7.654);
    float noise = 0.0;
    vec2 p = uv;
    
    for (float i=0.0; i<t; i++)
    {
        noise += a * valueNoise(p*f);
        p *= m;
        amb += a;
        a *= 0.5;
        f *= 2.0;
    }
    return noise / amb;
}

void main()
{
    // Normalized pixel coordinates (from -1 to 1)
    vec2 uv = FragCoord;
    
    vec3 col = vec3(0.0);
    
    col = vec3(fbm(uv, 4.0));
    // Output to screen
    FragColor = vec4(col,1.0);
}