#version 330

layout(location=0) out vec4 FragColor;

in vec2 v_Tex;

const float c_PI = 3.141592;

uniform float u_Time;
uniform sampler2D u_RGBTex; //0
uniform sampler2D u_CurrNumTex;
uniform sampler2D u_NumsTex;
uniform int u_InputNum;

uniform vec4 u_DropInfo[1000]; //vec4(x, y, sT, lT)

void Simple()
{
	if(v_Tex.x + v_Tex.y > 0.5)
	{
		FragColor = vec4(0);
	}
	else
	{
		FragColor = vec4(v_Tex, 0, 1);
	}
}

void LinePattern()
{
	float lineCountH = 10;
	float lineCountV = 2;
	float lineWidth = 1;
	lineCountH = lineCountH / 2;
	lineCountV = lineCountV / 2;
	lineWidth = 50 / lineWidth;
	float per = -0.5*c_PI;
	float grey = pow(
			abs(sin((v_Tex.y*2*c_PI+per)*lineCountH))
					, lineWidth);

	float grey1= pow(
			abs(sin((v_Tex.x*2*c_PI+per)*lineCountV))
					, lineWidth);

	FragColor = vec4(grey+grey1);
}

void Circle()
{	
	vec2 center = vec2(0.5, 0.7);
	vec2 currPos = v_Tex.xy;
	float d = distance(center, currPos);
	float lineWidth = 0.01;
	float radius = 0.1;

	if(d > radius-lineWidth && d < radius)
	{
		FragColor = vec4(1);
	}
	else
	{
		FragColor = vec4(0);
	}
}


void CircleSin()
{	
	vec2 center = vec2(0.5, 0.5);
	vec2 currPos = v_Tex.xy;
	float d = distance(center, currPos);
	float value = abs(sin(d * c_PI * 16 - u_Time*20));
	FragColor = vec4(pow(value, 16));
}

void FractalJuliaAnimated()
{
    // UV(0~1)를 복소평면 좌표(-1~1 근처)로 변환
    vec2 uv = v_Tex * 2.0 - 1.0;

    // 화면비 보정이 필요하면 x를 더 넓히거나 줄일 수 있음
    uv.x *= 1.6;

    // 시간에 따라 변화하는 Julia 상수
    vec2 c = vec2(
        0.35 * cos(u_Time * 0.7),
        0.35 * sin(u_Time * 1.1)
    );

    vec2 z = uv;

    const int maxIter = 96;
    int i;

    for (i = 0; i < maxIter; ++i)
    {
        // z = z^2 + c
        float x = z.x * z.x - z.y * z.y + c.x;
        float y = 2.0 * z.x * z.y + c.y;
        z = vec2(x, y);

        if (dot(z, z) > 4.0)
            break;
    }

    // 반복 횟수 기반 밝기
    float t = float(i) / float(maxIter);

    // 좀 더 화려하게 보이도록 비선형 매핑
    float r = 0.5 + 0.5 * cos(6.2831 * (t + u_Time * 0.08));
    float g = pow(t, 0.7);
    float b = 0.5 + 0.5 * sin(6.2831 * (t * 0.8 + u_Time * 0.12));

    // 내부는 어둡게, 외부는 컬러가 돌도록
    if (i == maxIter)
    {
        FragColor = vec4(0.02, 0.02, 0.03, 1.0);
    }
    else
    {
        float glow = 1.0 - t;
        FragColor = vec4(
            r * glow,
            g * (0.6 + 0.4 * glow),
            b,
            1.0
        );
    }
}

void RainDrop()
{	
	float accum = 0;
	//Rain Drop
	for(int i=0; i<200; i++)
	{
		float lTime = u_DropInfo[i].w;
		float sTime = u_DropInfo[i].z;
		float newTime = u_Time - sTime;
		if(newTime > 0)
		{
			newTime = fract(newTime/lTime); //0~1
			float oneMinus = 1-newTime; //1~0
			float t = newTime * lTime;
			vec2 center = u_DropInfo[i].xy;
			vec2 currPos = v_Tex.xy;
			float range = t/10;
			float d = distance(center, currPos);
			float fade = 30 * clamp(range-d, 0, 1);
			float value = pow(abs(sin(d * 4 * c_PI * 10 - t*15)), 16);
			accum += value * fade * oneMinus;
		}
		else
		{
		}
	}
	FragColor = vec4(accum);
}

