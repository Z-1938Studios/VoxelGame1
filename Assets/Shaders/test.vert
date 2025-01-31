#version 330

in vec3 vPosition;
in vec3 vColor;

uniform mat4 modelView;

out vec4 color;

void main()
{
    gl_Position = modelView * vec4(vPosition, 1.0);
    color = vec4(vColor,1.0);
}