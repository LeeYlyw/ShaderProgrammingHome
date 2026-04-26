#version 330 core

uniform float u_Time;

// 버텍스 속성
layout(location = 0) in vec3 a_Position;    // 파티클의 초기 위치
layout(location = 1) in float a_Mass;       // 질량 값 (현재 이 함수에서는 직접 사용하지 않음)
layout(location = 2) in vec2 a_Vel;         // 기본 속도
layout(location = 3) in float a_Rv;         // 랜덤값 1 : 원형 배치용 각도 생성에 사용
layout(location = 4) in float a_Rv1;        // 랜덤값 2 : 각 파티클의 시간 오프셋
layout(location = 5) in float a_Rv2;        // 랜덤값 3 : 링 선택, 크기, 흔들림 등에 사용

out float v_Grey;
out float v_T; // + 색확장

const float c_PI = 3.141592;
const vec2 c_G = vec2(0.0, -9.8);           // 아래 방향 중력

// 0~1 범위의 의사 난수 생성 함수
float psuedoRandom(float x)
{
    return fract(sin(x) * 43758.5453);
}

void MagicCircleBurst()
{
    float cycle = 2.5;                                  // 전체 이펙트 한 주기
    float localTime = mod(u_Time + a_Rv1 * 0.6, cycle); // 파티클마다 약간 다른 시간으로 반복

    float angle = a_Rv * 2.0 * c_PI;                   // 각 파티클의 기본 각도
    float spinTime = min(localTime, 1.2);              // 회전 단계는 최대 1.2초까지만 진행
    float rotateAngle = spinTime * 2.2;                // 회전 각도 증가량

    // 마법진 중심 자체를 시간에 따라 살짝 움직이게 함
    float centerX = sin(u_Time * 0.7) * 0.3;
    float centerY = cos(u_Time * 0.5) * 0.15;

    vec4 newPos;

    // 파티클이 어느 링에 속할지 결정
    // 0.5 미만이면 안쪽 원형 링, 아니면 바깥 별 모양 링
    float ringSelect = psuedoRandom(a_Rv2 + 7.3);

    float orbitX;
    float orbitY;
    float finalAngle;

    // 안쪽 원형 링
    if (ringSelect < 0.5)
    {
        // 반지름을 조금 흔들어서 살아있는 느낌 추가
        float radius = 0.22 + 0.015 * sin(u_Time * 4.0 + a_Rv2 * 10.0);
        finalAngle = angle + rotateAngle;              // 정방향 회전

        orbitX = cos(finalAngle) * radius;
        orbitY = sin(finalAngle) * radius;
    }
    // 바깥 별 모양 링
    else
    {
        finalAngle = angle - rotateAngle * 1.3;        // 반대 방향으로 더 빠르게 회전

        float baseRadius = 0.62;                       // 기본 반지름
        float starRadius = baseRadius + 0.12 * cos(finalAngle * 5.0);
        // cos(finalAngle * 5.0)로 별의 뾰족한 형태 생성

        orbitX = cos(finalAngle) * starRadius;
        orbitY = sin(finalAngle) * starRadius;
    }

    // 1단계: 회전하면서 마법진 형태를 유지하는 구간
    if (localTime < 1.2)
    {
        newPos.x = centerX + orbitX;
        newPos.y = centerY + orbitY;
        newPos.z = 0.0;
        newPos.w = 1.0;

        gl_Position = newPos;

        // 안쪽 원형 링은 더 크고 밝게 보이도록 점 크기 설정
        if (ringSelect < 0.5)
        {
            float pulseSize = 1.8 + sin(u_Time * 8.0 + a_Rv2 * 6.0) * 0.4;
            gl_PointSize = pulseSize + psuedoRandom(a_Rv2) * 0.5;
        }
        // 바깥 별 링은 더 작고 날카롭게 보이도록 설정
        else
        {
            float pulseSize = 0.9 + sin(u_Time * 7.0 + a_Rv2 * 5.0) * 0.2;
            gl_PointSize = pulseSize + psuedoRandom(a_Rv2) * 0.3;
        }
    }
    // 2단계: 마법진이 터지면서 파티클이 바깥으로 퍼져나가는 구간
    else
    {
        float t = localTime - 1.2;                     // 폭발 시작 후 경과 시간
        float tt = t * t;

        // 현재 링 위치에서 바깥 방향으로 나가는 단위 벡터
        vec2 dir = normalize(vec2(orbitX, orbitY));

        // 폭발 속도와 약간의 랜덤 흔들림
        float burstSpeed = 0.18 + psuedoRandom(a_Rv2 + 3.1) * 0.22;
        float jitterX = (psuedoRandom(a_Rv2 + 1.7) - 0.5) * 0.08;
        float jitterY = (psuedoRandom(a_Rv2 + 5.3) - 0.5) * 0.08;

        // 바깥 방향 + 랜덤 흔들림 + 기존 속도 일부 반영
        float vx = dir.x * burstSpeed + jitterX + a_Vel.x * 0.2;
        float vy = dir.y * burstSpeed + jitterY + a_Vel.y * 0.2;

        // 등가속도 운동 공식으로 위치 계산
        newPos.x = centerX + orbitX + vx * t + 0.5 * c_G.x * tt;
        newPos.y = centerY + orbitY + vy * t + 0.5 * c_G.y * tt;
        newPos.z = 0.0;
        newPos.w = 1.0;

        gl_Position = newPos;

        // 폭발 후에는 작은 점들로 흩어지게 표현
        float pointSize = 1.0 + psuedoRandom(a_Rv2) * 1.5;
        gl_PointSize = pointSize;
    }
}

