#pragma once

#include "Dependencies/glew.h"
#include <string>

class Renderer
{
public:
    Renderer(int windowSizeX, int windowSizeY);
    ~Renderer();

    void Initialize(int windowSizeX, int windowSizeY);
    bool IsInitialized();

    void DrawSolidRect(float x, float y, float z, float size, float r, float g, float b, float a);
    void DrawTriangle(float x, float y, float* newX, float* newY);
    void DrawFS();

private:
    void CreateVertexBufferObjects();
    void AddShader(GLuint ShaderProgram, const char* pShaderText, GLenum ShaderType);
    bool ReadFile(char* filename, std::string* target);
    GLuint CompileShaders(char* filenameVS, char* filenameFS);
    void GetGLPosition(float x, float y, float* newX, float* newY);
    void genParticles(int count);
    GLuint CreatePngTexture(char* filePath, GLuint samplingMethod);

private:
    bool m_Initialized = false;

    int m_WindowSizeX = 0;
    int m_WindowSizeY = 0;

    GLuint m_SolidRectShader = 0;
    GLuint m_Triangles = 0;
    GLuint m_FSShader = 0;

    GLuint m_VBORect = 0;
    GLuint m_TriangleVBO = 0;
    GLuint m_ParticleVBO = 0;
    GLuint m_VBOFS = 0;

    int m_VertexCount = 0;


    //Raindrop
    float m_RainInfo[2000];

    GLuint m_RgbTexture = 0;
    GLuint m_NumTexture[10];
    GLuint m_NumsTexture = 0;


};