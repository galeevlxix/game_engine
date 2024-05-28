using game_2.Brain.AimFolder;
using game_2.FileManagers;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace game_2.Brain.InfoPanelFolder
{
    public class SymbolMesh
    {
        private int VBO { get; set; }
        private int VAO { get; set; }
        private int IBO { get; set; }

        private int indicesCount = 0;

        public SymbolMesh(int col, int raw)
        {
            SymbolArrayOfVertices.GetVertices(col, raw, out float[] vertices, out int[] indices);

            Load(vertices, indices);
        }

        private void Load(float[] Vertices, int[] Indices)
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
            int location = CentralizedShaders.GetAttribLocation(ShaderName.SkyBoxShader, "aPosition");
            GL.VertexAttribPointer(location, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.EnableVertexAttribArray(location);

            var texCordLocation = CentralizedShaders.GetAttribLocation(ShaderName.SkyBoxShader, "aTexCoord");
            GL.VertexAttribPointer(texCordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(texCordLocation);

            // Развязываем VAO и VBO
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);

            indicesCount = Indices.Length;
        }

        public void Draw(Matrix4 world)
        {
            CentralizedShaders.SetValue(ShaderName.ScreenShader, world, ScreenStaticPersProjMat.PersProjMatrix.ToOpenTK());
            GL.BindVertexArray(VAO);
            GL.DrawElements(PrimitiveType.Triangles, indicesCount, DrawElementsType.UnsignedInt, 0);
        }

        public void Dispose()
        {
            GL.DeleteBuffer(VBO);
            GL.DeleteBuffer(IBO);
            GL.DeleteVertexArray(VAO);
        }
    }
}
