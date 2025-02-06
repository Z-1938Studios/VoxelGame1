#version 330

out vec2 texCoord;

const vec2 vertexList[] = vec2[](
    vec2( 1.0,  1.0),
    vec2(-1.0, -1.0),
    vec2( 1.0, -1.0),
    vec2( 1.0,  1.0),
    vec2(-1.0,  1.0),
    vec2(-1.0, -1.0)
);

const vec2 texcoords[] = vec2[](
    vec2(1.0,1.0),
    vec2(0.0,0.0),
    vec2(1.0,0.0),
    vec2(1.0,1.0),
    vec2(0.0,1.0),
    vec2(0.0,0.0)
);

void main()
{
    gl_Position = vec4(vertexList[gl_VertexID], 0.0, 1.0);
    texCoord = texcoords[gl_VertexID];
}