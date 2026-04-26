#include "stdafx.h"
#include "Renderer.h"
#include <vector>
#include <ctime>
#include <cstdlib>
#include <cmath>
#include <fstream>
#include <iostream>
#include <cstring>
#include "Dependencies/freeglut.h"

Renderer::Renderer(int windowSizeX, int windowSizeY)
{
    Initialize(windowSizeX, windowSizeY);
}

Renderer::~Renderer()
{
}

void Renderer::Initialize(int windowSizeX, int windowSizeY)
{
    m_WindowSizeX = windowSizeX;
    m_WindowSizeY = windowSizeY;

    srand((unsigned int)time(NULL));

    m_SolidRectShader = CompileShaders("./Shaders/SolidRect.vs", "./Shaders/SolidRect.fs");
    m_Triangles = CompileShaders("./Shaders/triangle.vs", "./Shaders/triangle.fs");
    m_FSShader = CompileShaders("./Shaders/Fullscreen.vs", "./Shaders/Fullscreen.frag");

    glEnable(GL_PROGRAM_POINT_SIZE);

    CreateVertexBufferObjects();
    genParticles(1000);

    int index = 0;
    for (int i = 0; i < 500; i++)
    {
        float x = (float)rand() / (float)RAND_MAX;
        float y = (float)rand() / (float)RAND_MAX;
        float sTime = 5.0f * rand() / (float)RAND_MAX;
        float LTime = 0.5f * rand() / (float)RAND_MAX;
        m_RainInfo[index] = x; index++;
        m_RainInfo[index] = y; index++;
        m_RainInfo[index] = sTime; index++;
        m_RainInfo[index] = LTime; index++;

    }

    if (m_SolidRectShader > 0 && m_Triangles > 0 && m_FSShader > 0
        && m_VBORect > 0 && m_ParticleVBO > 0 && m_VBOFS > 0)
    {
        m_Initialized = true;
    }
}

bool Renderer::IsInitialized()
{
    return m_Initialized;
}

void Renderer::CreateVertexBufferObjects()
{
    float rect[] =
    {
        -1.f / m_WindowSizeX, -1.f / m_WindowSizeY, 0.f,
        -1.f / m_WindowSizeX,  1.f / m_WindowSizeY, 0.f,
         1.f / m_WindowSizeX,  1.f / m_WindowSizeY, 0.f,

        -1.f / m_WindowSizeX, -1.f / m_WindowSizeY, 0.f,
         1.f / m_WindowSizeX,  1.f / m_WindowSizeY, 0.f,
         1.f / m_WindowSizeX, -1.f / m_WindowSizeY, 0.f,
    };

    glGenBuffers(1, &m_VBORect);
    glBindBuffer(GL_ARRAY_BUFFER, m_VBORect);
    glBufferData(GL_ARRAY_BUFFER, sizeof(rect), rect, GL_STATIC_DRAW);

    float centerx = 0.0f;
    float centery = 0.0f;
    float vx = 1.0f;
    float vy = 1.0f;
    float mass = 1.0f;
    float size = 0.5f;

    float triangle[] =
    {
        centerx - size / 2, centery - size / 2, 0.0f, mass, vx, vy,
        centerx + size / 2, centery - size / 2, 0.0f, mass, vx, vy,
        centerx + size / 2, centery + size / 2, 0.0f, mass, vx, vy,

        centerx - size / 2, centery - size / 2, 0.0f, mass, vx, vy,
        centerx + size / 2, centery + size / 2, 0.0f, mass, vx, vy,
        centerx - size / 2, centery + size / 2, 0.0f, mass, vx, vy
    };

    glGenBuffers(1, &m_TriangleVBO);
    glBindBuffer(GL_ARRAY_BUFFER, m_TriangleVBO);
    glBufferData(GL_ARRAY_BUFFER, sizeof(triangle), triangle, GL_STATIC_DRAW);

    // Fullscreen rectangle (position + texcoord)
    float rectFS[] =
    {
        // x, y, z,    u, v
        -1.0f, -1.0f, 0.0f,   0.0f, 0.0f,
         1.0f, -1.0f, 0.0f,   1.0f, 0.0f,
         1.0f,  1.0f, 0.0f,   1.0f, 1.0f,

        -1.0f, -1.0f, 0.0f,   0.0f, 0.0f,
         1.0f,  1.0f, 0.0f,   1.0f, 1.0f,
        -1.0f,  1.0f, 0.0f,   0.0f, 1.0f
    };

    glGenBuffers(1, &m_VBOFS);
    glBindBuffer(GL_ARRAY_BUFFER, m_VBOFS);
    glBufferData(GL_ARRAY_BUFFER, sizeof(rectFS), rectFS, GL_STATIC_DRAW);

    glBindBuffer(GL_ARRAY_BUFFER, 0);
}

