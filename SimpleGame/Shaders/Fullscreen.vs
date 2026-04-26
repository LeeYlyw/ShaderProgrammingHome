#version 330 core

in vec3 a_Position;
in vec2 a_Tex;

out vec2 v_Tex;

void main()
{
    vec4 newPosition;
    newPosition = vec4(a_Position, 1.0);

    v_Tex = a_Tex;

    gl_Position = newPosition;
}