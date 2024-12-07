#version 330 core

in vec2 FragCoord;

out vec4 FragColor;

uniform float u_Time;

float hash21( vec2 p )
{
    p  = 50.0*fract( p*0.3183099 );
    return fract( p.x*p.y*(p.x+p.y) );
}

vec2 hash12( float n ) 
{ 
    return fract(sin(vec2(n,n+1.0))*vec2(43758.5453123,22578.1459123)); 
}

mat2 rotate(float a)
{
    float ca = cos(a);
    float sa = sin(a);
    return mat2(ca, -sa, sa, ca);
}

float dot2( in vec2 v ) { return dot(v,v); }
float dot2( in vec3 v ) { return dot(v,v); }

float smin( float a, float b, float k )
{
    float h = max(k-abs(a-b),0.0);
    return min(a, b) - h*h*0.25/k;
}

float smax( float a, float b, float k )
{
    float h = max(k-abs(a-b),0.0);
    return max(a, b) + h*h*0.25/k;
}

float opUnion( float d1, float d2 )
{
    return min(d1,d2);
}

float opSubtraction( float d1, float d2 )
{
    return max(-d1,d2);
}

float sdSphere( vec3 p, float r )
{
    return length(p)-r;
}

float sdCylinder( vec3 p, float h, float r )
{
  vec2 d = abs(vec2(length(p.xz),p.y)) - vec2(r,h);
  return min(max(d.x,d.y),0.0) + length(max(d,0.0));
}

float sdTorus( vec3 p, vec2 t )
{
  vec2 q = vec2(length(p.xz)-t.x,p.y);
  return length(q)-t.y;
}

float sdCone( vec3 p, vec2 c, float h )
{
  vec2 q = h*vec2(c.x/c.y,-1.0);
    
  vec2 w = vec2( length(p.xz), p.y );
  vec2 a = w - q*clamp( dot(w,q)/dot(q,q), 0.0, 1.0 );
  vec2 b = w - q*vec2( clamp( w.x/q.x, 0.0, 1.0 ), 1.0 );
  float k = sign( q.y );
  float d = min(dot( a, a ),dot(b, b));
  float s = max( k*(w.x*q.y-w.y*q.x),k*(w.y-q.y)  );
  return sqrt(d)*sign(s);
}

float sdBox( vec3 p, vec3 b )
{
  vec3 q = abs(p) - b;
  return length(max(q,0.0)) + min(max(q.x,max(q.y,q.z)),0.0);
}

float sdCappedCone( vec3 p, float h, float r1, float r2 )
{
  vec2 q = vec2( length(p.xz), p.y );
  vec2 k1 = vec2(r2,h);
  vec2 k2 = vec2(r2-r1,2.0*h);
  vec2 ca = vec2(q.x-min(q.x,(q.y<0.0)?r1:r2), abs(q.y)-h);
  vec2 cb = q - k1 + k2*clamp( dot(k1-q,k2)/dot2(k2), 0.0, 1.0 );
  float s = (cb.x<0.0 && ca.y<0.0) ? -1.0 : 1.0;
  return s*sqrt( min(dot2(ca),dot2(cb)) );
}

float fluteHead(vec3 p)
{
    vec3 q = p;
    float head = sdCylinder(p, 9.0*0.5, (1.35-p.y*0.03)*(-cos(p.y)*0.04 + 0.96));
    head = smin(head, sdTorus(p-vec3(0.0, -3.0, 0.0), vec2(1.25, 0.0)), 1.1);
    head = opUnion(head, sdTorus(p-vec3(0.0, -4.6, 0.0), vec2(1.25, 0.35)));
    head = opUnion(head, sdTorus(p-vec3(0.0, -5.0, 0.0), vec2(1.25, 0.18)));
    head = opUnion(head, sdTorus(p-vec3(0.0, -2.1, 0.0), vec2(1.35, 0.1)));
    head = opUnion(head, sdTorus(p-vec3(0.0, -1.7, 0.0), vec2(1.35, 0.1)));
    
    head = opUnion(head, sdTorus(p-vec3(0.0, 4.5, 0.0), vec2(1.35, 0.2)));
    head = opUnion(head, sdTorus(p-vec3(0.0, 4.2, 0.0), vec2(1.35, 0.13)));
    
    q.y = -p.y;
    head = opSubtraction(sdCone(q-vec3(1.9, 0.3, 0.0), vec2(2.0, 6.0), 4.0), head);
    head = opSubtraction(sdBox(p-vec3(1.5, 3.4, 0.0), vec3(1.0, 0.25, 0.9)), head);
    
    head = opUnion(head, sdCylinder(p-vec3(0.0, 5.7, 0.0), 9.0*0.17, 1.25));
    head = smin(head, 0.9*sdTorus(p-vec3(0.0, 6.2, 0.0), vec2(0.2, 1.1)), 1.0);
    
    head = opUnion(head, sdCappedCone(q-vec3(0.0, -8.4, 0.0), 9.0*0.14, 1.0, 1.35));
    head = opSubtraction(sdSphere(p-vec3(-1.65,9.6,0.0), 2.0), head);
    head = opSubtraction(sdBox(p-vec3(0.65, 12.0, 0.0), vec3(0.1, 7.0, 0.6)), head);
    
    
    return head;
}

