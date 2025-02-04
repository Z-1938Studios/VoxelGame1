#version 330

in vec3 vPosition;
in vec3 vColor;

uniform mat4 modelView;
uniform mat4 cameraView;
uniform mat4 cameraProjection;

out vec4 color;

void main()
{
    //color = vec4(vColor,1.0);
    gl_Position = vec4(vPosition, 1.0) * modelView * cameraView * cameraProjection;
    color = vec4(normalize(mod(gl_Position, 12)).xyz, 1.);
}