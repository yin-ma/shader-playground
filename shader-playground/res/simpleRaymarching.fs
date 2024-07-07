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

void main()
{
    // Normalized pixel coordinates (from 0 to 1)
    vec2 uv = FragCoord;
    
    vec3 col = vec3(0.0);
    vec3 ro = vec3(0.0, 1.0, 0.0);
    vec3 rd = normalize(vec3(uv.x, uv.y, 1.0));
    
    float d = rayMarch(ro, rd);
    
    col = vec3(d*0.1);

    // Output to screen
    FragColor = vec4(col,1.0);
}