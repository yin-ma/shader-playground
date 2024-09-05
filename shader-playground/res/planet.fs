#version 330 core

in vec2 FragCoord;

out vec4 FragColor;

uniform float u_Time;

float PI = acos(-1.0);

mat2 rotate(float an)
{
    float ca = cos(an);
    float sa = sin(an);
    return mat2(ca, sa, -sa, ca);
}

float noise(vec2 p)
{
    return fract(sin(dot(p, vec2(13.124, 30.124))*7.123)*3214.120);
}

float valueNoise(vec2 p)
{
    vec2 id = floor(p);
    vec2 uv = fract(p);
    uv = smoothstep(0.0, 1.0, uv);
    return mix(mix(noise(id + vec2(0.0, 0.0)), noise(id + vec2(1.0, 0.0)), uv.x),
               mix(noise(id + vec2(0.0, 1.0)), noise(id + vec2(1.0, 1.0)), uv.x),
               uv.y);
}

float fbm(vec2 p)
{
    float a = 0.49;
    float n = 0.0;
    mat2 m = mat2(0.96, 0.55, -0.55, 0.96);
    for(int i=0; i<16; i++)
    {
        n += a * valueNoise(p);
        a *= 0.48;
        p = 1.7 * m * p;
    }
    return n;
}


float map(vec3 p)
{
    return length(p - vec3(0.0, 0.0, 0.0)) - 1.0;
}

vec3 getNorm(vec3 p)
{
    vec2 e = vec2(0.0001, 0.0);
    vec3 n = map(p) - vec3(map(p-e.xyy), map(p-e.yxy), map(p-e.yyx));
    return normalize(n);
}

void main()
{
    // Normalized pixel coordinates (from 0 to 1)
    vec2 uv = FragCoord;

    // Time varying pixel color
    vec3 col = vec3(0.1);
    
    vec3 ro = vec3(0.0, 0.0, 2.0);
    vec3 rd = normalize(vec3(uv, -1.0));
    float t = 0.0;
    
    for (int i=0; i<100; i++)
    {
        vec3 p = ro + t * rd;
        float d = map(p);
        t += d;
        if (t>100.0 || d<0.01) break;
        
    }
    
    if (t < 100.0)
    {
        vec3 p = ro + t * rd;
        vec3 lightPos = normalize(vec3(10.0, 6.0, 16.0));
        vec3 n = getNorm(p);
        
        // texture
        vec2 uv, st;
        uv.x = atan(n.x,n.z)/6.2831 - 0.1*u_Time;
        uv.y = acos(n.y)/3.1416;
        uv *= 8.0;
        
        st.x = atan(n.x,n.z)/6.2831 - 0.09*u_Time;
        st.y = acos(n.y)/3.1416 - 0.02*u_Time;
        st *= 8.0;
        float noi = fbm(uv + vec2(23.0, 22.0));
        noi = smoothstep(0.3, 0.4, noi);
        float cloud = fbm(st + vec2(2.0, 2.0));
        cloud = smoothstep(0.4, 0.8, cloud);
        
        vec3 tex = mix(vec3(0.1, 0.8, 0.3), vec3(0.1, 0.3, 0.88), noi);
        tex = mix(tex, vec3(0.9), cloud);
        
        // lighting
        float diff = clamp(dot(n, lightPos), 0.0, 1.0);
        float spec = clamp( dot( n, normalize(lightPos)), 0.0, 1.0 );
        spec = pow(spec, 8.0);
        float fre = 1.0 - clamp(n.z,0.0,1.0);
        
        //tex = mix(vec3(0.1, 0.8, 0.3), vec3(0.1, 0.3, 0.7), noi);
        col = tex*(diff + spec);
        col = mix(col, col+0.25, fre);
    }
    else
    {
        col = vec3(0.03);
    }
    
    // Output to screen
    FragColor = vec4(col,1.0);
}