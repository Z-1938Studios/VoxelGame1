#version 330

in vec2 texCoord;
out vec4 outputColor;

uniform sampler2D texture0;
uniform float bitDepth;

void main()
{
    vec4 tex = vec4(texture(texture0, texCoord).xyzw);
    vec4 posturized = floor(tex.rgba * bitDepth) / bitDepth;
    outputColor = posturized;
}