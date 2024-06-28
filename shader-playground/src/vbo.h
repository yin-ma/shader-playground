#pragma once
#include <GL/glew.h>
#include <GLFW/glfw3.h>
#include <vector>

#include "vertex.h"


class VBO
{
public:
	unsigned int vboID;

	VBO(float* vertices, unsigned int size);

	VBO(std::vector<Vertex>& vertices);
	~VBO();

	void bind();
	void unbind();
	void setLayoutf(unsigned int location, int size, unsigned int stride, int index);
};