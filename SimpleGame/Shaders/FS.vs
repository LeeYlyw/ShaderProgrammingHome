#version 330

in vec3 a_Pos;
in vec2 a_Tex;

out vec2 v_Tex;

uniform float u_Time;

void main()
{
	vec4 newPosition;
	newPosition = vec4(a_Pos, 1);

	v_Tex = a_Tex;

	gl_Position = newPosition;
}
