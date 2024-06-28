#pragma once

#include <GL/glew.h>
#include <string>

class Shader
{
public:
	unsigned int shaderID;

	Shader(const std::string& vertexShaderFilePath, const std::string& fragmentShaderFilePath);
	~Shader();

	static std::string parseShader(const std::string& filePath);
	void bind();
	void unbind();

	void setUniform1i(const std::string& key, int value);
	void setUniform1f(const std::string& key, float value);
	void setUniform3fv(const std::string& key, const float* vector);
	void setUniform4fv(const std::string& key, const float* vector);
	void setUniformMatrix4fv(const std::string& key, const float* mat);
};