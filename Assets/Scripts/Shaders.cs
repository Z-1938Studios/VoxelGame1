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

        private readonly Dictionary<string, (int Location, int ID)> attributeList = new();
        private readonly Dictionary<string, int> uniformList = new();

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
            uniformList[uniformName] = locationID;
        }

        public void InitAttribute(string attributeName)
        {
            int locationID = GL.GetAttribLocation(ProgramID, attributeName);
            // if (locationID == -1)
            //     throw new Exception($"Could not get attribute {attributeName}");
            int bufferID = GL.GenBuffer();
            attributeList[attributeName] = (locationID, bufferID);
        }

        public void SetUniform<T>(string uniformName, T value, int? index = null) where T : notnull
        {
            if (!uniformList.TryGetValue(uniformName, out var uniformLocation))
            {
                Console.WriteLine($"No uniform found for {uniformName}.\n\tAttempting To Create Uniform...");
                InitUniform(uniformName);
                if (!uniformList.TryGetValue(uniformName, out var attempedUniformLocation))
                {
                    throw new Exception($"Could not create Uniform {uniformName}");
                }
                uniformLocation = attempedUniformLocation;
            }

            if (typeof(T) == typeof(Vector2))
            {
                Bind();
                GL.Uniform2(uniformLocation, ref Unsafe.As<T, Vector2>(ref value));
            }
            else if (typeof(T) == typeof(Vector3))
            {
                Bind();
                GL.Uniform3(uniformLocation, ref Unsafe.As<T, Vector3>(ref value));
            }
            else if (typeof(T) == typeof(Vector4))
            {
                Bind();
                GL.Uniform4(uniformLocation, ref Unsafe.As<T, Vector4>(ref value));
            }
            else if (typeof(T) == typeof(Matrix4))
            {
                Bind();
                GL.UniformMatrix4(uniformLocation, true, ref Unsafe.As<T, Matrix4>(ref value));
            }
            else if (typeof(T) == typeof(Matrix3))
            {
                Bind();
                GL.UniformMatrix3(uniformLocation, true, ref Unsafe.As<T, Matrix3>(ref value));
            }
            else if (typeof(T) == typeof(Matrix2))
            {
                Bind();
                GL.UniformMatrix2(uniformLocation, true, ref Unsafe.As<T, Matrix2>(ref value));
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

        public void SetBufferData<T, T2>(string attributeName, T[] data, int size, int stride = 0, VertexAttribPointerType type = VertexAttribPointerType.Float, BufferUsageHint hint = BufferUsageHint.StaticDraw) where T : struct where T2 : struct
        {
            if (!attributeList.TryGetValue(attributeName, out var attributeData))
            {
                throw new Exception($"No attribute found for {attributeName}.");
            }
            int TSize = Marshal.SizeOf<T>() <= 0 ? 1 : Marshal.SizeOf<T>();
            int T2Size = Marshal.SizeOf<T2>() <= 0 ? 1 : Marshal.SizeOf<T2>();

            GL.BindBuffer(BufferTarget.ArrayBuffer, attributeData.ID);
            GL.BufferData(BufferTarget.ArrayBuffer, data.Length * TSize * T2Size, data, hint);
            GL.VertexAttribPointer(attributeData.Location, size, type, false, stride * T2Size, 0);
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