void CircleFalling()
{
    float newTime = u_Time * 0.3 - a_Rv1 * 0.3;

    if (newTime > 0.0)
    {
        float lifeTime = a_Rv2 + 0.5; // ** 시험 float lifeTime = () 조건 놓고 
        float t = mod(newTime, lifeTime);
        float scale = psuedoRandom(a_Rv1) * (lifeTime - t) / lifeTime; //  시험 float scale = psuedoRandom(a_Rv1) * ()를 하면 라이프타임이 0이 될까?

        float tt = t * t;

        float vx = a_Vel.x / 10.0;
        float vy = a_Vel.y / 10.0;

        // 원 둘레 위 시작 위치
        float initPosX = a_Position.x * scale + sin(a_Rv * 2.0 * c_PI) * 0.9;
        float initPosY = a_Position.y * scale + cos(a_Rv * 2.0 * c_PI) * 0.9;

        vec4 newPos;
        newPos.x = initPosX + vx * t + 0.5 * c_G.x * tt;
        newPos.y = initPosY + vy * t + 0.5 * c_G.y * tt;
        newPos.z = 0.0;
        newPos.w = 1.0;

        gl_Position = newPos;
        gl_PointSize = 3.0;
    }
    else
    {
        gl_Position = vec4(-10.0, -10.0, 0.0, 1.0);
        gl_PointSize = 0.0;
    }
}

void Thrust()
{

    float t = mod(u_Time * 0.1, 2.0); // ** 시험 문제 폭을 바꿔서 여러개가 움직이는것처럼 보이게(선처럼 보이게)

    vec4 newPosition;
    newPosition.x = a_Position.x - 1.0 + t;
    newPosition.y = a_Position.y + 0.5 * sin(t * 2.0 * c_PI);
    newPosition.z = a_Position.z;
    newPosition.w = 1.0;

    gl_Position = newPosition;
    gl_PointSize = 5.0;
}