float fluteBody(vec3 p)
{
    float body = sdCylinder(p, 9.0, 1.2);
    float hole1 = sdCylinder((p-vec3(1.0, 6.0, 0.0)).zxy, 2.0, 0.4);
    float hole2 = sdCylinder((p-vec3(1.0, 3.5, 0.0)).zxy, 2.0, 0.4);
    float hole3 = sdCylinder((p-vec3(1.0, 1.0, 0.0)).zxy, 2.0, 0.4);
    float hole4 = sdCylinder((p-vec3(1.0, -1.5, 0.0)).zxy, 2.0, 0.4);
    float hole5 = sdCylinder((p-vec3(1.0, -4.0, 0.0)).zxy, 2.0, 0.4);
    float hole6 = sdCylinder((p-vec3(1.0, -6.5, 0.10)).zxy, 2.0, 0.27);
    float hole7 = sdCylinder((p-vec3(1.0, -6.5, -0.40)).zxy, 2.0, 0.17);
    body = opSubtraction(hole1, body);
    body = opSubtraction(hole2, body);
    body = opSubtraction(hole3, body);
    body = opSubtraction(hole4, body);
    body = opSubtraction(hole5, body);
    body = opSubtraction(hole6, body);
    body = opSubtraction(hole7, body);
    return body;
}

float fluteTail(vec3 p)
{
    float tail = sdCylinder(p, 9.0*0.2, 1.25*(-cos(p.y)*0.13 + 0.87));
    tail = opUnion(tail, sdTorus(p-vec3(0.0, 1.8, 0.0), vec2(1.25, 0.2)));
    tail = opUnion(tail, sdTorus(p-vec3(0.0, 1.25, 0.0), vec2(1.25, 0.17)));
    tail = opUnion(tail, sdTorus(p-vec3(0.0, -1.25, 0.0), vec2(1.25, 0.17)));
    tail = opUnion(tail, sdTorus(p-vec3(0.0, -1.8, 0.0), vec2(1.25, 0.30)));
    
    tail = opSubtraction(sdSphere(p-vec3(1.8, 0.0, 0.0), 1.2), tail);
    
    float hole1 = sdCylinder((p-vec3(2.3, 0.0, 0.15)).zxy, 2.0, 0.27);
    float hole2 = sdCylinder((p-vec3(2.3, -0.1, -0.32)).zxy, 2.0, 0.13);
    tail = opSubtraction(hole1, tail);
    tail = opSubtraction(hole2, tail);
    
    float bot = 0.9*sdCylinder(p-vec3(0.0, -3.5, 0.0), 9.0*0.2, (1.15-p.y*0.03)*(-cos(p.y-3.5)*0.15 + 0.85));
    bot = opUnion(bot, sdTorus(p-vec3(0.0, -2.4, 0.0), vec2(0.9, 0.1)));
    bot = opUnion(bot, sdTorus(p-vec3(0.0, -4.8, 0.0), vec2(1.3, 0.15)));
    bot = opUnion(bot, sdTorus(p-vec3(0.0, -5.2, 0.0), vec2(1.3, 0.30)));
    
    tail = opUnion(tail, bot);
    
    return tail;
}

float flute(vec3 p)
{
    vec3 q = p;
    float head = fluteHead(p-vec3(0.0, 12.5, 0.0));
    float body = fluteBody(p);
    q.xz = q.xz*rotate(-0.5);
    float tail = fluteTail(q-vec3(0.0, -11.0, 0.0));
    
    head = opUnion(head, body);
    head = opUnion(head, tail);
    
    float rod =sdCylinder(p-vec3(0.0, -5.9, 0.0), 15.0, 0.6);
    head = opSubtraction(rod, head);
    return head;
}


vec2 map( in vec3 p ) 
{
    vec2 res = vec2(100.0);

    vec3 q = p;
    q.yz = q.yz*rotate(-0.6);
    q.xy = q.xy*rotate(0.3);
    q.xz = q.xz*rotate(2.2);
    float f = flute(q);
    if (f < res.x) res = vec2(f, 2.0);
        
    // floor
    float m = p.y + 20.0;
    if( m < res.x ) res = vec2( m, 1.0 );
    
    return res;
}

