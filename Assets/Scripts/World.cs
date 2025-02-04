using OpenTK.Mathematics;

namespace VoxelGame.World
{
    public class World
    {
        private static Vector3i PregenerateSize = (3, 3, 3);
        Vector3i worldPregenPositiveBound = (PregenerateSize - (1, 1, 1)) / 2;
        Vector3i worldPregenNegativeBound = (PregenerateSize - (1, 1, 1)) / 2 * -1;
        public static Vector3i ChunkSize = (32, 32, 32);

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
            Console.WriteLine($"Chunks pregenerated : {Chunks.Values.Count}");
        }

        public World()
        {

        }
    }

    public class Chunk : IDisposable
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

        public Vector3i WorldToLocal(Vector3 worldPos)
        {
            Vector3 local = worldPos - (Position - (World.ChunkSize / 2));
            return new Vector3i((int)local.X, (int)local.Y, (int)local.Z);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
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