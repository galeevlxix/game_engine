using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Runtime.InteropServices;

namespace game_2.Brain.NewAssimpFolder
{
    public class AMesh
    {
        private VertexArrayObject VAO;
        private BufferObject<AVertex> VBO;
        private BufferObject<int> IBO;

        private int indicesCount;

        private AMaterial material;

        public AMesh(List<AVertex> Vertices, List<int> Indices, AMaterial material)
        {
            Load(Vertices, Indices);

            this.material = material;
        }

        private unsafe void Load(List<AVertex> Vertices, List<int> Indices)
        {
            // Генерация и привязка VAO и VBO
            // Привязываем данные вершины к текущему буферу по умолчанию
            VAO = new VertexArrayObject();
            VBO = new BufferObject<AVertex>(Vertices.ToArray(), BufferTarget.ArrayBuffer);
            VAO.LinkBufferObject(ref VBO);

            // Устанавливаем указатели атрибутов вершины
            VAO.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, sizeof(AVertex), IntPtr.Zero);
            VAO.VertexAttributePointer(1, 2, VertexAttribPointerType.Float, sizeof(AVertex), Marshal.OffsetOf(typeof(AVertex), "Tex"));
            VAO.VertexAttributePointer(2, 3, VertexAttribPointerType.Float, sizeof(AVertex), Marshal.OffsetOf(typeof(AVertex), "Normal"));
            VAO.VertexAttributePointer(3, 3, VertexAttribPointerType.Float, sizeof(AVertex), Marshal.OffsetOf(typeof(AVertex), "Tangent"));
            VAO.VertexAttributePointer(4, 3, VertexAttribPointerType.Float, sizeof(AVertex), Marshal.OffsetOf(typeof(AVertex), "Bitangent"));

            // Element Buffer
            IBO = new BufferObject<int>(Indices.ToArray(), BufferTarget.ElementArrayBuffer);
            VAO.LinkBufferObject(ref IBO);

            // Развязываем VAO и VBO
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);

            indicesCount = Indices.Count;
        }

        public void Draw(Matrix4 world, Matrix4 wvp)
        {
            CentralizedShaders.SetValue(ShaderName.AssimpShader, "world", world);
            CentralizedShaders.SetValue(ShaderName.AssimpShader, "wvp", wvp);
            VAO.Bind();
            material.Use();
            GL.DrawElements(BeginMode.Triangles, indicesCount, DrawElementsType.UnsignedInt, 0);
        }

        public void Draw(ShaderName shader, Matrix4 wvp)
        {
            CentralizedShaders.SetValue(shader, "wvp", wvp);
            VAO.Bind();
            material.Use();
            GL.DrawElements(BeginMode.Triangles, indicesCount, DrawElementsType.UnsignedInt, 0);
        }
        public void Draw(ShaderName shader, Matrix4 wvp, Matrix4 light_wvp, Matrix4 world)
        {
            CentralizedShaders.SetValue(shader, "wvp", wvp);
            CentralizedShaders.SetValue(shader, "light_wvp", light_wvp);
            CentralizedShaders.SetValue(shader, "world", world);
            VAO.Bind();
            material.Use();
            GL.DrawElements(BeginMode.Triangles, indicesCount, DrawElementsType.UnsignedInt, 0);
        }

        public void Dispose()
        {
            VAO.Dispose();
            VBO.Dispose();
            IBO.Dispose();
        }
    }
}
