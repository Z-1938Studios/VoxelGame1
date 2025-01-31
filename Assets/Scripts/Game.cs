using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace VoxelGame
{
    public class Game(int width, int height, string title, GameWindowSettings gameWindowSettings) : GameWindow(gameWindowSettings, new NativeWindowSettings() { ClientSize = (width, height), Title = title })
    {
        readonly float[,,] vertices =
        {
            {
                {
                    -1f,-1f,0f
                },
                {
                    -1f,1f,0f
                },
                {
                    1f,1f,0f
                }
            },
            {
                {
                    -1f,-1f,0f
                },
                {
                    1f,-1f,0f
                },
                {
                    1f,1f,0f
                }
            }
        };
        Matrix4 projection;

        Visual.Shader testShader = new();
        protected override void OnLoad()
        {
            base.OnLoad();
            testShader.InitProgram("test.vert", "test.frag");
            testShader.InitAttribute("test");
            testShader.SetBufferData<float>("test", [0f, 0f, 0f], 3);

            GL.ClearColor(0, 0, 0, 1);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            testShader.DrawArrays();

            SwapBuffers();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, e.Width, e.Height);

            projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, width / (float)height, 1, 64);
        }
    }
}