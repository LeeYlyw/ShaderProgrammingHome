#version 330

layout(location=0) out vec4 FragColor;

uniform float u_Time;
uniform sampler2D u_RGBTex;

in vec2 v_Tex;

const float c_PI = 3.141592;

void Flag()
{
    float amp = 0.5;
    float speed = 15.0;

    float sinInput = v_Tex.x * c_PI * 2.0 - u_Time * speed;
    float sinValue = v_Tex.x * amp * ((sin(sinInput) + 1.0) / 2.0 - 0.5) + 0.5;

    float fWidth = 0.0;
    float width = 0.5 * mix(1.0, fWidth, v_Tex.x);

    float grey = 0.0;

    if(v_Tex.y < sinValue + width / 2.0 && v_Tex.y > sinValue - width / 2.0)
    {
        grey = 1.0;
    }
    else
    {
        grey = 0.0;
        discard;
    }

    FragColor = vec4(grey);
}

void Flame()
{
    float amp = 0.5;
    float speed = 15.0;

    float sinInput = v_Tex.y * c_PI * 2.0 - u_Time * speed;
    float sinValue = v_Tex.y * amp * ((sin(sinInput) + 1.0) / 2.0 - 0.5) + 0.5;

    float fWidth = 0.0;
    float width = 0.5 * mix(1.0, fWidth, v_Tex.y);

    float grey = 0.0;

    if(v_Tex.x < sinValue + width / 2.0 && v_Tex.x > sinValue - width / 2.0)
    {
        grey = 1.0;
    }
    else
    {
        grey = 0.0;
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

    c0 = texture(u_RGBTex, vec2(v_Tex.x - offsetX * 2.0, v_Tex.y));
    c1 = texture(u_RGBTex, vec2(v_Tex.x - offsetX * 1.0, v_Tex.y));
    c2 = texture(u_RGBTex, vec2(v_Tex.x - offsetX * 0.0, v_Tex.y));
    c3 = texture(u_RGBTex, vec2(v_Tex.x + offsetX * 1.0, v_Tex.y));
    c4 = texture(u_RGBTex, vec2(v_Tex.x + offsetX * 2.0, v_Tex.y));

    vec4 sum = c0 + c1 + c2 + c3 + c4;
    sum = sum / 5.0;

    FragColor = sum;
}

void main()
{
    TextureSampling();
}