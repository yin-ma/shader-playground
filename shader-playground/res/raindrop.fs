#version 330 core

in vec2 FragCoord;

out vec4 FragColor;

uniform float u_Time;
uniform sampler2D u_tex0;

#define S(a, b, t) smoothstep(a, b, t)

float N21(vec2 p)
{
    p = fract(p * vec2(123.34, 345.45));
    p += dot(p, p + 34.345);
    return fract(p.x*p.y);
}


void main()
{
    vec2 uv = FragCoord * 2.0 - 1.0;
    vec2 xy = uv;
    
    // config
    vec4 col = vec4(0.0);
    float t = u_Time;
    vec2 aspect = vec2(2.0, 1.0);
    uv *= 2.7;
    uv = uv * aspect;
    uv.y += t * 0.25;
    vec2 gv = fract(uv) - 0.5;
    
    vec2 id = floor(uv);
    float n = N21(id);
    t += n * 6.21;
    
    float w = xy.y*10.0;
    float x = (n-0.5)*0.8;
    x += (0.4-abs(x)) * sin(3.0*w) * pow(sin(w), 6.0) * 0.45;
    float y = -sin(t + sin(t + sin(t)*0.5)) * 0.45;
    y -= (gv.x-x) * (gv.x-x);
    
    
    vec2 dropPos = (gv - vec2(x, y)) / aspect;
    float drop = S(0.1, 0.09, length(dropPos));
    col += drop;
    
    vec2 trailPos = (gv - vec2(x, t*0.25)) / aspect;
    trailPos.y = (fract(trailPos.y * 8.0) - 0.5) / 8.0;
    float trail = S(0.03, 0.01, length(trailPos));
    float fogTrail = S(-0.05, 0.05, dropPos.y);
    fogTrail *= S(0.5, y, gv.y);
    trail *= fogTrail;
    fogTrail *= S(0.05, 0.04, abs(dropPos.x));
    
    col += fogTrail*0.5;
    col += trail;


    // Output to screen
    vec2 offs = vec2(drop*dropPos + trail*trailPos);
    col = texture(u_tex0, xy + offs * -5.0);
    FragColor = col;
}