#version 330 core

in vec2 FragCoord;

out vec4 FragColor;

uniform float u_Time;

#define MAX_STEPS 100
#define MAX_DIST 100.
#define SURF_DIST .001
#define PI 3.141592
#define S smoothstep
#define T u_Time

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

float getDist(vec3 p) 
{
    float d = 100.0;
    float sphere = sdSphere(p, 1.0);
    float box = sdBox(p + vec3(2.5, 0.5, 0.0), vec3(0.5));
    float plane = sdPlane(p, vec3(0.0, 1.0, 0.0), 1.0);
    
    d = min(d, sphere);
    d = min(d, box);
    d = min(d, plane);
    
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
    
    float dif = clamp(dot(n, l), 0.0, 1.0);
    float amb = 0.1;
    float d = rayMarch(p+n*SURF_DIST*2., l);
    if(d<length(lightPos-p)) dif *= .1;
    return clamp(dif + amb, 0.0, 1.0);
}


void main()
{
    // Normalized pixel coordinates (from -1 to 1)
    vec2 uv = FragCoord;

    vec3 col = vec3(0.05);
    
    // camera setup
    vec3 ro = vec3(0, 3.0, -3.0);
    vec3 up = vec3(0.0, 1.0, 0.0);
    vec3 center = vec3(0.0);
    float zoom = 1.0;

    Camera camera = Camera(ro, center, up, cross(up, center));
    vec3 rd = getRayDir(uv, camera, zoom);
    vec3 lightPos = vec3(1.0, 5.0, 2.0);
    lightPos.xz *= rotate(T);
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