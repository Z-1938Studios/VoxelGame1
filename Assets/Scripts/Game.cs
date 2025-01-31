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

        Vector3[] vertdata = new Vector3[] { new Vector3(-0.8f, -0.8f, 0f),
                new Vector3( 0.8f, -0.8f, 0f),
                new Vector3( 0f,  0.8f, 0f)};
        Vector3[] coldata = new Vector3[] { new Vector3(1f, 0f, 0f),
                new Vector3( 0f, 0f, 1f),
                new Vector3( 0f,  1f, 0f)};
        Matrix4 mviewdata = Matrix4.Identity;
        protected override void OnLoad()
        {
            base.OnLoad();

            testShader.InitProgram("test.vert", "test.frag");
            testShader.InitAttribute("vPosition");
            testShader.SetBufferData<Vector3>("vPosition", vertdata, 3);
            testShader.InitAttribute("vColor");
            testShader.SetBufferData<Vector3>("vColor", coldata, 3);
            testShader.InitUniform("modelView");
            testShader.SetUniform<Matrix4>("modelView", mviewdata);

            GL.Enable(EnableCap.DepthTest | EnableCap.CullFace | EnableCap.StencilTest);

            GL.ClearColor(0, 0, 0, 1);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            //GL.Viewport(0, 0, width, height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            testShader.DrawArrays(vertdata.Length);
            //Console.WriteLine(GL.GetError());

            GL.Flush();
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