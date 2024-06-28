#include "vbo.h"
#include "GL/glew.h"
#include <GLFW/glfw3.h>

VBO::VBO(float* vertices, unsigned int size)
{
	glGenBuffers(1, &vboID);
	glBindBuffer(GL_ARRAY_BUFFER, vboID);
	glBufferData(GL_ARRAY_BUFFER, size, vertices, GL_STATIC_DRAW);
}

VBO::VBO(std::vector<Vertex>& vertices)
{
	glGenBuffers(1, &vboID);
	glBindBuffer(GL_ARRAY_BUFFER, vboID);
	glBufferData(GL_ARRAY_BUFFER, vertices.size() * sizeof(Vertex), vertices.data(), GL_STATIC_DRAW);
}

VBO::~VBO()
{
	glDeleteBuffers(1, &vboID);
}

void VBO::bind()
{
	glBindBuffer(GL_ARRAY_BUFFER, vboID);
}

void VBO::unbind()
{
	glBindBuffer(GL_ARRAY_BUFFER, 0);
}

void VBO::setLayoutf(unsigned int location, int size, unsigned int stride, int index)
{
	glVertexAttribPointer(location, size, GL_FLOAT, GL_FALSE, stride, (void*)(index * sizeof(float)));
	glEnableVertexAttribArray(location);
}