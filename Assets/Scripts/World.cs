using OpenTK.Mathematics;

namespace VoxelGame.World
{
    public class World
    {
        private static Vector3i PregenerateSize = (3, 3, 3);
        public static Vector3i ChunkSize = (3, 3, 3);

        public Dictionary<Vector3i, Chunk> Chunks = new();

        public Chunk GetOrCreateChunk(Vector3i chunkPos)
        {
            if (!Chunks.TryGetValue(chunkPos, out var chunk))
            {
                chunk = new Chunk(chunkPos);
                Chunks[chunkPos] = chunk;
                Console.WriteLine($"Generated chunk at : {chunk.Position}");
            }
            return chunk;
        }

        public void Pregenerate()
        {
            Vector3i worldPregenPositiveBound = (PregenerateSize - (1, 1, 1)) / 2;
            Vector3i worldPregenNegativeBound = worldPregenPositiveBound * -1;
            Console.WriteLine($"World Pregeneration Bounds = {worldPregenNegativeBound} - {worldPregenPositiveBound}");
            for (int Y = worldPregenNegativeBound.Y; Y <= worldPregenPositiveBound.Y; Y++)
            {
                for (int X = worldPregenNegativeBound.X; X <= worldPregenPositiveBound.X; X++)
                {
                    for (int Z = worldPregenNegativeBound.Z; Z <= worldPregenPositiveBound.Z; Z++)
                    {
                        GetOrCreateChunk((X, Y, Z));
                    }
                }
            }
        }

        public World()
        {

        }
    }

    public class Chunk
    {
        public Block[,,] Blocks { get; private set; }
        public Vector3 Position { get; private set; }

        public Chunk(Vector3 position)
        {
            Position = position;
            Blocks = new Block[World.ChunkSize.X, World.ChunkSize.Y, World.ChunkSize.Z];

            for (int x = 0; x < World.ChunkSize.X; x++)
            {
                for (int y = 0; y < World.ChunkSize.Y; y++)
                {
                    for (int z = 0; z < World.ChunkSize.Z; z++)
                    {
                        Blocks[x, y, z] = new Block(1, false);
                    }
                }
            }
        }
    }

    public struct Block
    {
        public bool Air { get; private set; }
        public int ID { get; private set; }

        public bool IsAir() => Air;
        public int GetID() => ID;

        public Block(int id, bool air = true)
        {
            ID = id;
            Air = air;
        }
    }
}