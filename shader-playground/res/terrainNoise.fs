#version 330 core

in vec2 FragCoord;

out vec4 FragColor;

uniform float u_Time;


float hash21(vec2 uv)
{
    return fract(sin(dot(uv, vec2(21.231, 37.314))*17.123)*456.789);
}

vec3 noised(vec2 uv)
{
    vec2 f = fract(uv);
    vec2 u = f*f*(3.0-2.0*f);
    vec2 du = 6.0*f*(1.0-f);
    
    vec2 p = floor(uv);
	float a = hash21(p + vec2(0.0, 0.0));
	float b = hash21(p + vec2(1.0, 0.0));
	float c = hash21(p + vec2(0.0, 1.0));
	float d = hash21(p + vec2(1.0, 1.0));

	return vec3(a+(b-a)*u.x+(c-a)*u.y+(a-b-c+d)*u.x*u.y,
				du*(vec2(b-a,c-a)+(a-b-c+d)*u.yx));
}

const mat2 m2 = mat2(0.8,-0.6,0.6,0.8);

float terrain(vec2 x)
{
	vec2  p = x;
    float a = 0.0;
    float b = 1.0;
	vec2  d = vec2(0.0);
    float tot = 0.0;
    for( int i=0; i<9; i++ )
    {
        vec3 n = noised(p);
        d += n.yz;
        a += b*n.x/(1.0+dot(d,d));
        tot += b;
		b *= 0.5;
        p = m2*p*2.0;
    }

	return a / tot;
}

float fbm(vec2 p)
{
    float a = 1.0;
    float n = 0.0;
    float t = 0.0;
    for (int i=0; i<9; i++)
    {
        n += a * noised(p).x;
        t += a;
        a *= 0.5;
        p = m2*p*2.0;
    }
    
    return n / t; 
}


void main()
{
    // Normalized pixel coordinates (from 0 to 1)
    vec2 uv = FragCoord;

    vec3 col = vec3(0.01, 0.01, 0.3);
    
    col = vec3(terrain(uv*8.0));
    
    // Output to screen
    FragColor = vec4(col,1.0);
}