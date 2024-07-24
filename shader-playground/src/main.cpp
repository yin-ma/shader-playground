#include <GL/glew.h>
#include <GLFW/glfw3.h>

#include "vao.h"
#include "vbo.h"
#include "shader.h"

#include <vector>
#include <iostream>
#include <string>

void framebuffer_size_callback(GLFWwindow* window, int width, int height);

int SCR_WIDTH = 600;
int SCR_HEIGHT = 600;

float quadVertices[] = {
    // positions   // texCoords
    -1.0f,  1.0f,  0.0f, 1.0f,
    -1.0f, -1.0f,  0.0f, 0.0f,
     1.0f, -1.0f,  1.0f, 0.0f,

    -1.0f,  1.0f,  0.0f, 1.0f,
     1.0f, -1.0f,  1.0f, 0.0f,
     1.0f,  1.0f,  1.0f, 1.0f
};


int main(void)
{
    GLFWwindow* window;
    if (!glfwInit()) return -1;

    /* set opengl version */
    glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 3);
    glfwWindowHint(GLFW_CONTEXT_VERSION_MINOR, 3);
    glfwWindowHint(GLFW_OPENGL_PROFILE, GLFW_OPENGL_CORE_PROFILE);

    /* opengl config */
    //glfwWindowHint(GLFW_FLOATING, GL_TRUE);
    //glfwWindowHint(GLFW_TRANSPARENT_FRAMEBUFFER, GL_TRUE);

    window = glfwCreateWindow(SCR_WIDTH, SCR_HEIGHT, "Shader Playground", NULL, NULL);
    if (!window)
    {
        std::cout << "Fail to create glfw window" << std::endl;
        glfwTerminate();
        return -1;
    }

    glfwMakeContextCurrent(window);
    glfwSetFramebufferSizeCallback(window, framebuffer_size_callback);
    //glfwSwapInterval(1);

    if (glewInit() != GLEW_OK)
    {
        std::cout << "Fail to initialize glew" << std::endl;
        return -1;
    }

    std::cout << glGetString(GL_VERSION) << std::endl;

    /* opengl config */
    glViewport(0, 0, SCR_WIDTH, SCR_HEIGHT);
    glClearColor(0.05f, 0.05f, 0.05f, 0.05f);
    //glfwSetInputMode(window, GLFW_CURSOR, GLFW_CURSOR_DISABLED);
    //glPolygonMode(GL_FRONT_AND_BACK, GL_LINE);

    /* init camera(position, center, up) */

    VAO vao;
    VBO vbo(quadVertices, sizeof(quadVertices));
    vao.bind();
    vbo.bind();
    vbo.setLayoutf(0, 2, 4 * sizeof(float), 0);

    /* init shaders */
    Shader shader("./res/default.vs", "./res/terrain.fs");
    shader.bind();
    vao.bind();


    /* Loop until the user closes the window */
    while (!glfwWindowShouldClose(window))
    {
        glClearColor(0.0f, 0.0f, 0.0f, 0.0f);
        glClear(GL_COLOR_BUFFER_BIT);
        shader.setUniform1f("u_Time", glfwGetTime());
        glDrawArrays(GL_TRIANGLES, 0, 6);

        glfwSwapBuffers(window);
        glfwPollEvents();
    }

    glfwDestroyWindow(window);
    glfwTerminate();
    return 0;
}

void framebuffer_size_callback(GLFWwindow* window, int width, int height)
{
    SCR_WIDTH = width;
    SCR_HEIGHT = height;
    glViewport(0, 0, width, height);
}