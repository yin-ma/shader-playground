#version 330 core

in vec2 FragCoord;

out vec4 FragColor;

uniform float u_Time;

#define MAX_STEPS 100
#define MAX_DIST 100.0
#define SURF_DIST 0.01

float sdSphere( vec3 p, float s )
{
  return length(p)-s;
}

float sdBox( vec3 p, vec3 b )
{
  vec3 q = abs(p) - b;
  return length(max(q,0.0)) + min(max(q.x,max(q.y,q.z)),0.0);
}

float sdPlane( vec3 p, vec3 n, float h )
{
  // n must be normalized
  return dot(p,n) + h;
}

float getDist(vec3 p)
{
    float sphere = sdSphere(p - vec3(-1.5, 1.0, 4.0), 1.0);
    float box = sdBox(p - vec3(1.5, 1.0, 4.0), vec3(1.0, 1.0, 1.0));
    float plane = sdPlane(p, vec3(0.0, 1.0, 0.0), 0.0);
    return min(box, min(sphere, plane));
}

float rayMarch(vec3 ro, vec3 rd)
{
    float t = 0.0;
    
    for(int i=0; i<MAX_STEPS; i++)
    {
        vec3 p = ro + rd * t;
        float d = getDist(p);
        t += d;
        if(t > MAX_DIST || d < SURF_DIST) break;
    }
    
    return t;
}

vec3 getNormal(vec3 p)
{
    float d = getDist(p);
    vec2 e = vec2(0.0001, 0.0);

    vec3 n = d - vec3(getDist(p - e.xyy), getDist(p - e.yxy), getDist(p - e.yyx));
    return normalize(n);
}

float getLight(vec3 p, vec3 lightPos)
{
    vec3 l = normalize(lightPos - p);
    vec3 n = getNormal(p);

    float diff = clamp(dot(n, l), 0.0, 1.0);
    float d = rayMarch(p+n*SURF_DIST*2.0, l);
    // if (d < length(lightPos - p)) diff *= 0.1;
    diff *= mix(1.0, 0.1, step(d, length(lightPos - p)));
    return diff;
}

void main()
{
    // Normalized pixel coordinates (from 0 to 1)
    vec2 uv = FragCoord;
    
    vec3 col = vec3(0.0);
    vec3 ro = vec3(0.0, 1.0, 0.0);
    vec3 rd = normalize(vec3(uv.x, uv.y, 1.0));
    
    float d = rayMarch(ro, rd);
    vec3 p = ro + rd * d;
    // col = vec3(d*0.1);
    vec3 lightPos = vec3(0.0, 5.0, 0.0);
    lightPos.xz += vec2(sin(u_Time), cos(u_Time)) * 4.0;
    float diff = getLight(p, lightPos);
    col = vec3(diff);

    // Output to screen
    FragColor = vec4(col,1.0);
}