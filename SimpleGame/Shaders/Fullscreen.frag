#version 330 core

out vec4 FragColor;

const float c_PI = 3.141592;

in vec2 v_Tex; // 역할이 뭘까?

uniform float u_Time;

uniform vec4 u_Points[500];

const vec4 c_Points[2] = vec4[2](
    vec4(0.5, 0.5, 0, 0.5),
    vec4(0.5, 0.7, 0.5, 1)
);

void Simple()
{
    if (v_Tex.x < 0.5)
    {
        FragColor = vec4(0); // 검정색도 출력한거임 출력을 안하는 명령어는 따로 있음
    }
    else
    {
        FragColor = vec4(1);
    }
     
     // FragColor = vec4(sin(v_Tex.x*10*3.141592));
}

void Line()
{
    //FragColor = vec4(v_Tex.xy, 0, 1);
    //FragColor = vec4(sin(v_Tex.x * 2 * c_PI)); 
    //FragColor = vec4(abs(sin(v_Tex.x * 2 * c_PI))); // 2줄
    //FragColor = vec4(abs(sin(v_Tex.x * 10 * c_PI))); // 10이여서 10줄

    //float period = v_Tex.y * 2 * c_PI * 5; // x대신 y를 써서 가로줄로 바꿈
    //float value = pow(abs(sin(period)), 16); // 선이 얇아짐
    //FragColor = vec4(value);

    /*
    float periodX = v_Tex.x * 2 * c_PI * 5; // x대신 y를 써서 가로줄로 바꿈
    float periodY = v_Tex.y * 2 * c_PI * 5; // x대신 y를 써서 가로줄로 바꿈

    float valueX = pow(abs(sin(periodX)), 16); // 선이 얇아짐
    float valueY = pow(abs(sin(periodY)), 16); // 선이 얇아짐

    //FragColor = vec4(valueX + valueY); 
    FragColor = vec4(max(valueX , valueY));  // 더 깔끔해짐 

    */
    // 교수님 버전
    float trans = c_PI /2;
    float periodX = (v_Tex.x * 2 * c_PI - trans)* 5; // x대신 y를 써서 가로줄로 바꿈
    float periodY = (v_Tex.y * 2 * c_PI - trans)* 5; // x대신 y를 써서 가로줄로 바꿈

    float valueX = pow(abs(sin(periodX)), 16); // 선이 얇아짐
    float valueY = pow(abs(sin(periodY)), 16); // 선이 얇아짐
    FragColor = vec4(max(valueX , valueY));  // 더 깔끔해짐 

    /*
    //줄들이 모서리에 딱 맞게 출력됨 sin->cos로 변환
    float periodX = v_Tex.x * 2 * c_PI * 5; // x대신 y를 써서 가로줄로 바꿈
    float periodY = v_Tex.y * 2 * c_PI * 5; // x대신 y를 써서 가로줄로 바꿈

    float valueX = pow(abs(cos(periodX)), 16); // 선이 얇아짐
    float valueY = pow(abs(cos(periodY)), 16); // 선이 얇아짐

    FragColor = vec4(max(valueX , valueY));  // 더 깔끔해짐 

    // procedural을 texture 할 수 있다. texture를 저장하는게 아니라 실시간으로 만들 수 있다.
    // 확대 축소해도 안 깨짐 / sampling에 비해 성능을 떨어질 수 있지만 더 깔끔함
    */
}

// 시험에 나옴 =========================================================================================================
// 사선으로 만들기 해보기 abs 없애고 그 후작업은 모름
void a()
{
    float trans = c_PI / 2.0;

    float periodX = (v_Tex.x * 2.0 * c_PI - trans) * 5.0;
    float periodY = (v_Tex.y * 2.0 * c_PI - trans) * 5.0;

    float valueX = pow(abs(sin(periodX + periodY)), 16.0);
    float valueY = pow(abs(sin(periodX - periodY)), 16.0);

    FragColor = vec4(valueX, valueY, 0.0, 1.0);
}