void Flag()
{
    float amp = 0.5;
    float speed = 15;
    float sinInput = v_Tex.x * c_PI * 2 - u_Time*speed;
    float sinValue = v_Tex.x*amp*(((sin(sinInput) + 1)/2)-0.5)+0.5; //0~1
    
    float fWidth = 0.0;
    float width = 0.5 * mix(1, fWidth, v_Tex.x);
    float grey = 0;

    if(v_Tex.y < sinValue + width/2 && v_Tex.y > sinValue - width/2)
    {
        grey = 1;
    }
    else
    {
        grey = 0;
        discard;
    }

    FragColor = vec4(grey);
}

void Flame()
{
    float amp = 0.5;
    float speed = 15;
    float newY = 1-v_Tex.y;
    float sinInput = newY * c_PI * 2 - u_Time*speed;
    float sinValue = newY*amp*(((sin(sinInput) + 1)/2)-0.5)+0.5; //0~1
    
    float fWidth = 0.0;
    float width = 0.5 * mix(fWidth, 1, newY);
    float grey = 0;

    if(v_Tex.x < sinValue + width/2 && v_Tex.x > sinValue - width/2)
    {
        grey = 1;
    }
    else
    {
        grey = 0;
        discard;
    }

    FragColor = vec4(grey);
}

void TextureSampling()
{
    vec4 c0;
    vec4 c1;
    vec4 c2;
    vec4 c3;
    vec4 c4;

    float offsetX = 0.01;
    
    c0 = texture(u_RGBTex, vec2(v_Tex.x - offsetX * 2, v_Tex.y));
    c1 = texture(u_RGBTex, vec2(v_Tex.x - offsetX * 1, v_Tex.y));
    c2 = texture(u_RGBTex, vec2(v_Tex.x - offsetX * 0, v_Tex.y));
    c3 = texture(u_RGBTex, vec2(v_Tex.x + offsetX * 1, v_Tex.y));
    c4 = texture(u_RGBTex, vec2(v_Tex.x + offsetX * 2, v_Tex.y));

    vec4 sum = c0 + c1 + c2 + c3 + c4;
    sum = sum/5;

    FragColor = sum;
}

void TextureQ1()
{
    float tx = v_Tex.x;
    float ty = 1-2*abs(v_Tex.y - 0.5);
    vec2 newTex = vec2(tx, ty);
    FragColor = texture(u_RGBTex, newTex);
}

void TextureQ2()
{
    float tx = fract(v_Tex.x * 3);
    float ty = v_Tex.y / 3;

    float offsetX = 0;
    float offsetY = (2 - floor(v_Tex.x * 3))/3;

    vec2 newTex = vec2(tx + offsetX, ty + offsetY);
    FragColor = texture(u_RGBTex, newTex);
}

void TextureQ3()
{
    float tx = fract(v_Tex.x * 3);
    float ty = v_Tex.y / 3;

    float offsetX = 0;
    float offsetY = floor(v_Tex.x * 3)/3;

    vec2 newTex = vec2(tx + offsetX, ty + offsetY);
    FragColor = texture(u_RGBTex, newTex);
}

void TextureQ4()
{
    float resolX = 5;
    float resolY = 5;
    float shear = 0.5 * u_Time;

    float offsetX = fract(ceil(v_Tex.y*resolY)*shear); //offset
    float offsetY = 0;

    float tx = fract(v_Tex.x * resolX + offsetX); //range
    float ty = fract(v_Tex.y * resolY + offsetY);


    vec2 newTex = vec2(tx, ty);
    FragColor = texture(u_RGBTex, newTex);
}

void Num()
{
    float tx = v_Tex.x;
    float ty = v_Tex.y;

    float offsetX = 0;
    float offsetY = 0;

    vec2 newTex = vec2(tx + offsetX, ty + offsetY);
    FragColor = texture(u_CurrNumTex, newTex);
}

