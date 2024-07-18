#version 330 core

in vec2 FragCoord;

out vec4 FragColor;

uniform float u_Time;

#define MAX_STEPS 100
#define MAX_DIST 100.0
#define SURF_DIST 0.001
#define PI 3.141592
#define S smoothstep
#define T iTime

struct Camera
{
    vec3 position;
    vec3 center;
    vec3 up;
    vec3 right;
};

mat2 rotate(float a) {
    float s=sin(a);
    float c=cos(a);
    return mat2(c, -s, s, c);
}

vec3 getRayDir(vec2 uv, Camera camera, float zoom) {
    vec3 
        f = normalize(camera.center-camera.position),
        r = normalize(cross(camera.up, f)),
        u = cross(f,r),
        c = f*zoom,
        i = c + uv.x*r + uv.y*u;
    return normalize(i);
}

float sdSphere( vec3 p, float r )
{
  return length(p) - r;
}


float sdBox(vec3 p, vec3 s) 
{
    p = abs(p)-s;
	return length(max(p, 0.))+min(max(p.x, max(p.y, p.z)), 0.);
}

float sdPlane( vec3 p, vec3 n, float h )
{
  // n must be normalized
  return dot(p,n) + h;
}

float sdCappedCylinder( vec3 p, float h, float r )
{
  vec2 d = abs(vec2(length(p.xz),p.y)) - vec2(r,h);
  return min(max(d.x,d.y),0.0) + length(max(d,0.0));
}

vec2 opRepLim( vec2 p, float s, vec2 lim )
{
    return p-s*clamp(round(p/s),-lim,lim);
}


float getDist(vec3 p) 
{
    vec3 q = p;
    
    q.xz = opRepLim(q.xz, 4.0, vec2(4.0, 2.0));
    float d = 100.0;
    float cylinder = sdCappedCylinder(q, 2.0, 0.5 - 0.1*q.y - 0.1 * (0.5 + 0.5 * sin(12.0*atan(q.z, q.x))));
    d = min(d, cylinder*0.4);
    
    vec3 qq = vec3(q.x, abs(q.y)-2.0, q.z);
    float box = sdBox(qq, vec3(0.85, 0.1, 0.85));
    d = min(d, box);
    
    float bigBox = sdBox(p, vec3(16.0, 2.2, 6.0));
    d = max(d, -bigBox);
    
    vec3 s = p;
    s.xz = opRepLim(s.xz, 4.1, vec2(4.0, 2.0));
    float base = sdBox(s - vec3(0.0, -2.3, 0.0), vec3(2.0, 0.2, 2.0));
    d = min(d, base);
    
    return d;
}


float rayMarch(vec3 ro, vec3 rd) 
{
	float d=0.;
    
    for(int i=0; i<MAX_STEPS; i++) {
    	vec3 p = ro + rd*d;
        float dS = getDist(p);
        d += dS;
        if(d>MAX_DIST || abs(dS)<SURF_DIST) break;
    }
    return d;
}

vec3 getNormal(vec3 p) {
    vec2 e = vec2(.001, 0);
    vec3 n = getDist(p) - 
        vec3(getDist(p-e.xyy), getDist(p-e.yxy), getDist(p-e.yyx));
    
    return normalize(n);
}

float getLight(vec3 p, vec3 lightPos) {
    vec3 l = normalize(lightPos-p);
    vec3 n = getNormal(p);
    
    float dif = clamp(dot(n, l), 0., 1.);
    float amb = 0.2;
    float d = rayMarch(p+n*SURF_DIST*2., l);
    if(d<length(lightPos-p)) dif *= .1;
    
    return clamp(dif + amb, 0.0, 1.0);
}


void main()
{
    vec2 uv = FragCoord;

    vec3 col = vec3(0.05);
    
    // camera setup
    vec3 ro = vec3(0, 3.0, -20.0);
    vec3 up = vec3(0.0, 1.0, 0.0);
    vec3 center = vec3(0.0);
    float zoom = 1.0;

    Camera camera = Camera(ro, center, up, cross(up, center));
    vec3 rd = getRayDir(uv, camera, zoom);
    vec3 lightPos = vec3(1.0, 5.0, 2.0);
    lightPos.xz *= rotate(u_Time);
    float d = rayMarch(camera.position, rd);
    
    if(d<MAX_DIST) {
        vec3 p = ro + rd * d;
        vec3 n = getNormal(p);
        float light = getLight(p, lightPos);
        col = vec3(light);
    }
    
    // Output to screen
    FragColor = vec4(col,1.0);
}