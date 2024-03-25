using OpenTK.Graphics.OpenGL4;
using game_2.Brain.ObjectFolder;
using OpenTK.Mathematics;

namespace game_2.Brain.MonochromeObjectFolder
{
    public class MonochromeObjectMesh : Mesh
    {
        public MonochromeObjectMesh()
        {
            Storage.SphereVertices.InitSegments(30);
            Load(Storage.SphereVertices.GetVertices(), Storage.SphereVertices.GetIndices());
        }

        protected override void Load(float[] Vertices, int[] Indices)
        {
            // Генерация и привязка VAO и VBO
            VAO = GL.GenVertexArray();
            GL.BindVertexArray(VAO);

            VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            // Привязываем данные вершины к текущему буферу по умолчанию
            // Static Draw, потому что наши данные о вершинах в буфере не меняются
            GL.BufferData(BufferTarget.ArrayBuffer, Vertices.Length * sizeof(float), Vertices, BufferUsageHint.StaticDraw);

            // Element Buffer
            IBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, IBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, Indices.Length * sizeof(int), Indices, BufferUsageHint.StaticDraw);

            // Устанавливаем указатели атрибутов вершины
            int location = CentralizedShaders.MonochromeShader.GetAttribLocation("aPosition");
            GL.VertexAttribPointer(location, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(location);

            // Развязываем VAO и VBO
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);

            indicesCount = Indices.Length;
        }

        public override void Draw(Matrix4 matrix)
        {
            CentralizedShaders.MonochromeShader.setMatrices(matrix);
            GL.BindVertexArray(VAO);
            GL.DrawElements(BeginMode.Triangles, indicesCount, DrawElementsType.UnsignedInt, 0);
        }

        public override void Dispose()
        {
            GL.DeleteBuffer(VBO);
            GL.DeleteBuffer(IBO);
            GL.DeleteVertexArray(VAO);
        }
    }
}
