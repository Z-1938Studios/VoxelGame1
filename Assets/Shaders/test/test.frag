#version 330 core

in vec4 color;
out vec4 outputColor;

uniform float bitDepth;

void main()
{
    vec4 posturizedColor = floor(color.rgba * bitDepth) / bitDepth;
    outputColor = posturizedColor;
}