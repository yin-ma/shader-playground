#version 330 core

in vec2 FragCoord;

out vec4 FragColor;

uniform float u_Time;

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

void main()
{
    // Normalized pixel coordinates (from -1 to 1)
    vec2 uv = FragCoord;
    uv *= 4.0;

    // Time varying pixel color
    vec3 col = vec3(0.0);
    float n = fbm(uv*2.0 + vec2(23.123, 32.321) - vec2(0.2*u_Time, 3.7*u_Time));
    n = smoothstep(0.0, 1.0, n);
    
    float f = 1.0 - 2.3*(length(uv * vec2(-17.4*(-0.2-0.2*uv.y), 1.1*uv.y - 0.1)) - 0.3*(1.0-uv.y) - 1.1 * n * uv.y);
    float c = clamp(f, 0.0, 1.0) * n * 1.7 * (1.0-0.4*uv.y);
    
    col = vec3(2.2*c*c, 1.4*c*c*c, c*c*c*c*c);

    // Output to screen
    FragColor = vec4(col,1.0);
}