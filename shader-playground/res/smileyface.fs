// ref https://www.shadertoy.com/view/lsXcWn

#version 330 core

in vec2 FragCoord;

out vec4 FragColor;

uniform float u_Time;


float remap(float a, float b, float t)
{
    return clamp((t - a) / (b - a), 0.0, 1.0);
}

vec4 Head(vec2 uv)
{
    vec4 col = vec4(0.9, 0.65, 0.1, 1.0);
    uv.x = abs(uv.x);
    float d = length(uv);
    col.a = smoothstep(1.0, 0.98, d);
    
    float edgeShadow = remap(0.7, 1.0, d);
    edgeShadow *= edgeShadow;
    col.rgb *= 1.0 - edgeShadow * 0.5;
    
    col.rgb = mix(col.rgb, vec3(0.6, 0.3, 0.1), smoothstep(0.9, 1.0, d));
    
    float highlight = smoothstep(0.8, 0.79, d);
    highlight *= remap(0.0, 1.0, uv.y);
    col.rgb = mix(col.rgb, vec3(1.0), highlight);
    
    float cheek = smoothstep(0.3, 0.03, length(uv - vec2(0.5, -0.3)));
    cheek *= smoothstep(0.2, 0.3, d);
    col.rgb = mix(col.rgb, vec3(1.0, 0.0, 0.0), cheek * 0.21);
    
    return col;
}

vec4 Eye(vec2 uv)
{
    uv.x = abs(uv.x);
    vec4 col = vec4(1.0);
    float d = length(uv - vec2(0.4, 0.15));
    col.a = smoothstep(0.3, 0.29, d);
    col.rgb = mix(col.rgb, vec3(1.0), d);
    
    col.rgb = mix(col.rgb, vec3(0.0), (1.0 - smoothstep(0.3, 0.28, length(uv - vec2(0.41, 0.16))))*0.3);
    
    vec4 irisCol = vec4(0.3, 0.66, 1.0, 1.0);
    col.rgb = mix(col.rgb, irisCol.rgb, smoothstep(0.1, 0.29, d)*0.22);
    col.rgb = mix(col.rgb, vec3(0.0), smoothstep(0.2, 0.19, d));
    col.rgb = mix(col.rgb, irisCol.rgb, smoothstep(0.18, 0.17, d));
    col.rgb = mix(col.rgb, vec3(0.0), smoothstep(0.18, 0.17, d) * smoothstep(0.1, 0.2, d)*0.3);
    col.rgb = mix(col.rgb, vec3(1.0), smoothstep(0.17, 0.01, d));
    col.rgb = mix(col.rgb, vec3(0.0), smoothstep(0.1, 0.09, d));
    
    col.rgb = mix(col.rgb, vec3(1.0), smoothstep(0.08, 0.07, length(uv - vec2(0.3, 0.25))));
    col.rgb = mix(col.rgb, vec3(1.0), smoothstep(0.04, 0.03, length(uv - vec2(0.47, 0.08))));
    
    return col;
}

vec4 Mouth(vec2 uv)
{
    vec4 col = vec4(0.5, 0.18, 0.05, 1.0);
    vec2 st = uv;
    uv.y *= 3.0;
    uv.y -= uv.x * uv.x * 2.2;
    float d = length(uv - vec2(0.0, -1.6));
    col.a = smoothstep(0.5, 0.48, d);
    
    col.rgb = mix(col.rgb, vec3(1.0), smoothstep(0.6, 0.59, length(st - vec2(0.0, 0.15))));
    uv.y = st.y - 3.0;
    uv.y = st.y - st.x * st.x * 1.23;
    col.rgb = mix(col.rgb, vec3(1.0, 0.5, 0.5), smoothstep(0.6, 0.46, length(uv - vec2(0.0, -1.1))));
    return col;
}

void main()
{
    // Normalized pixel coordinates (from 0 to 1)
    vec2 uv = FragCoord;

    vec4 col = vec4(0.0);
    
    float d = length(uv);
    vec4 head = Head(uv);
    col = mix(col, head, head.a);
    
    vec4 eye = Eye(uv);
    col = mix(col, eye, eye.a);
    
    vec4 mouth = Mouth(uv);
    col = mix(col, mouth, mouth.a);
    

    // Output to screen
    FragColor = col;
}