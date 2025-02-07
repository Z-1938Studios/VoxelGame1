using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using VoxelGame.Visual;

namespace VoxelGame
{
    public class GameConstants
    {
        public static NativeWindowSettings DEFAULT_SETTINGS(int width, int height, string title)
        {
            return new NativeWindowSettings()
            {
                ClientSize = (width, height),
                Title = title,
                TransparentFramebuffer = true,
                API = ContextAPI.OpenGL,
                APIVersion = new Version(3, 3),
                Profile = ContextProfile.Core
            };
        }
    }
    public class Game(int width, int height, string title, GameWindowSettings gameWindowSettings) : GameWindow(gameWindowSettings, GameConstants.DEFAULT_SETTINGS(width, height, title))
    {
        bool firstMove = true;
        Visual.Shader testShader = new();
        Visual.Shader screenShader = new();
        Visual.Camera camera = new();
        Vector3[] coldata = new Vector3[] {
            new Vector3(1f, 0f, 0f),
            new Vector3( 0f, 0f, 1f),
            new Vector3( 0f,  1f, 0f),
            new Vector3(1f, 0f, 1f),
            new Vector3( 0f, 1f, 1f),
            new Vector3( 1f,  1f, 0f)
        };
        Matrix4[] mviewdata = [Matrix4.Identity];

        static World.World world = new();

        static void WorldFunction()
        {
            world.Pregenerate();
        }

        List<Vector3> vdata = new();
        Thread worldGenerationThread = new(new ThreadStart(WorldFunction));

        // int __FBO, __FBTEX, __DEPTHBUF;
        FrameBufferTexture frameBufferTexture;
        protected override void OnLoad()
        {
            base.OnLoad();

            // worldGenerationThread.Start();
            camera.SetProjection(45.0f, (float)width / height);

            #region FrameBuffer Test

            // __FBO = GL.GenFramebuffer();
            // GL.BindFramebuffer(FramebufferTarget.Framebuffer, __FBO);

            // __FBTEX = GL.GenTexture();
            // GL.BindTexture(TextureTarget.Texture2D, __FBTEX);
            // GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
            // GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            // GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Linear);

            // GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, __FBTEX, 0);

            // __DEPTHBUF = GL.GenRenderbuffer();
            // GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, __DEPTHBUF);
            // GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent24, width, height);
            // GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, __DEPTHBUF);

            // FramebufferErrorCode _s = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
            // if(_s!=FramebufferErrorCode.FramebufferComplete) throw new Exception($"Frame Buffer Error : {_s}");

            // GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            #endregion

            frameBufferTexture = new FrameBufferTexture(width, height);

            vdata.AddRange(World.Meshing.BlockFaces.FORWARD);
            vdata.AddRange(World.Meshing.BlockFaces.BACK);
            vdata.AddRange(World.Meshing.BlockFaces.LEFT);
            vdata.AddRange(World.Meshing.BlockFaces.RIGHT);
            vdata.AddRange(World.Meshing.BlockFaces.TOP);
            vdata.AddRange(World.Meshing.BlockFaces.BOTTOM);

            testShader.InitProgram("test/test.vert", "test/test.frag");

            testShader.InitAttribute("vPosition");
            testShader.InitAttribute("vColor");
            testShader.BufferData<Vector3, float>("vPosition", [.. vdata], 3, stride: 3);
            testShader.BufferData<Vector3, float>("vColor", coldata, 3, stride: 3);

            testShader.InitUniform("modelView");
            testShader.InitUniform("cameraView");
            testShader.InitUniform("cameraProjection");
            testShader.Bind();
            testShader.SetUniformT<Matrix4>("modelView", mviewdata[0]);
            testShader.SetUniformT<Matrix4>("cameraView", camera.GetViewMatrix());
            testShader.SetUniformT<Matrix4>("cameraProjection", camera.GetProjectionMatrix());

            screenShader.InitProgram("screenShader/vert.glsl", "screenShader/frag.glsl");
            screenShader.InitUniform("bitDepth");
            screenShader.SetUniformT<float>("bitDepth", 256);

            GL.PointSize(5.0f);
            GL.LineWidth(5.0f);

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.ProgramPointSize);
            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.Texture2D);

            GL.DepthFunc(DepthFunction.Less);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            GL.ClearColor(0, 0, 0, 0);

            CursorState = CursorState.Grabbed;

        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            // GL.BindFramebuffer(FramebufferTarget.Framebuffer, __FBO);
            frameBufferTexture.Bind();
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            //testShader.Bind();
            testShader.Bind();
            testShader.SetUniformT<Matrix4>("cameraView", camera.GetViewMatrix());
            testShader.DrawArrays(vdata.Count);

            // GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            frameBufferTexture.Unbind();
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            // GL.BindTexture(TextureTarget.Texture2D, __FBTEX);
            frameBufferTexture.BindTex();
            screenShader.DrawArrays(6);
            //testShader.Unbind();

            //GL.Flush();
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

            if (input.IsKeyDown(Keys.Escape)) Close();

            if (input.IsKeyDown(Keys.S)) camera.Backward((float)args.Time);
            if (input.IsKeyDown(Keys.W)) camera.Forward((float)args.Time);
            if (input.IsKeyDown(Keys.A)) camera.Left((float)args.Time);
            if (input.IsKeyDown(Keys.D)) camera.Right((float)args.Time);
            if (input.IsKeyDown(Keys.Space)) camera.Up((float)args.Time);
            if (input.IsKeyDown(Keys.LeftShift)) camera.Down((float)args.Time);

            if (input.IsKeyPressed(Keys.D1)) testShader.SetDrawingMode(PrimitiveType.Triangles);
            if (input.IsKeyPressed(Keys.D2)) testShader.SetDrawingMode(PrimitiveType.Lines);
            if (input.IsKeyPressed(Keys.D3)) testShader.SetDrawingMode(PrimitiveType.Points);
            if (input.IsKeyPressed(Keys.D4)) testShader.SetDrawingMode(PrimitiveType.LineLoop);

            if (input.IsKeyPressed(Keys.KeyPad0)) screenShader.SetUniformT<float>("bitDepth", 1);
            if (input.IsKeyPressed(Keys.KeyPad1)) screenShader.SetUniformT<float>("bitDepth", 2);
            if (input.IsKeyPressed(Keys.KeyPad2)) screenShader.SetUniformT<float>("bitDepth", 4);
            if (input.IsKeyPressed(Keys.KeyPad3)) screenShader.SetUniformT<float>("bitDepth", 8);
            if (input.IsKeyPressed(Keys.KeyPad4)) screenShader.SetUniformT<float>("bitDepth", 16);
            if (input.IsKeyPressed(Keys.KeyPad5)) screenShader.SetUniformT<float>("bitDepth", 32);
            if (input.IsKeyPressed(Keys.KeyPad6)) screenShader.SetUniformT<float>("bitDepth", 64);
            if (input.IsKeyPressed(Keys.KeyPad7)) screenShader.SetUniformT<float>("bitDepth", 128);
            if (input.IsKeyPressed(Keys.KeyPad8)) screenShader.SetUniformT<float>("bitDepth", 256);
            if (input.IsKeyPressed(Keys.KeyPad9)) screenShader.SetUniformT<float>("bitDepth", 24);
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