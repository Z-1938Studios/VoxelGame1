#version 330

in vec2 texCoord;
out vec4 outputColor;

uniform sampler2D texture0;
uniform float bitDepth;

const float offset = 1.0 / 150.0;  

const float kernel[9] = float[](
    -1, -1, -1,
    -1,  9, -1,
    -1, -1, -1
);

const float kernelEdge[9] = float[](
     1,  1,  1,
     1, -8,  1,
     1,  1,  1
);

const float kernelOdd1[9] = float[](
     1, -1,  1,
    -1,  8, -1,
     1, -1,  1
);

void main()
{
    vec2 offsets[9] = vec2[](
        vec2(-offset,  offset), // top-left
        vec2( 0.0f,    offset), // top-center
        vec2( offset,  offset), // top-right
        vec2(-offset,  0.0f),   // center-left
        vec2( 0.0f,    0.0f),   // center-center
        vec2( offset,  0.0f),   // center-right
        vec2(-offset, -offset), // bottom-left
        vec2( 0.0f,   -offset), // bottom-center
        vec2( offset, -offset)  // bottom-right    
    );

    vec4 texList[9];
    for(int i = 0; i < 9; i++)
    {
        texList[i] = texture(texture0, texCoord.st + offsets[i]).xyzw;
    }
    
    vec4 tex = vec4(0.0);
    for(int i = 0; i < 9; i++)
    {
        tex += texList[i] * kernelOdd1[i];
    }

    

    vec4 posturized = floor(tex.rgba * bitDepth) / bitDepth;
    outputColor = posturized;
}