void Circle()
{
    vec2 center = vec2(0.5, 0.5);
    vec2 pos = v_Tex - center;

    float period = length(pos);
    float radius = 0.25;
    float thickness = 0.01;

    float value = 1.0 - step(thickness, abs(period - radius));

    FragColor = vec4(value, value, value, 1.0);
}

// 교수님 코드
void Circle2()
{
    vec2 center = vec2(0.5,0.5);
    vec2 currPos = v_Tex;
    
    float d = distance(center, currPos);
    float width = 0.01;
    float radius = 0.5;

    if(d > radius -width && d<radius)
        FragColor = vec4(1); // d를 넣으면?
    else
        FragColor = vec4(0);
}

// 동일한 센터를 가지는 동심원 여러개 
void Circles()
{
    vec2 center = vec2(0.5, 0.5);
    vec2 currPos = v_Tex;

    float d = distance(center, currPos);

    float value = cos(d * 100);
    FragColor = vec4(value);
}
// vs에 비해 fs는 브랜치를 쓰면 성능이 매우 떨어지므로 지양해야함, if문을 쓰지말라는거? 브랜치가 뭐지?

void CircleMany()
{
    vec2 center = vec2(0.5, 0.5);
    vec2 currPos = v_Tex;

    float d = distance(center, currPos);
    float period = d * 50.0;

    float line = pow(abs(sin(period)), 8.0);

    float valueR = line * (0.5 + 0.5 * sin(period));
    float valueG = line * (0.5 + 0.5 * sin(period + 2.0 * c_PI / 3.0));
    float valueB = line * (0.5 + 0.5 * sin(period + 4.0 * c_PI / 3.0));

    FragColor = vec4(valueR, valueG, valueB, 1.0);
}

// 움직이는 프랙탈 무늬 만들기
void Fractal()
{
    vec2 p = v_Tex * 2 - 1;
    vec2 z = p;

    float value = 0;

    for (int i = 0; i < 5; i++)
    {
        z = abs(z) / dot(z, z) - 0.7;
        value += exp(-length(z) * 3);
        z.x += sin(u_Time) * 0.1;
        z.y += cos(u_Time) * 0.1;
    }

    float r = 0.5 + 0.5 * sin(value * 6 + u_Time);
    float g = 0.5 + 0.5 * sin(value * 6 + 2 * c_PI / 3 + u_Time);
    float b = 0.5 + 0.5 * sin(value * 6 + 4 * c_PI / 3 + u_Time);

    FragColor = vec4(r, g, b, 1);
}

// 교수님꺼
void t()
{
    vec2 center = vec2(0.5,0.5);
    vec2 currPos = v_Tex;
    
    float count = 5;
    float d = distance(center, currPos);

    float grey = pow(abs(sin(d * 4 * c_PI  * count - u_Time * 3)), 
    24);
    FragColor = vec4(grey);
}

void RainDrop()
{
    vec2 center = vec2(0.5,0.5);
    vec2 currPos = v_Tex;
    
    float count = 5;
    float range = 0.5;
    float d = distance(center, currPos);

    float fade = 2 * clamp(0.5 - d, 0, 1);  // (1/range) * clamp(0.5 - d, 0, 1); // 주석달은 버전은 작은 범위 내에서 퍼짐
    float grey = pow(abs(sin(d * 4 * c_PI  * count- u_Time * 3)), 
    24);

    FragColor = vec4(grey * fade);
}

/*
    float fade = 1.0 - d * 1.4;  
    fade = clamp(fade, 0.0, 1.0);

    grey *= fade;

    FragColor = vec4(vec3(grey), 1.0);
*/

 void RainDrop2()
{
    float newTime = fract(u_Time);
    vec2 center = vec2(0.5,0.5);
    vec2 currPos = v_Tex;    
    float count = 5;
    float range = newTime / 5;

    float d = distance(center, currPos);
    float fade = (1 * range) * clamp(0.5 - d, 0, 1);

    float grey = pow(abs(sin(d * 4 * c_PI  * count- u_Time * 3)), 
    24);

    FragColor = vec4(grey * fade);
}