void Nums()
{
    float index = float(u_InputNum);

    float tx = v_Tex.x / 5;
    float ty = v_Tex.y / 2;

    float offsetX = fract(index/5.0);
    float offsetY = floor(index/5.0)/2.0;

    vec2 newTex = vec2(tx + offsetX, ty + offsetY);
    FragColor = texture(u_NumsTex, newTex);
}

void main()
{
	//TextureSampling();
    Nums();
}



/*void FS_01_Q6()
{
    float tx = fract(v_Tex.x*3);
    float ty = v_Tex.y/3;
    float offsetX = 0;
    float offsetY = abs(floor(v_Tex.x*3)/3 - 2);
    FragColor = texture(u_RGBTex, vec2(tx+offsetX, ty+offsetY));
}

void FS_01_Q7()
{
    float tx = v_Tex.x;
    float ty = fract(v_Tex.y*3)/3;
    float offsetX = 0;
    float offsetY = fract(ceil(v_Tex.y*3.0)/3.0);
    FragColor = texture(u_RGBTex, vec2(tx+offsetX, ty+offsetY));
}

void FS_01_Q8()
{
    float tx = v_Tex.x/5.0;
    float ty = v_Tex.y/2.0;
    float offsetX = 2.0/5.0;
    float offsetY = 1.0/2.0;
    FragColor = texture(u_NumsTex, vec2(tx+offsetX, ty+offsetY));
    if(FragColor.a == 0) discard;
}

void FS_01_Q9()
{
    float tx = 2*v_Tex.x/5.0;
    float ty = v_Tex.y/2.0;
    float offsetX = 2.0/5.0;
    float offsetY = 0.0/2.0;
    FragColor = texture(u_NumsTex, vec2(tx+offsetX, ty+offsetY));
    if(FragColor.a == 0) discard;
}

void FS_01_Q10()
{
    float index = float(8);
    float tx = v_Tex.x / 5;
    float ty = v_Tex.y / 2;
    float offsetX = fract(index/5.0);
    float offsetY = floor(index/5.0)/2.0;
    FragColor = texture(u_NumsTex, vec2(tx + offsetX, ty + offsetY));
    if(FragColor.a == 0) discard;
}

void FS_03_Q6()
{
    float tx = v_Tex.x;
    float ty = fract(v_Tex.y*3)/3;
    float offsetX = 0;
    float offsetY = fract(floor(v_Tex.y*3)*2/3);
    FragColor = texture(u_RGBTex, vec2(tx+offsetX, ty+offsetY));
}

void FS_03_Q7()
{
    float tx = fract(v_Tex.x*3);
    float ty = v_Tex.y/3;
    float offsetX = 0;
    float offsetY = fract(ceil(v_Tex.x*3.0)/3.0);
    FragColor = texture(u_RGBTex, vec2(tx+offsetX, ty+offsetY));
}
void FS_03_Q8()
{
    float tx = v_Tex.x/5.0;
    float ty = v_Tex.y/2.0;
    float offsetX = 4.0/5.0;
    float offsetY = 0.0/2.0;
    FragColor = texture(u_NumsTex, vec2(tx+offsetX, ty+offsetY));
    if(FragColor.a == 0) discard;
}

void FS_03_Q9()
{
    float tx = 2*v_Tex.x/5.0;
    float ty = v_Tex.y/2.0;
    float offsetX = 2.0/5.0;
    float offsetY = 1.0/2.0;
    FragColor = texture(u_NumsTex, vec2(tx+offsetX, ty+offsetY));
    if(FragColor.a == 0) discard;
}

void FS_03_Q10()
{
    float index = float(3);
    float tx = v_Tex.x / 5;
    float ty = v_Tex.y / 2;
    float offsetX = fract(index/5.0);
    float offsetY = floor(index/5.0)/2.0;
    FragColor = texture(u_NumsTex, vec2(tx + offsetX, ty + offsetY));
    if(FragColor.a == 0) discard;
}*/
