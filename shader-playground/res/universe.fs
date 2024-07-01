// ref: https://www.youtube.com/watch?v=KGJUl8Teipk
#version 330 core

in vec2 FragCoord;

out vec4 FragColor;

uniform float u_Time;

#define S(a, b, t) smoothstep(a, b, t)

float sdLine(vec2 p, vec2 a, vec2 b)
{
    vec2 pa = p-a, ba = b-a;
    float h = clamp( dot(pa,ba)/dot(ba,ba), 0.0, 1.0 );
    return length( pa - ba*h );
}

float N21(vec2 p)
{
    p = fract(p*vec2(633.35, 803.61));
    p += dot(p, p+12.34);
    return fract(p.x*p.y);
}

vec2 N22(vec2 p)
{
    float n = N21(p);
    return vec2(n, N21(p+n+0.1));
}

float drawLine(vec2 uv, vec2 a, vec2 b, float width, float blur)
{
    float d = sdLine(uv, a, b);
    float m = length(S(width, width-blur, d));
    float d2 = length(a-b);
    m *= S(1.2, 0.8, d2) * 0.5 + S(0.05, 0.03, abs(d2 - 0.75));
    return m;
}

vec2 getPos(vec2 id)
{
    vec2 n = N22(id);
    float x = sin(u_Time * n.x);
    float y = cos(u_Time * n.y);
    return vec2(x, y)* 0.3;
}

vec3 Layer(vec2 uv)
{
    // config
    vec3 col = vec3(0.0);
    uv *= 0.5;
    vec2 gv = fract(uv) - 0.5;
    vec2 id = floor(uv);
    float t = u_Time;
    
    vec2 p[9];
    int i = 0;
    for (float y=-1.0; y<=1.0; y++)
    {
        for (float x=-1.0; x<=1.0; x++)
        {
            p[i++] = vec2(x,y) + getPos(id + vec2(x,y));
        }
    }
    
    // draw point
    float d = length(gv - p[4]);
    vec2 lightDist = (p[4] - gv) * 20.0;
    float sparkle = 1.0 / dot(lightDist, lightDist);
    col += (sparkle * (sin(t*p[4].x*0.01 + fract(p[4].x*p[4].y*10.0))*0.5 + 0.5)) * 1.4;
    col += S(0.1, 0.05, d);
    
    // draw lines
    for (int i=0; i<9; i++)
    {
        float line = drawLine(gv, p[4], p[i], 0.03, 0.01);
        col += vec3(line);
    }
    col += vec3(drawLine(gv, p[1], p[3], 0.03, 0.01));
    col += vec3(drawLine(gv, p[1], p[5], 0.03, 0.01));
    col += vec3(drawLine(gv, p[3], p[7], 0.03, 0.01));
    col += vec3(drawLine(gv, p[5], p[7], 0.03, 0.01));
    
    return col;
}

void main()
{
    vec2 uv = FragCoord;
    
    // config
    vec3 col = vec3(0.0);
    float t = u_Time * 0.1;
    mat2 rot = mat2(cos(t), sin(t), -sin(t), cos(t));
    for (float i = 0.0; i <= 1.0; i+=1.0/4.0)
    {
        float z = fract(i+t);
        float size = mix(10.0, 0.5, z);
        vec3 layer = Layer(uv*size*rot + i * 20.0);
        float fade = S(0.0, 0.5, z) * S(1.0, 0.8, z);
        col += layer * fade;
    }

    // Output to screen
    FragColor = vec4(col,1.0);
}