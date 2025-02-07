using OpenTK.Graphics.OpenGL4;

namespace VoxelGame.Visual
{
    public abstract class FrameBufferTexture
    {
        public virtual int FBO { get; private set; }
        public virtual int TEX { get; private set; }
        public virtual int DEPTHBUFFER { get; private set; }
        public FrameBufferTexture(int width, int height)
        {
            FBO = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, FBO);

            TEX = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, TEX);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Linear);

            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, TEX, 0);

            DEPTHBUFFER = GL.GenRenderbuffer();
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, DEPTHBUFFER);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent24, width, height);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, DEPTHBUFFER);

            FramebufferErrorCode _s = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
            if (_s != FramebufferErrorCode.FramebufferComplete) throw new Exception($"Frame Buffer Error : {_s}");

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public virtual void Bind()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, FBO);
        }

        public virtual void Unbind()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public virtual void BindTex()
        {
            GL.BindTexture(TextureTarget.Texture2D, TEX);
        }

        public virtual void SetDimensions(int width, int height)
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, FBO);

            GL.BindTexture(TextureTarget.Texture2D, TEX);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Linear);

            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, TEX, 0);

            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, DEPTHBUFFER);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent24, width, height);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, DEPTHBUFFER);

            FramebufferErrorCode _s = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
            if (_s != FramebufferErrorCode.FramebufferComplete) throw new Exception($"Frame Buffer Error : {_s}");

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }
    }
}