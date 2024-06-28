#pragma once

class VAO
{
public:
	unsigned int vaoID;

	VAO();
	~VAO();

	void bind();
	void unbind();

};