void Renderer::AddShader(GLuint ShaderProgram, const char* pShaderText, GLenum ShaderType)
{
    GLuint ShaderObj = glCreateShader(ShaderType);

    if (ShaderObj == 0) {
        fprintf(stderr, "Error creating shader type %d\n", ShaderType);
    }

    const GLchar* p[1];
    p[0] = pShaderText;
    GLint Lengths[1];
    Lengths[0] = (GLint)strlen(pShaderText);

    glShaderSource(ShaderObj, 1, p, Lengths);
    glCompileShader(ShaderObj);

    GLint success;
    glGetShaderiv(ShaderObj, GL_COMPILE_STATUS, &success);
    if (!success) {
        GLchar InfoLog[1024];
        glGetShaderInfoLog(ShaderObj, 1024, NULL, InfoLog);
        fprintf(stderr, "Error compiling shader type %d: '%s'\n", ShaderType, InfoLog);
        printf("%s \n", pShaderText);
    }

    glAttachShader(ShaderProgram, ShaderObj);
}

bool Renderer::ReadFile(char* filename, std::string* target)
{
    std::ifstream file(filename);
    if (file.fail())
    {
        std::cout << filename << " file loading failed.. \n";
        file.close();
        return false;
    }

    std::string line;
    while (getline(file, line)) {
        target->append(line.c_str());
        target->append("\n");
    }

    file.close();
    return true;
}

GLuint Renderer::CompileShaders(char* filenameVS, char* filenameFS)
{
    GLuint ShaderProgram = glCreateProgram();

    if (ShaderProgram == 0) {
        fprintf(stderr, "Error creating shader program\n");
    }

    std::string vs, fs;

    if (!ReadFile(filenameVS, &vs)) {
        printf("Error compiling vertex shader\n");
        return -1;
    }

    if (!ReadFile(filenameFS, &fs)) {
        printf("Error compiling fragment shader\n");
        return -1;
    }

    AddShader(ShaderProgram, vs.c_str(), GL_VERTEX_SHADER);
    AddShader(ShaderProgram, fs.c_str(), GL_FRAGMENT_SHADER);

    GLint Success = 0;
    GLchar ErrorLog[1024] = { 0 };

    glLinkProgram(ShaderProgram);
    glGetProgramiv(ShaderProgram, GL_LINK_STATUS, &Success);

    if (Success == 0) {
        glGetProgramInfoLog(ShaderProgram, sizeof(ErrorLog), NULL, ErrorLog);
        std::cout << filenameVS << ", " << filenameFS << " Error linking shader program\n" << ErrorLog;
        return -1;
    }

    glValidateProgram(ShaderProgram);
    glGetProgramiv(ShaderProgram, GL_VALIDATE_STATUS, &Success);
    if (!Success) {
        glGetProgramInfoLog(ShaderProgram, sizeof(ErrorLog), NULL, ErrorLog);
        std::cout << filenameVS << ", " << filenameFS << " Error validating shader program\n" << ErrorLog;
        return -1;
    }

    glUseProgram(ShaderProgram);
    std::cout << filenameVS << ", " << filenameFS << " Shader compiling is done.";

    return ShaderProgram;
}

void Renderer::DrawSolidRect(float x, float y, float z, float size, float r, float g, float b, float a)
{
    float newX, newY;
    (void)z;

    GetGLPosition(x, y, &newX, &newY);

    glUseProgram(m_SolidRectShader);

    glUniform4f(glGetUniformLocation(m_SolidRectShader, "u_Trans"), newX, newY, 0, size);
    glUniform4f(glGetUniformLocation(m_SolidRectShader, "u_Color"), r, g, b, a);

    int attribPosition = glGetAttribLocation(m_SolidRectShader, "a_Position");
    glEnableVertexAttribArray(attribPosition);

    glBindBuffer(GL_ARRAY_BUFFER, m_VBORect);
    glVertexAttribPointer(attribPosition, 3, GL_FLOAT, GL_FALSE, sizeof(float) * 3, 0);

    glDrawArrays(GL_TRIANGLES, 0, 6);

    glDisableVertexAttribArray(attribPosition);
    glBindBuffer(GL_ARRAY_BUFFER, 0);
    glBindFramebuffer(GL_FRAMEBUFFER, 0);
}

void Renderer::GetGLPosition(float x, float y, float* newX, float* newY)
{
    *newX = x * 2.f / m_WindowSizeX;
    *newY = y * 2.f / m_WindowSizeY;
}

