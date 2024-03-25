using OpenTK.Graphics.OpenGL4;
using game_2.Storage;
using game_2.FileManagers;
using OpenTK.Mathematics;

namespace game_2.Brain.ObjectFolder
{
    public class Mesh : IDisposable
    {
        protected int VBO { get; set; }
        protected int VAO { get; set; }
        protected int IBO { get; set; }

        protected int indicesCount = 0;

        protected Texture texture;

        public Mesh()
        {
            texture = Texture.Load(BoxVertices.DiffusePath);
            Load(BoxVertices.Vertices, BoxVertices.Indices);
        }

        public Mesh(string file_name, string tex_file_name)
        {
            ModelLoader.LoadMesh(file_name, out float[] Vertices, out int[] Indices);
            texture = Texture.Load(tex_file_name);

            Load(Vertices, Indices);
        }

        public Mesh(float[] Vertices, int[] Indices, string tex_file_name)
        {
            texture = Texture.Load(tex_file_name);
            Load(Vertices, Indices);
        }

        protected virtual void Load(float[] Vertices, int[] Indices)
        {
            // Генерация и привязка VAO и VBO
            VAO = GL.GenVertexArray();
            GL.BindVertexArray(VAO);

            VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            // Привязываем данные вершины к текущему буферу по умолчанию
            // Static Draw, потому что наши данные о вершинах в буфере не меняются
            GL.BufferData(BufferTarget.ArrayBuffer, Vertices.Length * sizeof(float), Vertices, BufferUsageHint.DynamicDraw);

            // Element Buffer
            IBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, IBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, Indices.Length * sizeof(int), Indices, BufferUsageHint.DynamicDraw);

            // Устанавливаем указатели атрибутов вершины
            int location = CentralizedShaders.ObjectShader.GetAttribLocation("aPosition");
            GL.VertexAttribPointer(location, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);
            GL.EnableVertexAttribArray(location);

            int texCordLocation = CentralizedShaders.ObjectShader.GetAttribLocation("aTexCoord");
            GL.VertexAttribPointer(texCordLocation, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(texCordLocation);

            int normCordLocation = CentralizedShaders.ObjectShader.GetAttribLocation("aNormal");
            GL.VertexAttribPointer(normCordLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 5 * sizeof(float));
            GL.EnableVertexAttribArray(normCordLocation);

            // Развязываем VAO и VBO
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);

            indicesCount = Indices.Length;
        }

        public virtual void Draw(Matrix4 matrix)
        {
            CentralizedShaders.ObjectShader.setMatrices(matrix);
            GL.BindVertexArray(VAO);
            UseTextures();
            GL.DrawElements(BeginMode.Triangles, indicesCount, DrawElementsType.UnsignedInt, 0);
        }

        public virtual void Draw(Matrix4 matrix, Matrix4 cameraPos, Matrix4 cameraRot, Matrix4 PersProj)
        {
            CentralizedShaders.ObjectShader.setMatrices(matrix, cameraPos, cameraRot, PersProj);
            GL.BindVertexArray(VAO);
            UseTextures();
            GL.DrawElements(BeginMode.Triangles, indicesCount, DrawElementsType.UnsignedInt, 0);
        }

        public virtual void Draw(Matrix4 matrix, bool check)
        {
            if (check)
            {
                UseTextures();
                GL.BindVertexArray(VAO);
                CentralizedShaders.ObjectShader.setMatrices(matrix);
                GL.DrawElements(BeginMode.Triangles, indicesCount, DrawElementsType.UnsignedInt, 0);
            }
        }

        protected virtual void UseTextures() => texture.Use();

        public virtual void Dispose()
        {
            texture.Dispose();
            GL.DeleteBuffer(VBO);
            GL.DeleteBuffer(IBO);
            GL.DeleteVertexArray(VAO);
        }
    }
}