void Thrust1()
{
    float amp = a_Rv;
    float t = mod(u_Time * 0.1, 1.0); // ** 시험 문제 : 폭을 바꿔서 여러개가 움직이는것처럼 보이게(선처럼 보이게)
                                      // ** 시험 문제 : 이제 스타트 타임을 적용해보기 

    vec4 newPosition;
    newPosition.x = a_Position.x - 1.0 + t;
    newPosition.y = a_Position.y + amp * sin(t * 2.0 * c_PI);
    newPosition.z = a_Position.z;
    newPosition.w = 1.0;

    gl_Position = newPosition;
    gl_PointSize = 5.0;
}

void Thrust2()
{
    float amp = a_Rv;

    float startTime = a_Rv1 * 2.0;          // 파티클마다 다른 시작 지연 시간
    float newTime = u_Time - startTime;

    if (newTime > 0.0)
    {
        float t = mod(newTime * 0.1, 2.0);

        vec4 newPosition;
        newPosition.x = a_Position.x - 1.0 + t;
        newPosition.y = a_Position.y + amp * sin(t * 2.0 * c_PI);
        newPosition.z = a_Position.z;
        newPosition.w = 1.0;

        gl_Position = newPosition;
        gl_PointSize = 5.0;
    }
    else
    {
        gl_Position = vec4(-10.0, -10.0, 0.0, 1.0);
        gl_PointSize = 0.0;
    }
}

void Thrust3()
{
    float newTime = u_Time  - a_Rv1;

    if (newTime > 0.0)
    {
        float amp =  2* (a_Rv -0.5); // a_Rv -0.5 여기에서 왜 저렇게 변경된건지 시험
        float t = mod(newTime * 0.1, 1.0);

        vec4 newPosition;
        newPosition.x = -1.0 + 2.0 * t;
        newPosition.y = a_Position.y + amp * sin(t * 2.0 * c_PI);
        newPosition.z = a_Position.z;
        newPosition.w = 1.0;

        gl_Position = newPosition;
        gl_PointSize = 3.0;
    }
    else
    {
        gl_Position = vec4(10000., 0, 0, 1);
        gl_PointSize = 0.0;
    }
}

void Thrust4()
{
    float newTime = u_Time * 3.0 - a_Rv1;

    if (newTime > 0.0)
    {
        float amp = 2.0 * (a_Rv - 0.5);
        float phase = a_Rv1;                         // 파티클마다 다른 위상
        float t = mod(newTime * 0.1 + phase, 1.0);  // 위상 차이 추가

        vec4 newPosition;
        newPosition.x = -1.0 + 2.0 * t;
        newPosition.y = a_Position.y + amp * sin(t * 2.0 * c_PI);
        newPosition.z = a_Position.z;
        newPosition.w = 1.0;

        gl_Position = newPosition;
        gl_PointSize = 2.0;
    }
    else
    {
        gl_Position = vec4(10000.0, 0.0, 0.0, 1.0);
        gl_PointSize = 0.0;
    }
}

void Thrust5()
{
    float newTime = u_Time *0.1 - a_Rv1;

    if (newTime > 0.0)
    {
        float amp = 2.0 * (a_Rv - 0.5);
        float period = a_Rv2; // rv, rv1, rv2 일 때 다름  rv들 뒤에 곱하기를 해보기도 해서 실험해보기 () 세로 중앙에서 시작해서 0.5,-0.5 사이에서만 뿌려지게 
        float t = mod(newTime , 1.0);  // 위상 차이 추가

        vec4 newPosition;
        newPosition.x = -1.0 + 2.0 * t;
        newPosition.y = a_Position.y + amp * sin(t * 2.0 * c_PI * period );
        newPosition.z = a_Position.z;
        newPosition.w = 1.0;

        gl_Position = newPosition;
        gl_PointSize = 2.0;
    }
    else
    {
        gl_Position = vec4(10000.0, 0.0, 0.0, 1.0);
        gl_PointSize = 0.0;
    }
}