vec3 calcNormal( in vec3 p, in float t )
{
    float e = 0.001*t;

    vec2 h = vec2(1.0,-1.0)*0.5773;
    return normalize( h.xyy*map( p + h.xyy*e ).x + 
					  h.yyx*map( p + h.yyx*e ).x + 
					  h.yxy*map( p + h.yxy*e ).x + 
					  h.xxx*map( p + h.xxx*e ).x );   
}

float calcOcclusion( in vec3 pos, in vec3 nor, float time )
{
	float occ = 0.0;
    float sca = 1.0;
    for( int i=0; i<5; i++ )
    {
        float h = 0.01 + 0.11*float(i)/4.0;
        vec3 opos = pos + h*nor;
        float d = map( opos ).x;
        occ += (h-d)*sca;
        sca *= 0.95;
    }
    return clamp( 1.0 - 2.0*occ, 0.0, 1.0 );
}

vec2 rayMarch( in vec3 ro, in vec3 rd )
{
    float ma = -1.0;
    vec2 res = vec2(-1.0);
    float tmax = 1000.0;   
    float t = 0.01;
    
    for( int i=0; i<256; i++ )
    {
        vec3 pos = ro + t*rd;
        vec2 h = map( pos );
        ma = h.y;
        if( h.x<(0.0001*t) || t>tmax ) break;
        t += h.x;
    }

    if( t<tmax )
    {
    	res = vec2(t, ma);
    }

    return res;
}

mat3 setCamera( in vec3 ro, in vec3 ta, float cr )
{
	vec3 cw = normalize(ta-ro);
	vec3 cp = vec3(sin(cr), cos(cr),0.0);
	vec3 cu = normalize( cross(cw,cp) );
	vec3 cv = normalize( cross(cu,cw) );
    return mat3( cu, cv, cw );
}


void main()
{
    // Normalized pixel coordinates (from 0 to 1)
    vec2 uv = FragCoord;
     
    float fl = 4.0;
    float an = u_Time*0.4;
    vec3 ta = vec3(0.0, 0.0, 0.0);
    vec3 ro = vec3(-90.0*cos(an), 0.0, -90.0*sin(an));
    mat3 ca = setCamera( ro, ta, 0.0 );
    vec3 rd = ca * normalize( vec3(uv,fl));
    
    vec2 res = rayMarch( ro, rd );

    vec3 col = vec3(0.5, 0.85, 1.0) - max(rd.y,0.0)*0.5;
    
    if (res.x < 1000.0 && res.x > 0.001) 
    {
        float t = res.x;
        vec3 pos = ro + t*rd;
        vec3 nor = calcNormal( pos, t );
        vec3 ref = reflect( rd, nor );
        
        float ks = 2.0;
        vec3 sun_lig = normalize( vec3(-3.6, 3.35, 0.5) );
        float sun_dif = clamp(dot( nor, sun_lig ), 0.0, 1.0 );
        vec3 sun_hal = normalize( sun_lig-rd );
        float sun_spe = ks*pow(clamp(dot(nor,sun_hal),0.0,1.0),256.0)*sun_dif*(0.04+0.96*pow(clamp(1.0+dot(sun_hal,rd),0.0,1.0),5.0));
        float sun_sha = step(rayMarch( pos+0.001*nor, sun_lig ).x,0.0);
        float sky_dif = sqrt(clamp( 0.5+0.5*nor.y, 0.0, 1.0 ));
        float bou_dif = sqrt(clamp( 0.1-0.9*nor.y, 0.0, 1.0 ));
        float occ = calcOcclusion(pos, nor, t);
        vec3 mat;
        
        
        if (res.y < 1.5 && res.y > 0.5)
        {
            mat = vec3(0.35);
        } else if (res.y > 1.5 && res.y < 2.5) {
            mat = vec3(0.8, 0.5, 0.3);
        }
        
        vec3 lin = vec3(0.0);
        lin += mat*sun_dif*vec3(1.20,1.10,0.80)*sun_sha*occ;
        lin += 0.1*sky_dif*vec3(0.50,0.70,1.00);
		lin += sun_spe*vec3(8.10,6.00,4.20)*sun_sha;
        lin += bou_dif*vec3(0.60,0.40,0.30);
        
        col = vec3(lin);
        
    }
    
    col = pow( col, vec3(0.4545) );
    

    // Output to screen
    FragColor = vec4(col,1.0);
}