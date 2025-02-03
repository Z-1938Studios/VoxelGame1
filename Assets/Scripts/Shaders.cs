using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace VoxelGame.Visual
{
    public class Shader : IDisposable
    {
        public int ProgramID { get; private set; }
        private int vertexID;
        private int fragmentID;
        private int geometryID;
        private int computeID;
        private int VAO;

        private PrimitiveType drawingMode = PrimitiveType.Triangles;

        private readonly Dictionary<string, (int Location, int Buf)> attributeList = new();
        private readonly Dictionary<string, (int Location, int Buf)> uniformList = new();

        public void Dispose()
        {
            GL.DeleteProgram(ProgramID);
            GL.DeleteVertexArray(VAO);
        }

        public void InitProgram(string vertexPath, string fragmentPath, string? geometryPath = null, string? computePath = null)
        {
            ProgramID = GL.CreateProgram();

            ShaderUtils.LoadShader(vertexPath, ShaderType.VertexShader, ProgramID, out vertexID);
            ShaderUtils.LoadShader(fragmentPath, ShaderType.FragmentShader, ProgramID, out fragmentID);
            if (geometryPath != null)
                ShaderUtils.LoadShader(geometryPath, ShaderType.GeometryShader, ProgramID, out geometryID);
            if (computePath != null)
                ShaderUtils.LoadShader(computePath, ShaderType.ComputeShader, ProgramID, out computeID);

            GL.LinkProgram(ProgramID);
            GL.GetProgram(ProgramID, GetProgramParameterName.LinkStatus, out int success);
            if (success == 0)
            {
                throw new Exception($"Shader linking failed: {GL.GetProgramInfoLog(ProgramID)}");
            }

            GL.DeleteShader(vertexID);
            GL.DeleteShader(fragmentID);
            if (geometryID != 0) GL.DeleteShader(geometryID);
            if (computeID != 0) GL.DeleteShader(computeID);

            VAO = GL.GenVertexArray();
            GL.BindVertexArray(VAO);
        }

        public void InitUniform(string uniformName)
        {
            int locationID = GL.GetUniformLocation(ProgramID, uniformName);
            if (locationID == -1)
                throw new Exception($"Could not get uniform {uniformName}\n\tError: {GL.GetError()}");
            uniformList[uniformName] = (locationID, 0);
        }

        public void InitAttribute(string attributeName)
        {
            int locationID = GL.GetAttribLocation(ProgramID, attributeName);
            if (locationID == -1)
                throw new Exception($"Could not get attribute {attributeName}");
            GL.GenBuffers(1, out int bufferID);
            attributeList[attributeName] = (locationID, bufferID);
        }

        public void SetUniform<T>(string uniformName, T value, int? index = null) where T : notnull
        {
            if (!uniformList.TryGetValue(uniformName, out var uniformData))
            {
                throw new Exception($"No uniform found for {uniformName}.");
            }

            if (typeof(T) == typeof(Vector2))
            {
                Bind();
                GL.Uniform2(uniformData.Location, ref Unsafe.As<T, Vector2>(ref value));
            }
            else if (typeof(T) == typeof(Vector3))
            {
                Bind();
                GL.Uniform3(uniformData.Location, ref Unsafe.As<T, Vector3>(ref value));
            }
            else if (typeof(T) == typeof(Vector4))
            {
                Bind();
                GL.Uniform4(uniformData.Location, ref Unsafe.As<T, Vector4>(ref value));
            }
            else if (typeof(T) == typeof(Matrix4))
            {
                Bind();
                GL.UniformMatrix4(uniformData.Location, true, ref Unsafe.As<T, Matrix4>(ref value));
            }
            else if (typeof(T) == typeof(Matrix3))
            {
                Bind();
                GL.UniformMatrix3(uniformData.Location, true, ref Unsafe.As<T, Matrix3>(ref value));
            }
            else if (typeof(T) == typeof(Matrix2))
            {
                Bind();
                GL.UniformMatrix2(uniformData.Location, true, ref Unsafe.As<T, Matrix2>(ref value));
            }
            else
                throw new Exception($"Unsupported uniform type {typeof(T)} for {uniformName}");
        }

        public void Bind()
        {
            GL.UseProgram(ProgramID);
        }

        public void Unbind()
        {
            GL.UseProgram(0);
        }

        public void SetDrawingMode(PrimitiveType mode)
        {
            drawingMode = mode;
        }

        public void SetBufferData<T>(string attributeName, T[] data, int size, VertexAttribPointerType type = VertexAttribPointerType.Float, BufferUsageHint hint = BufferUsageHint.StaticDraw) where T : struct
        {
            if (!attributeList.TryGetValue(attributeName, out var attributeData))
            {
                throw new Exception($"No attribute found for {attributeName}.");
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, attributeData.Buf);
            GL.BufferData(BufferTarget.ArrayBuffer, Marshal.SizeOf<T>() * data.Length, data, hint);
            GL.VertexAttribPointer(attributeData.Location, size, type, false, 0, 0);
            GL.EnableVertexAttribArray(attributeData.Location);
        }

        public void DrawArrays(int vertexCount)
        {
            GL.UseProgram(ProgramID);
            GL.BindVertexArray(VAO);
            GL.DrawArrays(drawingMode, 0, vertexCount);
        }
    }

    public static class ShaderUtils
    {
        public static void LoadShader(string filepath, ShaderType type, int program, out int shaderID)
        {
            shaderID = GL.CreateShader(type);
            string shaderCode = File.ReadAllText(Path.Combine("Assets", "Shaders", filepath));
            GL.ShaderSource(shaderID, shaderCode);
            GL.CompileShader(shaderID);

            GL.GetShader(shaderID, ShaderParameter.CompileStatus, out int success);
            if (success == 0)
            {
                throw new Exception($"Shader compilation failed ({type}): {GL.GetShaderInfoLog(shaderID)}");
            }

            GL.AttachShader(program, shaderID);
        }
    }
}
