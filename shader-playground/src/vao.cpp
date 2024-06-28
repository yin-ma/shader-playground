#include "GL/glew.h"
#include "vao.h"
#include <iostream>

VAO::VAO()
{
	glGenVertexArrays(1, &vaoID);
}

VAO::~VAO()
{
	// model class handle delete vao
	//glDeleteVertexArrays(1, &vaoID);
}

void VAO::bind()
{
	glBindVertexArray(vaoID);
}

void VAO::unbind()
{
	glBindVertexArray(0);
}