void RainDrop3()
{
    float accum = 0;
    for (int i = 0; i <2; i++)
    {
        float sTime = c_Points[i].z;
        float LTime = c_Points[i].w;
        float newTime = u_Time - sTime;
        if (newTime > 0)
        {
            float t = fract(newTime/LTime);
            float oneMinus = 1 - t;
            t = t*LTime;
            vec2 center = c_Points[i].xy;
            vec2 currPos = v_Tex;    
            float count = 5;
            float range = t / 5;

            float d = distance(center, currPos);
            float fade = (1/range) * clamp(range - d ,0,1);
            float grey = pow(abs(sin(d * 4 * c_PI * count - t * 8)), 
            4);
            accum += grey * fade * oneMinus;
        }    
    }
    FragColor = vec4(accum);
}

void RainDrop4()
{
    float accum = 0;
    for (int i = 0; i < 500; i++)
    {
        float sTime = u_Points[i].z;
        float LTime = u_Points[i].w;
        float newTime = u_Time - sTime;
        if (newTime > 0)
        {
            float t = fract(newTime/LTime);
            float oneMinus = 1 - t;
            t = t*LTime;
            vec2 center = u_Points[i].xy;
            vec2 currPos = v_Tex;    
            float count = 5;
            float range = t / 5;

            float d = distance(center, currPos);
            float fade = (1/range) * clamp(range - d ,0,1);
            float grey = pow(abs(sin(d * 4 * c_PI * count - t * 8)), 
            4);
            accum += grey * fade * oneMinus;
        }    
    }
    FragColor = vec4(accum);
}


void RainDrop5()
{
    float accum = 0;

    for (int i = 0; i < 500; i++)
    {
        float sTime = u_Points[i].z;
        float lTime = u_Points[i].w;
        float newTime = u_Time - sTime;
        if (newTime > 0)
        {
            float t = fract(newTime / lTime); // 0~1
            float oneMinus = 1 - t; // 1~0
            t = t * lTime;

            vec2 center = u_Points[i].xy;
            vec2 currPos = v_Tex;
            float count = 5;
            float range = t / 5;

            float d = distance(center, currPos);
            float fade = 10 * clamp(range - d, 0, 1);
            float grey = pow(
                sin(d * 4 * c_PI * count - t * 10),
                4);

            accum += grey * fade * oneMinus;
        }
    }

    FragColor = vec4(accum);
}

void RainDrop6()
{
    float accum = 0;
    for (int i = 0; i < 500; i++)
    {
        float sTime = u_Points[i].z;
        float LTime = u_Points[i].w;
        float newTime = u_Time - sTime;
        if (newTime > 0)
        {
            float t = fract(newTime / LTime);
            float oneMinus = 1 - t;
            t = t * LTime;
            vec2 center = u_Points[i].xy;
            vec2 currPos = v_Tex;
            float count = 5;
            float range = t / 5;

            float d = distance(center, currPos);
            float fade = (1 / range) * clamp(range - d, 0, 1);
            float grey = pow(abs(sin(d * 4 * c_PI * count - t * 8)),
            4);
            accum += grey * fade * oneMinus;
        }
    }
    FragColor = vec4(accum);
}

void RainDrop02()
{
    float newTime = fract(u_Time);
    vec2 center = vec2(0.5,0.5);
    vec2 currPos = v_Tex;
    float count = 5;
    float range = newTime / 5;

    float d = distance(center, currPos);
    float fade = clamp(range - d, 0, 1);
    float grey = pow(abs(sin(d * 4 * c_PI * count - u_Time * 0.5)),
    12);

    FragColor = vec4(grey * fade);
}

void main()
{
    RainDrop02();
}
