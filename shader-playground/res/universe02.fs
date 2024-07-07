#version 330 core

in vec2 FragCoord;

out vec4 FragColor;

uniform float u_Time;

float rand(vec2 co)
{
  return fract(sin(dot(co.xy ,vec2(12.9898,78.233))) * 43758.5453);
}

vec2 getPos(vec2 id)
{
    float x = sin(u_Time*(rand(id)+0.5) + 1.234)*0.35;
    float y = sin(u_Time*(rand(id)+0.5) + 2.234)*0.35;
    return vec2(x, y);
}

float sdLine( vec2 p, vec2 a, vec2 b, float th )
{
    float l = length(b-a);
    vec2  d = (b-a)/l;
    vec2  q = (p-(a+b)*0.5);
          q = mat2(d.x,-d.y,d.y,d.x)*q;
          q = abs(q)-vec2(l,th)*0.5;
    return length(max(q,0.0)) + min(max(q.x,q.y),0.0);    
}

float drawLine(vec2 st, vec2 pos, vec2 nei, float th)
{
    float d2 = sdLine(st, pos, nei, th);
    float dis = length(pos - nei);
    float m = smoothstep(0.03, 0.02, d2);
    m *= smoothstep(1.5, 0.0, dis);
    return m;
}

vec4 layer(vec2 uv)
{
    vec4 col = vec4(0.0);
    
    vec2 st = uv;
    vec2 id = floor(st);
    st = fract(st) - 0.5;
    vec2 pos = getPos(id);
    float d = length(st - pos);
    float t = u_Time;
    
    d = smoothstep(0.07, 0.06, d);
    
    for (float i=-1.0; i<=1.0; i++)
    {
        for (float j=-1.0; j<=1.0; j++)
        {
            vec2 nei = getPos(id - vec2(i, j));
            float d2 = drawLine(st, pos, nei - vec2(i, j), 0.01);
            col += vec4(d2);
        }
    }
    
    
    for (float i=-1.0; i<=1.0; i++)
    {
        for (float j=-1.0; j<=1.0; j++)
        {
            vec2 sparkPos =  - vec2(i, j) + getPos(id - vec2(i, j));
            float spark = 0.005/(length(st - sparkPos) * length(st - sparkPos));
            col += spark*((sin(t * 4.0 * rand(id - vec2(i, j)) + 12.415) + 1.0) * 0.4 + 0.1);
        }
    }
    
    
    col += vec4(drawLine(st, getPos(id - vec2(0.0, 1.0)) - vec2(0.0, 1.0), getPos(id - vec2(-1.0, 0.0)) - vec2(-1.0, 0.0), 0.01));
    col += vec4(drawLine(st, getPos(id - vec2(-1.0, 0.0)) - vec2(-1.0, 0.0), getPos(id - vec2(0.0, -1.0)) - vec2(0.0, -1.0), 0.01));
    col += vec4(drawLine(st, getPos(id - vec2(0.0, -1.0)) - vec2(0.0, -1.0), getPos(id - vec2(1.0, 0.0)) - vec2(1.0, 0.0), 0.01));
    col += vec4(drawLine(st, getPos(id - vec2(1.0, 0.0)) - vec2(1.0, 0.0), getPos(id - vec2(0.0, 1.0)) - vec2(0.0, 1.0), 0.01));
    col += vec4(d);
    return col;
}

void main()
{
    // Normalized pixel coordinates (from 0 to 1)
    vec2 uv = FragCoord;
    
    vec4 col = vec4(0.0);
    
    float t = u_Time * 0.05;
    mat2 rot = mat2(cos(t), sin(t), -sin(t), cos(t));
    for (float i = 0.0; i < 1.0; i+=1.0/4.0)
    {
        float z = fract(i+t);
        float size = mix(5.0, 0.1, z);
        vec4 layer = layer(uv*size*rot + i * 20.0);
        float fade = smoothstep(0.0, 0.5, z) * smoothstep(1.0, 0.9, z);
        col += layer * fade;
    }
    
    FragColor = col;
}