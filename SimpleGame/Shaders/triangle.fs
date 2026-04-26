#version 330 core

in float v_T;
out vec4 FragColor;

void main()
{
    // t=0 근처: 밝은 노랑-주황
    vec3 hotColor = vec3(1.0, 0.85, 0.2);

    // 중간: 주황
    vec3 midColor = vec3(1.0, 0.45, 0.05);

    // 끝: 어두운 빨강
    vec3 coolColor = vec3(0.45, 0.0, 0.0);

    vec3 color;

    if (v_T < 0.5)
    {
        float k = v_T / 0.5;
        color = mix(hotColor, midColor, k);
    }
    else
    {
        float k = (v_T - 0.5) / 0.5;
        color = mix(midColor, coolColor, k);
    }

    FragColor = vec4(color, 1.0);
}