void Renderer::genParticles(int count)
{
    std::vector<float> particles;
    particles.reserve(count * 9);

    for (int i = 0; i < count; i++)
    {
        float px = 0.0f;
        float py = 0.0f;
        float pz = 0.0f;

        float mass = 1.0f;

        float vx = ((rand() % 200) - 100) / 1500.0f;
        float vy = ((rand() % 200) - 100) / 1500.0f;

        float Rv = (rand() % 1000) / 1000.0f;
        float Rv1 = (rand() % 1000) / 1000.0f;
        float Rv2 = (rand() % 1000) / 1000.0f;

        particles.push_back(px);
        particles.push_back(py);
        particles.push_back(pz);
        particles.push_back(mass);
        particles.push_back(vx);
        particles.push_back(vy);
        particles.push_back(Rv);
        particles.push_back(Rv1);
        particles.push_back(Rv2);
    }

    m_VertexCount = count;

    glGenBuffers(1, &m_ParticleVBO);
    glBindBuffer(GL_ARRAY_BUFFER, m_ParticleVBO);
    glBufferData(GL_ARRAY_BUFFER, sizeof(float) * particles.size(), particles.data(), GL_STATIC_DRAW);
    glBindBuffer(GL_ARRAY_BUFFER, 0);
}

void Renderer::DrawTriangle(float x, float y, float* newX, float* newY)
{
    (void)x;
    (void)y;
    (void)newX;
    (void)newY;

    glUseProgram(m_Triangles);

    GLint uTime = glGetUniformLocation(m_Triangles, "u_Time");
    float gTime = glutGet(GLUT_ELAPSED_TIME) / 1000.0f;
    glUniform1f(uTime, gTime);

    int attribPosition = glGetAttribLocation(m_Triangles, "a_Position");
    int attribMass = glGetAttribLocation(m_Triangles, "a_Mass");
    int attribVel = glGetAttribLocation(m_Triangles, "a_Vel");
    int attribRv = glGetAttribLocation(m_Triangles, "a_Rv");
    int attribRv1 = glGetAttribLocation(m_Triangles, "a_Rv1");
    int attribRv2 = glGetAttribLocation(m_Triangles, "a_Rv2");

    glEnableVertexAttribArray(attribPosition);
    glEnableVertexAttribArray(attribMass);
    glEnableVertexAttribArray(attribVel);
    glEnableVertexAttribArray(attribRv);
    glEnableVertexAttribArray(attribRv1);
    glEnableVertexAttribArray(attribRv2);

    glBindBuffer(GL_ARRAY_BUFFER, m_ParticleVBO);

    glVertexAttribPointer(attribPosition, 3, GL_FLOAT, GL_FALSE, 9 * sizeof(float), (GLvoid*)0);
    glVertexAttribPointer(attribMass, 1, GL_FLOAT, GL_FALSE, 9 * sizeof(float), (GLvoid*)(sizeof(float) * 3));
    glVertexAttribPointer(attribVel, 2, GL_FLOAT, GL_FALSE, 9 * sizeof(float), (GLvoid*)(sizeof(float) * 4));
    glVertexAttribPointer(attribRv, 1, GL_FLOAT, GL_FALSE, 9 * sizeof(float), (GLvoid*)(sizeof(float) * 6));
    glVertexAttribPointer(attribRv1, 1, GL_FLOAT, GL_FALSE, 9 * sizeof(float), (GLvoid*)(sizeof(float) * 7));
    glVertexAttribPointer(attribRv2, 1, GL_FLOAT, GL_FALSE, 9 * sizeof(float), (GLvoid*)(sizeof(float) * 8));

    glDrawArrays(GL_POINTS, 0, m_VertexCount);

    glDisableVertexAttribArray(attribPosition);
    glDisableVertexAttribArray(attribMass);
    glDisableVertexAttribArray(attribVel);
    glDisableVertexAttribArray(attribRv);
    glDisableVertexAttribArray(attribRv1);
    glDisableVertexAttribArray(attribRv2);

    glBindBuffer(GL_ARRAY_BUFFER, 0);
}

void Renderer::DrawFS()
{
    glUseProgram(m_FSShader);

    GLint uTime = glGetUniformLocation(m_FSShader, "u_Time");
    float gTime = GetTickCount64() / 1000.0f;    glUniform1f(uTime, gTime);

    int uPoints = glGetAttribLocation(m_FSShader, "a_Points");
    glUniform4fv(uPoints, 400, m_RainInfo);

    int attribPosition = glGetAttribLocation(m_FSShader, "a_Position");
    int attribTex = glGetAttribLocation(m_FSShader, "a_Tex");

    glEnableVertexAttribArray(attribPosition);
    glEnableVertexAttribArray(attribTex);

    glBindBuffer(GL_ARRAY_BUFFER, m_VBOFS);

    glVertexAttribPointer(attribPosition, 3, GL_FLOAT, GL_FALSE, 5 * sizeof(float), (GLvoid*)0);
    glVertexAttribPointer(attribTex, 2, GL_FLOAT, GL_FALSE, 5 * sizeof(float), (GLvoid*)(sizeof(float) * 3));

    glDrawArrays(GL_TRIANGLES, 0, 6);

    glDisableVertexAttribArray(attribPosition);
    glDisableVertexAttribArray(attribTex);

    glBindBuffer(GL_ARRAY_BUFFER, 0);
}

