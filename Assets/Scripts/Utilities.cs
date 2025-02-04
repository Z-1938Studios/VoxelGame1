using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using System.Runtime.CompilerServices;
namespace VoxelGame.Util
{
    public class OpenTK
    {
        public static void ShaderUniformSet<T>(int location, string name, T value) where T : notnull
        {
            switch (value)
            {
                case int v:
                    GL.Uniform1(location, v);
                    break;
                case float v:
                    GL.Uniform1(location, v);
                    break;
                case bool v:
                    GL.Uniform1(location, v ? 1 : 0);
                    break;

                // Vector types
                case Vector2 v:
                    GL.Uniform2(location, ref v);
                    break;
                case Vector3 v:
                    GL.Uniform3(location, ref v);
                    break;
                case Vector4 v:
                    GL.Uniform4(location, ref v);
                    break;
                case Vector2i v:
                    GL.Uniform2(location, v.X, v.Y);
                    break;
                case Vector3i v:
                    GL.Uniform3(location, v.X, v.Y, v.Z);
                    break;
                case Vector4i v:
                    GL.Uniform4(location, v.X, v.Y, v.Z, v.W);
                    break;

                // Matrix types
                case Matrix2 v:
                    GL.UniformMatrix2(location, true, ref v);
                    break;
                case Matrix3 v:
                    GL.UniformMatrix3(location, true, ref v);
                    break;
                case Matrix4 v:
                    GL.UniformMatrix4(location, true, ref v);
                    break;
                
                default:
                    throw new Exception($"Unsupported uniform type {typeof(T)} for {name}");
            }
        }
    }
}