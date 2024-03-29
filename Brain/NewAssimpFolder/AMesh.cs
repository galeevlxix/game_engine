using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Runtime.InteropServices;

namespace game_2.Brain.NewAssimpFolder
{
    public class AMesh
    {
        private Dictionary<string, Texture> textures;

        private VertexArrayObject VAO;
        private BufferObject<AVertex> VBO;
        private BufferObject<int> IBO;

        int indicesCount;

        private AMaterial material;

        public AMesh(List<AVertex> Vertices, List<int> Indices, AMaterial material)
        {
            textures= new Dictionary<string, Texture>();
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

        public unsafe void Draw(Matrix4 matrix)
        {
            CentralizedShaders.AssimpShader.setMatrices(matrix);
            VAO.Bind();
            material.Use();
            GL.DrawElements(BeginMode.Triangles, indicesCount, DrawElementsType.UnsignedInt, 0);
        }

        public void Dispose()
        {
            foreach (Texture texture in textures.Values) texture.Dispose();

            VAO.Dispose();
            VBO.Dispose();
            IBO.Dispose();
        }
    }
}
