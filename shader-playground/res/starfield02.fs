#version 330 core

in vec2 FragCoord;

out vec4 FragColor;

uniform float u_Time;


mat2 rotate(float a)
{
    float s = sin(a);
    float c = cos(a);
    return mat2(s, c, -c, s);
}

vec3 Star(vec2 uv, float flare)
{
    vec3 col = vec3(0.0);
    vec2 st = uv;
    float d = length(uv);
    d = mix(0.0, 1.0, clamp(d, 0.01, 1.0));
    d = max(0.0, 0.03 / d);
    col += vec3(d);
    col += max(0.0, 1.0 - length(abs(uv.x) * abs(uv.y) * 1000.0)) * flare;
    uv *= rotate(3.1415/4.0);
    col += max(0.0, 1.0 - length(abs(uv.x) * abs(uv.y) * 1000.0)) * smoothstep(1.0, 0.0, length(st)) * flare;
    col *= smoothstep(0.7, 0.0, length(st));
    return col;
}

float hash21(vec2 p)
{
    vec2 res = fract(p * vec2(123.456, 789.012));
    res += dot(res, res*124.345);
    return fract(res.x*res.y);
}

vec3 StarLayer(vec2 uv)
{
    vec3 col = vec3(0.0);
    vec2 gv = fract(uv) - 0.5;
    vec2 id = floor(uv);
    
    for(int y=-1; y<=1; y++)
    {
        for(int x=-1; x<=1; x++)
        {
            vec2 offset = vec2(x, y);
            float n = hash21(id + offset);
            float size = fract(n*987.654);
            vec3 star = Star(gv - offset - vec2(n, fract(n*233.456))+0.5, smoothstep(0.8, 1.0, fract(n)));
            vec3 color = fract(sin(vec3(0.3, 0.4, 0.5)) * n * 345.123);
            color *= vec3(1.0, 0.5, 0.96);
            star *= sin(u_Time*3.0 + n*192.314)*0.5 + 1.0;
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
    float t = u_Time*0.1;
    
    for (float i=0.0; i<1.0; i+=1.0/5.0)
    {
        float depth = fract(t+i);
        float fade = depth*smoothstep(1.0, 0.9, depth);
        col += StarLayer(uv*mix(5.0, 0.5, depth) + i *89.0123) * fade;
   
    }
    

    // Output to screen
    FragColor = vec4(col,1.0);
}