#version 330 core

in vec2 FragCoord;

out vec4 FragColor;

uniform float u_Time;

mat2 rotate(float a)
{
    float ca = cos(a);
    float sa = sin(a);
    return mat2(ca, sa, -sa, ca);
}

float getDist(vec3 p)
{
    return length(p) - 1.0;
}

vec3 getNorm(vec3 p)
{
    vec2 e = vec2(0.0001, 0.0);
    vec3 n = getDist(p) - vec3(getDist(p-e.xyy), getDist(p-e.yxy), getDist(p-e.yyx));
    return normalize(n);
}

void main()
{
    // Normalized pixel coordinates (from -1 to 1)
    vec2 uv = FragCoord;

    // Ray March
    vec3 col = vec3(0.0);
    
    vec3 ro = vec3(0.0, 0.0, 3.0);
    vec3 rd = normalize(vec3(uv.x, uv.y, -1.0));
    
    float steps = 0.0;
    for (int i=0; i<100; i++)
    {
        vec3 p = ro + steps * rd;
        float ds = getDist(p);
        steps += ds;
        if (ds < 0.001 || steps > 100.0) break;
    }
    
    // lighting
    vec3 lightPos = vec3(12.0, 5.0, 2.0);
    lightPos.xz *= rotate(u_Time);
    if (steps < 100.0)
    {
        vec3 p = ro + steps * rd;
        vec3 n = getNorm(p);
        vec3 lightDir = normalize(lightPos - p);
        float diff = clamp(dot(n, lightDir), 0.0, 1.0);
        float amb = 0.1;
        float spec = pow(clamp(dot(normalize(ro-p), reflect(-lightDir, n)), 0.0, 1.0), 64.0);
        col = vec3(diff + amb + spec);
        col *= vec3(1.0, 0.1, 0.2);
    }
    
    // Output to screen
    FragColor = vec4(col,1.0);
}