using OpenTK.Windowing.Desktop;

namespace VoxelGame
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            using (Game game = new Game(1280,720,"Test",GameWindowSettings.Default))
            {
                
                game.Run();
            }
        }
    }
}