#version 330 core

uniform float u_Time;

layout(location = 0) in vec3 a_Position;

void Basic()
{
    float t = mod(u_Time, 1.0) * 10.0;

    vec4 newPosition;
    newPosition.x = a_Position.x + t * 0.1;
    newPosition.y = a_Position.y;
    newPosition.z = a_Position.z;
    newPosition.w = 1.0;

    gl_Position = newPosition;
}

void Sin1()
{
    float t = mod(u_Time, 1.0) * 10.0; // 0~1

    vec4 newPosition;
    newPosition.x = a_Position.x + t ;
    newPosition.y = a_Position.y + 0.5 * sin(t*2*3.141592); // 0.5 를 조정하면 낙폭이 더 커지거나 적어짐
    newPosition.z = a_Position.z;
    gl_Position = newPosition;

    gl_Position = newPosition;
}

void Sin2()
{
    float t = mod(u_Time, 1.0) * 10.0; // 0~1

    vec4 newPosition;
    newPosition.x = a_Position.x - 1 + t*2 ;
    newPosition.y = a_Position.y + 0.5 * sin(t*2*3.141592); // 0.5 를 조정하면 낙폭이 더 커지거나 적어짐
    newPosition.z = a_Position.z;
    gl_Position = newPosition;

    gl_Position = newPosition;
}

void main()
{
    Basic();
}