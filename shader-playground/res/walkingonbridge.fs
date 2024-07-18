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

float sdCylinder( vec3 p, float h, float r )
{
  vec2 d = abs(vec2(length(p.xz),p.y)) - vec2(r,h);
  return min(max(d.x,d.y),0.0) + length(max(d,0.0));
}

float sdSphere(vec3 p, float r)
{
    return length(p) - r;
}

float sdBox( vec3 p, vec3 b )
{
    vec3 q = abs(p) - b;
    return length(max(q,0.0)) + min(max(q.x,max(q.y,q.z)),0.0);
}

vec3 offset(vec3 p)
{
    vec3 off = vec3(0.0);
    off.x += sin(p.z*0.2) + sin(p.z*0.137)*3.0;
    off.y += sin(p.z*0.5)*0.2 + p.z*0.3;
    return off;
}

float getDist(vec3 p)
{
    p += offset(p);
    float d = 100.0;
    vec3 rp = p;
    float sizerepeat = 2.0;
    rp.z = rp.z - sizerepeat*round(rp.z/sizerepeat);
    rp.yz *= rotate(rp.z*0.2);

    float bridge = sdBox(rp, vec3(1.0,0.2,2.0));
    d = min(d, bridge);
    rp.x = abs(rp.x) - 1.0;
    float bar = sdBox(rp + vec3(0.0, -0.5, 0.0), vec3(0.05, 0.05, 2.0));
    d = min(d, bar*0.5);
    
    vec3 rp2 = rp;
    float size2 = 0.2;
    rp2.z = rp2.z - size2*round(rp2.z/size2);
    float vBar = sdBox(rp2 + vec3(0.0, -0.25, 0.0), vec3(0.03, 0.2, 0.03));
    d = min(d, vBar*0.5);
    
    vec3 rp3 = p;
    rp3.z = rp3.z - sizerepeat*round(rp3.z/sizerepeat);
    float cylinder = sdCylinder(rp3 + vec3(1.0, -1.5, 0.7), 1.5, 0.1);
    d = min(d, cylinder*0.1);

    return d;
}

vec3 norm(vec3 p)
{
    vec2 e = vec2(0.001, 0.0);
    vec3 n = getDist(p) - vec3(getDist(p - e.xyy), getDist(p - e.yxy), getDist(p - e.yyx));
    return normalize(n);
}

void main()
{
    // Normalized pixel coordinates (from -1 to 1)
    vec2 uv = FragCoord;

    vec3 col = vec3(0.0);
        
    vec3 ro = vec3(0.0, 1.0, -3.0);
    ro.z += u_Time;
    ro -= offset(ro);
    vec3 rd = vec3(uv.x, uv.y, 1.0);
    
    float t = 0.0;
    for (int i=0; i<500; i++)
    {
        vec3 p = ro + t * rd;
        float d = getDist(p);
        t += d;
        if (abs(d) < 0.0001) break;
        if (t > 100.0) break;
    }
    
    vec3 lightPos = vec3(-5.0, 10.0, -7.0);
    if (t < 100.0)
    {
        vec3 p = ro + rd * t;
        vec3 n = norm(p);
        vec3 l = normalize(lightPos-p);

        float diff = clamp(dot(n, l), 0.0, 1.0);
        float amb = 0.1;
        col = vec3(diff) + amb;
    }
            
    FragColor = vec4(col,1.0);
}