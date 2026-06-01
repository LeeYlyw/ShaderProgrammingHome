#version 330

in vec3 a_Position;

uniform vec4 u_Trans;
uniform float u_Time;
uniform vec4 u_DropInfo[1000];
out float v_Grey;
out vec2 v_Tex;

float c_PI = 3.141592;

void Frag()
{
    float tX, tY;

    tX = a_Position.x + 0.5;
    tY = 1.0 - (a_Position.y + 0.5);

    v_Tex = vec2(tX, tY);

    float value = a_Position.x + 0.5;
    float newX = a_Position.x;

    float newY = a_Position.y * (1.0 - (value * 0.5)) +
        value * 0.25 * sin((newX + 0.5) * 2.0 * c_PI - u_Time);

    vec4 final = vec4(newX, newY, 0.0, 1.0);

    vec4 newPosition = final;

    float grey =
        (sin((newX + 0.5) * 2.0 * c_PI - u_Time) + 1.0) / 2.0;

    v_Grey = grey;

    gl_Position = newPosition;
}

void Circle()
{
    //vec4 points[2];

    //points[0] = vec4(0.0, 0.0, 1.0, 0.2); // x, y, w(lifeTime), z(startTime)
    //points[1] = vec4(0.2, 0.2, 0.5, 0.0);

    float accum = 0.0;

    for(int i = 0; i < 1000; i++)
    {
        vec2 center = u_DropInfo[i].xy -vec2(0.5, 0.5);
        vec2 pos = a_Position.xy;

        float lTime = u_DropInfo[i].z;
        float sTime = u_DropInfo[i].w;

        float nTime = u_Time - sTime;

        if(nTime > 0.0)
        {
            float lVal = fract(nTime / lTime); // 0~1
            float oneMinus = 1.0 - lVal;
            float t = lVal * lTime;

            float d = distance(center, pos);

            float range = t / 30.0;

            float fade = 15.0 * clamp(range - d, 0.0, 1.0);

            float sinValue = pow(abs(sin(d * 4.0 * c_PI * 8.0 - t * 2.0)), 3);

            accum += sinValue * fade * oneMinus;
        }
    }
    v_Grey = accum;

    //gl_Position = vec4(a_Position, 1.0);
    gl_Position = vec4(a_Position.x, a_Position.y + accum * 0.05, a_Position.z, 1.0);

}
void main()
{
    Circle();
}