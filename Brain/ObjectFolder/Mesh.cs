using OpenTK.Graphics.OpenGL;
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

        protected float[] Vertices { get; set; }
        protected int[] Indices { get; set; }

        public Shader shader;
        public Texture texture;

        protected string texPath;

        public Mesh()
        {
            Vertices = Box.Vertices;
            Indices = Box.Indices;
            texPath = Box.TexturePath;

            Load();
        }

        public Mesh(string file_name, string tex_file_name)
        {
            ModelLoader.LoadMesh(file_name, out float[] Vertices, out int[] Indices);
            this.Vertices = Vertices;
            this.Indices = Indices;
            texPath = tex_file_name;

            Load();
        }

        public Mesh(float[] Vertices, int[] Indices, string texPath)
        {
            this.Vertices = Vertices;
            this.Indices = Indices;
            this.texPath = texPath;

            Load();
        }

        protected virtual void Load()
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

            // Шейдеры
            shader = new Shader(ShaderLoader.LoadVertexShader(), ShaderLoader.LoadFragmentShader());

            // Устанавливаем указатели атрибутов вершины
            var location = shader.GetAttribLocation("aPosition");
            GL.VertexAttribPointer(location, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);
            GL.EnableVertexAttribArray(location);

            var texCordLocation = shader.GetAttribLocation("aTexCoord");
            GL.VertexAttribPointer(texCordLocation, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(texCordLocation);

            var normCordLocation = shader.GetAttribLocation("aNormal");
            GL.VertexAttribPointer(normCordLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 5 * sizeof(float));
            GL.EnableVertexAttribArray(normCordLocation);

            // Текстуры
            texture = Texture.Load(texPath);

            // Развязываем VAO и VBO
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);

            GL.Enable(EnableCap.DepthTest);
        }

        public bool ShowHitBox = false;

        public virtual void Draw(Matrix4 matrix)
        {
            texture.Use(TextureUnit.Texture0);
            GL.BindVertexArray(VAO);
            GL.DrawElements(PrimitiveType.Triangles, Indices.Length, DrawElementsType.UnsignedInt, 0);
            shader.setMatrix(matrix);
            shader.Use();
        }

        public virtual void Draw(Matrix4 matrix, bool check)
        {
            if (check)
            {
                texture.Use(TextureUnit.Texture0);
                GL.BindVertexArray(VAO);
                GL.DrawElements(PrimitiveType.Triangles, Indices.Length, DrawElementsType.UnsignedInt, 0);
            }
            shader.setMatrix(matrix);
            shader.Use();
        }

        public void Dispose()
        {
            GL.DeleteBuffer(VBO);
            GL.DeleteBuffer(IBO);
            GL.DeleteVertexArray(VAO);
        }
    }
}
