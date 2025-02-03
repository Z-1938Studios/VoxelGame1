using OpenTK.Mathematics;
using VoxelGame.Visual;

namespace VoxelGame.World.Meshing
{
    public class ChunkMesh
    {
        Shader targetShader;
        Dictionary<int, Vector3> indices = new();
        public ChunkMesh(Shader s)
        {
            targetShader = s;
        }
        public void GetChunkMesh(Chunk c, Dictionary<Vector3i, Chunk> chunkList)
        {

        }
    }

    public static class BlockFaces
    {
        // Front (+Z)
        public static readonly Vector3[] FORWARD =
        {
            new( 1f,  1f,  1f),  // A
            new(-1f, -1f,  1f),  // B
            new( 1f, -1f,  1f),  // C

            new( 1f,  1f,  1f),  // A
            new(-1f,  1f,  1f),  // D
            new(-1f, -1f,  1f)   // B
        };

        // Back (-Z)
        public static readonly Vector3[] BACK =
        {
            new( 1f,  1f, -1f),  // A
            new( 1f, -1f, -1f),  // B
            new(-1f, -1f, -1f),  // C

            new( 1f,  1f, -1f),  // A
            new(-1f, -1f, -1f),  // C
            new(-1f,  1f, -1f)   // D
        };

        // Left (-X)
        public static readonly Vector3[] LEFT =
        {
            new(-1f,  1f,  1f),  // A
            new(-1f, -1f, -1f),  // B
            new(-1f, -1f,  1f),  // C

            new(-1f,  1f,  1f),  // A
            new(-1f,  1f, -1f),  // D
            new(-1f, -1f, -1f)   // B
        };

        // Right (+X)
        public static readonly Vector3[] RIGHT =
        {
            new( 1f,  1f,  1f),  // A
            new( 1f, -1f,  1f),  // B
            new( 1f, -1f, -1f),  // C

            new( 1f,  1f,  1f),  // A
            new( 1f, -1f, -1f),  // C
            new( 1f,  1f, -1f)   // D
        };

        // Top (+Y)
        public static readonly Vector3[] TOP =
        {
            new( 1f,  1f,  1f),  // A
            new( 1f,  1f, -1f),  // B
            new(-1f,  1f, -1f),  // C

            new( 1f,  1f,  1f),  // A
            new(-1f,  1f, -1f),  // C
            new(-1f,  1f,  1f)   // D
        };

        // Bottom (-Y)
        public static readonly Vector3[] BOTTOM =
        {
            new( 1f, -1f,  1f),  // A
            new(-1f, -1f,  1f),  // B
            new(-1f, -1f, -1f),  // C

            new( 1f, -1f,  1f),  // A
            new(-1f, -1f, -1f),  // C
            new( 1f, -1f, -1f)   // D
        };
    }

}