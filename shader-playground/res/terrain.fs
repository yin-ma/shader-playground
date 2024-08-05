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

float hash21(vec2 p)
{
    return fract(sin(dot(p, vec2(555.456, 777.345)))*4243.5432);
}

float valueNoise(vec2 uv)
{
    vec2 gridId = floor(uv);
    vec2 u = fract(uv);
    u = smoothstep(0.0, 1.0, u);

    float a = hash21(gridId+vec2(0,0));
    float b = hash21(gridId+vec2(1,0));
    float c = hash21(gridId+vec2(0,1));
    float d = hash21(gridId+vec2(1,1));
    
    float top = mix(c, d, u.x);
    float bot = mix(a, b, u.x);
    
    return mix(bot, top, u.y);
}

float fbm(vec2 uv, float t)
{
    float a = 1.0;
    float f = 1.0;
    float amb = 0.0;
    mat2 m = rotate(PI/7.654);
    float noise = 0.0;
    vec2 p = uv;
    
    for (float i=0.0; i<t; i++)
    {
        noise += a * valueNoise(p*f);
        p *= m;
        amb += a;
        a *= 0.5;
        f *= 2.0;
    }
    return noise / amb;
}

float sdPlane( vec3 p, vec3 n, float h )
{
  return dot(p,n) + h;
}

vec2 map(vec3 p)
{
    float d = 10.0;
    float t = 0.0;
    float height = fbm(p.xz, 16.0) + 10.0*smoothstep(2.0, 2.1, -p.z);
    float plane = sdPlane(p, vec3(0.0, 1.0, 0.0), height);
    if (plane < d)
    {
        d = plane;
        t = 0.0;
    }
    float sky = sdPlane(p, vec3(0.0, -1.0, 0.0), 6.0);
    if (sky < d)
    {
        d = sky;
        t = 1.0;
    }

    return vec2(d, t);
}

vec3 getNorm(vec3 p)
{
    vec2 e = vec2(0.0001, 0.0);
    vec3 n = map(p).x - vec3(map(p-e.xyy).x, map(p-e.yxy).x, map(p-e.yyx).x);
    return normalize(n);
    
}

vec2 rayMarch(vec3 ro, vec3 rd)
{
    float t = 0.0;
    float a = 0.0;
    for(int i=0; i<200; i++)
    {
        vec3 p = ro + t * rd;
        vec2 ds = map(p);
        t += ds.x;
        a = ds.y;
        if (t > 100.0 || abs(ds.x) < 0.001) break;
    }
    return vec2(t, a);
}

void main()
{
    // Normalized pixel coordinates (from -1 to 1)
    vec2 uv = FragCoord;
    
    vec3 col = vec3(0.0);
    vec3 ro = vec3(0.0, -0.4, 3.0);
    vec3 rd = normalize(vec3(uv.x, uv.y, -3.0));
    
    vec2 t = rayMarch(ro, rd);
    
    vec3 p = ro + t.x * rd;
    if (t.y < 0.5)
    {
        vec3 n = getNorm(p);
        vec3 lightPos = vec3(8.0, 2.0, -2.0);
        float diff = clamp(dot(normalize(lightPos - p), n), 0.0, 1.0);
        col = vec3(diff);
        col += 0.1 * clamp(dot(n, vec3(0.0, 1.0, 0.0)), 0.0, 1.0) * vec3(0.15, 0.85, 0.95);
        
        vec2 sha = rayMarch(p + n*0.01, normalize(lightPos-p));
        if (sha.x < length(lightPos-p))
        {
            col *= 0.07;
        }
        
        // material
        float temp = smoothstep(0.85, 1.0, n.y);
        col *= vec3(0.68, 0.35, 0.21) * (1.0-temp) + temp * vec3(0.6, 0.6, 0.04);
        vec3 fog = exp2(-0.87 * t.x * vec3(0.1, 0.3, 0.9)); 
        col = col*fog + (1.0-fog) * vec3(0.3);
    } 
    else if (t.y < 1.5)
    {
        float cloud = smoothstep(0.5, 0.81, fbm(p.xz*0.1, 8.0)) * 0.7;
        col = (vec3(0.65, 0.85, 0.95) - rd.y) * (1.0 - cloud) + cloud;
    }
    else if (t.y < 2.5)
    {
        col = vec3(1.0, 0.0, 0.0);
    }

    col = pow(col, vec3(0.4545));
    
    // Output to screen
    FragColor = vec4(col,1.0);
}