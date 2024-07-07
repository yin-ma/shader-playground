#version 330 core

// ref: https://www.shadertoy.com/view/tlyGW3

in vec2 FragCoord;

out vec4 FragColor;

uniform float u_Time;

#define NUM_LAYERS 4.0

mat2 rotate(float a)
{
    float s = sin(a);
    float c = cos(a);
    return mat2(s, c, -c, s);
}

float Star(vec2 uv, float flare)
{
    float d = length(uv);
    float m = 0.05 / d;
    float rays = max(0.0, 1.0 - abs(uv.x * uv.y * 1000.0));
    m += rays * flare;
    uv *= rotate(3.1415 / 4.0);
    rays = max(0.0, 1.0 - abs(uv.x * uv.y * 1000.0));
    m += rays * flare;
    m *= smoothstep(1.0, 0.2, d);
    return m;
}

float Hash21(vec2 p)
{
    p = fract(p * vec2(123.456, 456.789));
    p += dot(p, p + 234.567);
    return fract(p.x * p.y);
}

vec3 StarLayer(vec2 uv)
{
    vec3 col = vec3(0.0);
    vec2 gv = fract(uv) - 0.5;
    vec2 id = floor(uv);

    for (int y = -1; y <= 1; y++)
    {
        for (int x = -1; x <= 1; x++)
        {
            vec2 offs = vec2(x, y);
            float n = Hash21(id + offs);
            float size = fract(n * 321.1);
            float star = Star(gv - offs - vec2(n, fract(n * 34.0)) + 0.5, smoothstep(0.6, 1.0, size));
            vec3 color = sin(vec3(0.6, 0.5, 0.4) * fract(n * 9876.5433) * 1234.5678) * 0.5 + 0.5;
            color = color * vec3(1.0, 0.5, 0.85);
            star *= sin(u_Time * 3.0 + n * 789.2043) * 0.5 + 1.0;
            col += star * size * color;
        }
    }
    return col;
}


void main()
{
    // Normalized pixel coordinates (from 0 to 1)
    vec2 uv = FragCoord;

    vec3 col = vec3(0.0);
    float t = u_Time * 0.1;
    uv *= rotate(t);

    for (float i = 0.0; i < 1.0; i += 1.0 / NUM_LAYERS)
    {
        float depth = fract(i + t);
        float scale = mix(5.0, 0.5, depth);
        float fade = depth * smoothstep(1.0, 0.9, depth);
        col += StarLayer(uv * scale + i * 89.012) * fade;
    }


    // Output to screen
    // if (gv.x > 0.48 || gv.y > 0.48) col = vec3(1.0, 0.0, 0.0);
    FragColor = vec4(col, 1.0);
}