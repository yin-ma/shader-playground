#version 330 core

in vec2 FragCoord;

out vec4 FragColor;

uniform float u_Time;

vec2 hash22(vec2 p)
{
  p = p + 0.02;
  float x = dot(p, vec2(123.4, 234.5));
  float y = dot(p, vec2(234.5, 345.6));
  vec2 gradient = vec2(x, y);
  gradient = sin(gradient);
  gradient = gradient * 43758.5453;

  // part 4.5 - update noise function with time
  gradient = sin(gradient);
  return gradient;
}

float sdSegment( vec2 p, vec2 a, vec2 b )
{
    vec2 pa = p-a, ba = b-a;
    float h = clamp( dot(pa,ba)/dot(ba,ba), 0.0, 1.0 );
    return length( pa - ba*h );
}


void main()
{
    // Normalized pixel coordinates (from 0 to 1)
    vec2 uv = FragCoord;

    vec3 col = vec3(0.0);
    vec2 gridUv = fract(uv*8.0);
    vec2 gridId = floor(uv*8.0);
    
    vec2 gradBL = hash22(gridId + vec2(0.0, 0.0));
    vec2 gradBR = hash22(gridId + vec2(1.0, 0.0));
    vec2 gradTL = hash22(gridId + vec2(0.0, 1.0));
    vec2 gradTR = hash22(gridId + vec2(1.0, 1.0));
    
    // visualize grad
    float distBL = smoothstep(0.011, 0.010, sdSegment(gridUv, vec2(0.0, 0.0), vec2(0.0, 0.0) + gradBL));
    float distBR = smoothstep(0.011, 0.010, sdSegment(gridUv, vec2(1.0, 0.0), vec2(1.0, 0.0) + gradBR));
    float distTL = smoothstep(0.011, 0.010, sdSegment(gridUv, vec2(0.0, 1.0), vec2(0.0, 1.0) + gradTL));
    float distTR = smoothstep(0.011, 0.010, sdSegment(gridUv, vec2(1.0, 1.0), vec2(1.0, 1.0) + gradTR));
    float test = sdSegment(gridUv, vec2(0.0, 0.0), vec2(0.5, 0.5));
    test = smoothstep(0.05, 0.051, test);
    
    // dot
    float dotBL = dot(gradBL, gridUv - vec2(0.0, 0.0));
    float dotBR = dot(gradBR, gridUv - vec2(1.0, 0.0));
    float dotTL = dot(gradTL, gridUv - vec2(0.0, 1.0));
    float dotTR = dot(gradTR, gridUv - vec2(1.0, 1.0));
    
    gridUv = smoothstep(0.0, 1.0, gridUv);
    float b = mix(dotBL, dotBR, gridUv.x);
    float t = mix(dotTL, dotTR, gridUv.x);
    float perlin = mix(b, t, gridUv.y);
    
    // col += vec3(distBL);
    // col += vec3(distBR);
    // col += vec3(distTL);
    // col += vec3(distTR);
    
    col += vec3(perlin);
    
    // if (gridUv.x > 0.99 || gridUv.y > 0.99) col = vec3(1.0, 0.0, 0.0);

    // Output to screen
    FragColor = vec4(col,1.0);
}