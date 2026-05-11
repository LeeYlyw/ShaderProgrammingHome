#version 330

layout(location=0) out vec4 FragColor;

uniform float u_Time;
uniform sampler2D u_RGBTex;
uniform sampler2D u_CurrNumTex;
uniform sampler2D u_NumsTex;
uniform int u_InputNum;

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

void RGBMirror()
{
    vec2 tex = v_Tex;

    tex.y = 1.0 - abs(2.0 * v_Tex.y - 1.0);

    FragColor = texture(u_RGBTex, tex);
}

//교수님이 처음 준 뼈대
void TextureQ () 
{ 
    float tx = v_Tex.x; 
    float ty = v_Tex.y; 
    
    vec2 tex = vec2(tx,ty); 
    
    FragColor = texture(u_RGBTex, tex);
}

// 시험에 이것 응용으로 2~3개 나온다고 함
// 시험에서는 if문이 불가

void TextureQ1()
{
    float tx = v_Tex.x;
    float ty = v_Tex.y;

    // ty에 바로 연결하셨음 교수님꺼는
    ty = 1.0 - abs(2.0 * ty - 1.0);

    vec2 tex = vec2(tx, ty);

    FragColor = texture(u_RGBTex, tex);
}


// 내림연산을 잘 쓰면 된다고 하심 플로어?를 언급함

void TextureQ2()
{
    float tx = fract(v_Tex.x * 3.0);
    float ty = v_Tex.y / 3.0;

    float offsetX = 0.0;
    float offsetY = (2.0 - floor(v_Tex.x * 3.0)) / 3.0;

    vec2 tex = vec2(offsetX + tx, offsetY + ty);

    FragColor = texture(u_RGBTex, tex);
}

void TextureQ3()
{
    float tx = fract(v_Tex.x * 3.0);
    float ty = v_Tex.y / 3.0;

    float offsetX = 0.0;
    float offsetY = floor(v_Tex.x * 3.0) / 3.0;

    vec2 tex = vec2(offsetX + tx, offsetY + ty);

    FragColor = texture(u_RGBTex, tex);
}

void TextureQ4()
{
    float resolX = 5;
    float resolY = 5;
    float shear = 0.5 * u_Time;

    float offsetX = fract(ceil(v_Tex.y * resolY) * shear); //offset
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

    float tx = v_Tex.x/5;
    float ty = v_Tex.y/2;

    float offsetX = fract (index / 5.0);
    float offsetY = floor (index / 5.0) / 2.0;

    vec2 newTex = vec2(tx + offsetX, ty + offsetY);
    FragColor = texture(u_NumsTex, newTex);
}


void main()
{
    Nums();
}