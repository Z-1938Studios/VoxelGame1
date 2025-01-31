using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using OpenTK.Graphics.ES30;
using OpenTK.Mathematics;

namespace VoxelGame.Visual
{
    public class Shader : IDisposable
    {
        public Int32 programID;
        private Int32 vertexID;
        private Int32 fragmentID;
        private Int32 geometryID;
        private Int32 computeID;

        private PrimitiveType drawingMode = PrimitiveType.Triangles;

        Dictionary<String, (Int32 Location, Int32 Buf)> attributeList = new Dictionary<String, (Int32 Location, Int32 Buf)>();
        Dictionary<String, (Int32 Location, Int32 Buf)> uniformList = new Dictionary<String, (Int32 Location, Int32 Buf)>();

        public void Dispose()
        {
            GL.DeleteProgram(programID);
        }
        public void InitProgram(String vertexPath, String fragmentPath, String? geometryPath = null, String? computePath = null)
        {
            programID = GL.CreateProgram();

            ShaderUtils.LoadShader(vertexPath, ShaderType.VertexShader, programID, out vertexID);
            ShaderUtils.LoadShader(fragmentPath, ShaderType.FragmentShader, programID, out fragmentID);
            if (geometryPath != null)
                ShaderUtils.LoadShader(geometryPath, ShaderType.GeometryShader, programID, out geometryID);
            if (computePath != null)
                ShaderUtils.LoadShader(computePath, ShaderType.ComputeShader, programID, out computeID);
            GL.LinkProgram(programID);
            Console.WriteLine(GL.GetProgramInfoLog(programID));
        }

        public void InitUniform(String uniformName, Int32 n = 1)
        {
            Int32 LocationID = GL.GetUniformLocation(programID, uniformName);
            if (LocationID == -1)
                throw new Exception($"Could not get Uniform {uniformName}");
            GL.GenBuffers(n, out Int32 ID);
            uniformList[uniformName] = (LocationID, ID);
        }
        public void InitAttribute(String attributeName, Int32 n = 1)
        {
            Int32 LocationID = GL.GetAttribLocation(programID, attributeName);
            if (LocationID == -1)
                throw new Exception($"Could not get Attribute {attributeName}");
            GL.GenBuffers(n, out Int32 ID);
            attributeList[attributeName] = (LocationID, ID);
        }
        public void SetUniform<T>(String uniformName, T[] data, Int32 index) where T : struct
        {
            if (!uniformList.TryGetValue(uniformName, out var uniformData))
            {
                throw new Exception($"No uniform was found for {uniformName}.");
            }
            Type T_TYPE = typeof(T);
            if (T_TYPE == typeof(Vector2))
                GL.Uniform2(uniformData.Location, ref Unsafe.As<T, Vector2>(ref data[index]));
            else if (T_TYPE == typeof(Vector3))
                GL.Uniform3(uniformData.Location, ref Unsafe.As<T, Vector3>(ref data[index]));
            else if (T_TYPE == typeof(Vector4))
                GL.Uniform4(uniformData.Location, ref Unsafe.As<T, Vector4>(ref data[index]));
            else if (T_TYPE == typeof(Matrix2))
                GL.UniformMatrix2(uniformData.Location, false, ref Unsafe.As<T, Matrix2>(ref data[index]));
            else if (T_TYPE == typeof(Matrix3))
                GL.UniformMatrix3(uniformData.Location, false, ref Unsafe.As<T, Matrix3>(ref data[index]));
            else if (T_TYPE == typeof(Matrix4))
                GL.UniformMatrix4(uniformData.Location, false, ref Unsafe.As<T, Matrix4>(ref data[index]));
            else
                throw new Exception($"No uniform was found for {uniformName}, or the type assigned ({typeof(T)}) was incompatible.");
        }

        
        public void SetBufferData<T>(String attributeName, T[] data, Int32 size, System.Boolean normalized = false, VertexAttribPointerType type = VertexAttribPointerType.Float, BufferTarget target = BufferTarget.ArrayBuffer, BufferUsageHint hint = BufferUsageHint.StaticDraw) where T : struct
        {
            if (!attributeList.TryGetValue(attributeName, out var attributeData))
            {
                throw new Exception($"No attribute was found for {attributeName}.");
            }
            int sizeInBytes = Marshal.SizeOf<T>();
            int totalSize = sizeInBytes * data.Length;
            GL.BindBuffer(target, attributeData.Buf);
            GL.BufferData<T>(target, totalSize, data , hint);
            GL.VertexAttribPointer(attributeData.Location, size, type, normalized, 0, 0);
        }

        public void EnableVertexAttribArray(String attributeName)
        {
            if (!attributeList.TryGetValue(attributeName, out var attributeData))
            {
                throw new Exception($"No attribute was found for {attributeName}.");
            }
            GL.EnableVertexAttribArray(attributeData.Location);
        }
        public void DisableVertexAttribArray(String attributeName)
        {
            if (!attributeList.TryGetValue(attributeName, out var attributeData))
            {
                throw new Exception($"No attribute was found for {attributeName}.");
            }
            GL.DisableVertexAttribArray(attributeData.Location);
        }

        public void DrawArrays()
        {
            GL.DrawArrays(drawingMode, 0, 3);
        }

        
    }
    public class ShaderUtils
    {
        public static void LoadShader(String filepath, ShaderType type, Int32 program, out Int32 address)
        {
            address = GL.CreateShader(type);
            using (StreamReader sr = new StreamReader(Path.Join("Assets", "Shaders", filepath)))
            {
                GL.ShaderSource(address, sr.ReadToEnd());
            }
            GL.CompileShader(address);
            GL.AttachShader(program, address);
            Console.WriteLine(GL.GetShaderInfoLog(address));
        }
    }
}
