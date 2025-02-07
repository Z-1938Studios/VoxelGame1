using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using StbImageSharp;

namespace VoxelGame.Visual
{
    public abstract class Texture
    {
        public string TEXUTRE_PATH { get; private set; }
        public string TEXTURE_NAME { get; private set; }
        public int TEXUTRE_ID { get; private set; }
        public TextureUnit TEXTURE_UNIT { get; private set; }
        private (int X, int Y) RES;
        public int RES_X
        {
            get { return this.RES.X; }
            private set
            {
                this.RES.X = value;
            }
        }
        public int RES_Y
        {
            get { return this.RES.Y; }
            private set
            {
                this.RES.Y = value;
            }
        }
        public Texture(
            string path,
            string name,
            int? res_x = null,
            int? res_y = null,
            (int, int)? res = null,
            TextureUnit textureUnit = TextureUnit.Texture0,
            ColorComponents colorComponents = ColorComponents.RedGreenBlueAlpha,
            PixelInternalFormat pixelInternalFormat = PixelInternalFormat.Rgba,
            PixelFormat pixelFormat = PixelFormat.Rgba,
            PixelType pixelType = PixelType.UnsignedByte,
            int levelOfDetail = 0,
            bool flipOnLoad = true,
            TextureMinFilter textureMinFilter = TextureMinFilter.Linear,
            TextureMagFilter textureMagFilter = TextureMagFilter.Linear,
            TextureWrapMode textureWrapS = TextureWrapMode.ClampToBorder,
            TextureWrapMode textureWrapT = TextureWrapMode.ClampToBorder
        )
        {
            this.TEXUTRE_PATH = path;
            this.TEXTURE_NAME = name;
            this.TEXTURE_UNIT = textureUnit;
            if (res.HasValue) this.RES = res.Value;
            else if (res_x.HasValue || res_y.HasValue)
            {
                if (res_x.HasValue)
                {
                    this.RES_X = res_x.Value;
                }
                if (res_y.HasValue)
                {
                    this.RES_Y = res_y.Value;
                }
            }
            StbImage.stbi_set_flip_vertically_on_load(flipOnLoad ? 1 : 0);

            ImageResult imageResult = ImageResult.FromStream(File.OpenRead(Path.Combine("Assets", "Textures", TEXUTRE_PATH)), colorComponents);

            this.TEXUTRE_ID = GL.GenTexture();

            Bind();

            GL.TexImage2D(TextureTarget.Texture2D, levelOfDetail, pixelInternalFormat, (RES_X != 0 ? RES_X : imageResult.Width), (RES_Y != 0 ? RES_Y : imageResult.Height), 0, pixelFormat, pixelType, imageResult.Data);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)textureMinFilter);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)textureMagFilter);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)textureWrapS);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)textureWrapT);

            Unbind();
        }

        public virtual void BindToShader(Shader s)
        {
            s.InitUniform(TEXTURE_NAME);

            GL.ActiveTexture(TEXTURE_UNIT);
            Bind();
            s.SetUniformT<int>(TEXTURE_NAME, TEXTURE_UNIT - TextureUnit.Texture0);
        }

        public virtual void Unbind() => GL.BindTexture(TextureTarget.Texture2D, 0);
        public virtual void Bind() => GL.BindTexture(TextureTarget.Texture2D, TEXUTRE_ID);
    }

    public class FBTexture : FrameBufferTexture
    {
        public Vector4 BIND_CLEAR_COLOR { get; private set; }
        public Vector4 UNBIND_CLEAR_COLOR { get; private set; }
        public FBTexture(int width, int height, Vector4? bindClearColor = null, Vector4? unbindClearColor = null) : base(width, height)
        {
            if (bindClearColor.HasValue)
            {
                BIND_CLEAR_COLOR = bindClearColor.Value;
            }
            else
            {
                Console.WriteLine($"! ! Bind Clear Color for FBTexture was unset, defaulting to Vector4.Zero ! !");
                BIND_CLEAR_COLOR = Vector4.Zero;
            }

            if (unbindClearColor.HasValue)
            {
                UNBIND_CLEAR_COLOR = unbindClearColor.Value;
            }
            else
            {
                Console.WriteLine($"! ! Unbind Clear Color for FBTexture was unset, defaulting to Vector4.Zero ! !");
                UNBIND_CLEAR_COLOR = Vector4.Zero;
            }
        }

        public override void Bind()
        {
            base.Bind();
            GL.ClearColor(BIND_CLEAR_COLOR.X, BIND_CLEAR_COLOR.Y, BIND_CLEAR_COLOR.Z, BIND_CLEAR_COLOR.W);
        }

        public override void BindTex()
        {
            base.BindTex();
        }

        public override void SetDimensions(int width, int height)
        {
            base.SetDimensions(width, height);
        }

        public override void Unbind()
        {
            base.Unbind();
            GL.ClearColor(UNBIND_CLEAR_COLOR.X, UNBIND_CLEAR_COLOR.Y, UNBIND_CLEAR_COLOR.Z, UNBIND_CLEAR_COLOR.W);
        }
    }
}