void Thrust6() //() 세로 중앙에서 시작해서 0.5,-0.5 사이에서만 뿌려지게 
{
    float newTime = u_Time *0.1 - a_Rv1;

    if (newTime > 0.0)
    {
        float t = mod(newTime , 1.0);  // 0~1
        float ampScale =  t * 0.5; // 0~0.5
        float amp = 2.0 * (a_Rv - 0.5) *ampScale;
        float period = a_Rv2; 

        vec4 newPosition;
        newPosition.x = -1.0 + 2.0 * t;
        newPosition.y = a_Position.y + amp * sin(t * 2.0 * c_PI * period );
        newPosition.z = a_Position.z;
        newPosition.w = 1.0;

        gl_Position = newPosition;
        gl_PointSize = 2.0;
    }
    else
    {
        gl_Position = vec4(10000.0, 0.0, 0.0, 1.0);
        gl_PointSize = 0.0;
    }
}

void Thrust7() // 나이에 따라 성장하는 것처럼 t가 1에서 시작해서 2로  + // 색 점점 검어지게 // 파티클 작아지면서 검어지게 하는게 최종목표
{
    float newTime = u_Time *0.1 - a_Rv1;

    if (newTime > 0.0)
    {
        float t = mod(newTime , 1.0);  // 위상 차이 추가
        float ampScale =  t * 0.5; // 0~0.5
        float amp = 2.0 * (a_Rv - 0.5) *ampScale;
        float period = a_Rv2; 
        float sizeScale = t*2; // 2 - t * 2 케이스도 있음 // 반대로 움직이는 케이스도 만들어보기
        vec4 newPosition;

        newPosition.x = a_Position.x *sizeScale -1 + 2* t;
        newPosition.y = a_Position.y + amp * sin(t * 2.0 * c_PI * period );
        newPosition.z = a_Position.z;
        newPosition.w = 1.0;

        gl_Position = newPosition;
        gl_PointSize = 4.0;
        v_Grey = 1-t;
    }
    else
    {
        gl_Position = vec4(10000.0, 0.0, 0.0, 1.0);
        gl_PointSize = 0.0;
        v_Grey = 0;
    }
}

void Thrust8()
{
    float newTime = u_Time * 0.1 - a_Rv1;

    if (newTime > 0.0)
    {
        float t = mod(newTime, 1.0);
        float ampScale = t * 0.5;
        float amp = 2.0 * (a_Rv - 0.5) * ampScale;
        float period = a_Rv2;

        vec4 newPosition;
        newPosition.x = -1.0 + 2.0 * t;
        newPosition.y = a_Position.y + amp * sin(t * 2.0 * c_PI * period);
        newPosition.z = a_Position.z;
        newPosition.w = 1.0;

        gl_Position = newPosition;

        // 오른쪽으로 갈수록 작아짐
        gl_PointSize = 1.0 + 3.0 * (1.0 - t);

        // 오른쪽으로 갈수록 검어짐
        v_Grey = 1.0 - t;
    }
    else
    {
        gl_Position = vec4(10000.0, 0.0, 0.0, 1.0);
        gl_PointSize = 0.0;
        v_Grey = 0.0;
    }
}


void Thrust9()
{
    float newTime = u_Time * 0.1 - a_Rv1;

    if (newTime > 0.0)
    {
        float t = mod(newTime, 1.0);
        float ampScale = t * 0.5;
        float amp = 2.0 * (a_Rv - 0.5) * ampScale;
        float period = a_Rv2;

        vec4 newPosition;
        newPosition.x = -1.0 + 2.0 * t;
        newPosition.y = a_Position.y + amp * sin(t * 2.0 * c_PI * period);
        newPosition.z = a_Position.z;
        newPosition.w = 1.0;

        gl_Position = newPosition;

        // 오른쪽으로 갈수록 작아짐
        gl_PointSize = 1.0 + 4.0 * (1.0 - t);

        // 색 계산용 시간값 전달
        v_T = t;
    }
    else
    {
        gl_Position = vec4(10000.0, 0.0, 0.0, 1.0);
        gl_PointSize = 0.0;
        v_T = 1.0;
    }
}
void main()
{
    Thrust9();
}