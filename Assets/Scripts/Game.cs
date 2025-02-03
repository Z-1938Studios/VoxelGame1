using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace VoxelGame
{
    public class Game(int width, int height, string title, GameWindowSettings gameWindowSettings) : GameWindow(gameWindowSettings, new NativeWindowSettings() { ClientSize = (width, height), Title = title })
    {
        bool firstMove = true;
        Matrix4 projection;

        Visual.Shader testShader = new();
        Visual.Camera camera = new();

        Vector3[] vertdata = {
            (1,1,1),
            (1,-1,1),
            (-1,-1,1),
            (1,1,1),
            (-1,1,1),
            (-1,-1,1),
            
            (1,1,-1),
            (-1,1,-1),
            (-1,-1,-1),
            (1,1,-1),
            (1,-1,-1),
            (-1,-1,-1)
        };
        Vector3[] coldata = new Vector3[] {
            new Vector3(1f, 0f, 0f),
            new Vector3( 0f, 0f, 1f),
            new Vector3( 0f,  1f, 0f),
            new Vector3(1f, 0f, 1f),
            new Vector3( 0f, 1f, 1f),
            new Vector3( 1f,  1f, 0f)
        };
        Matrix4 mviewdata = Matrix4.Identity;

        static World.World world = new();

        static void WorldFunction()
        {
            world.Pregenerate();
        }
        Thread worldGenerationThread = new(new ThreadStart(WorldFunction));
        protected override void OnLoad()
        {
            base.OnLoad();

            worldGenerationThread.Start();
            camera.SetProjection(45.0f, (float)width / height);

            testShader.InitProgram("test.vert", "test.frag");

            testShader.InitAttribute("vPosition");
            testShader.InitAttribute("vColor");
            testShader.SetBufferData<Vector3>("vPosition", vertdata, 3);
            testShader.SetBufferData<Vector3>("vColor", coldata, 3);

            testShader.InitUniform("modelView");
            testShader.InitUniform("cameraView");
            testShader.InitUniform("cameraProjection");
            testShader.SetUniform<Matrix4>("modelView", mviewdata);
            testShader.SetUniform<Matrix4>("cameraView", camera.GetViewMatrix());
            testShader.SetUniform<Matrix4>("cameraProjection", camera.GetProjectionMatrix());

            GL.PointSize(5.0f);
            GL.LineWidth(5.0f);
            //GL.CullFace(TriangleFace.Front);

            GL.Enable(EnableCap.DepthTest | EnableCap.CullFace | EnableCap.ProgramPointSize | EnableCap.Blend);
            GL.DepthFunc(DepthFunction.Less);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            GL.ClearColor(0, 0, 0, 1);

            CursorState = CursorState.Grabbed;
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            //testShader.Bind();
            testShader.SetUniform<Matrix4>("cameraView", camera.GetViewMatrix());
            testShader.DrawArrays(vertdata.Length);
            //testShader.Unbind();

            GL.Flush();
            SwapBuffers();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, e.Width, e.Height);
            camera.SetProjection(45.0f, (float)e.Width / e.Height);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            KeyboardState input = KeyboardState;

            if (input.IsKeyDown(Keys.Escape))
            {
                Close();
            }

            if (input.IsKeyDown(Keys.S))
            {
                camera.Backward((float)args.Time);
            }

            if (input.IsKeyDown(Keys.W))
            {
                camera.Forward((float)args.Time);
            }

            if (input.IsKeyDown(Keys.A))
            {
                camera.Left((float)args.Time);
            }

            if (input.IsKeyDown(Keys.D))
            {
                camera.Right((float)args.Time);
            }

            if (input.IsKeyDown(Keys.Space))
            {
                camera.Up((float)args.Time);
            }

            if (input.IsKeyDown(Keys.LeftShift))
            {
                camera.Down((float)args.Time);
            }

            if(input.IsKeyPressed(Keys.D1))
            {
                testShader.SetDrawingMode(PrimitiveType.Triangles);
            }

            if(input.IsKeyPressed(Keys.D2))
            {
                testShader.SetDrawingMode(PrimitiveType.LineLoop);
            }

            if(input.IsKeyPressed(Keys.D3))
            {
                testShader.SetDrawingMode(PrimitiveType.Points);
            }
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            base.OnMouseMove(e);
                if (firstMove)
                {
                    camera.UpdateLastPos(new(e.X, e.Y));
                    firstMove = false;
                }
                else
                {
                    Vector2 currentPos = new(e.X, e.Y);
                    camera.UpdateRotation(currentPos);
                }
                camera.Rotate();
            
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            camera.SetSpeed(camera.GetSpeed() + e.OffsetY);
        }
    }
}