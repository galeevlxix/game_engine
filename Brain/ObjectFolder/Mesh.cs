using OpenTK.Graphics.OpenGL;
using game_2.Storage;
using game_2.FileManagers;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;
using System.Numerics;
using game_2.MathFolder;

namespace game_2.Brain.ObjectFolder
{
    public class Mesh : IDisposable
    {
        protected int VBO { get; set; }
        protected int VAO { get; set; }
        protected int IBO { get; set; }

        protected float[] Vertices { get; set; }
        protected int[] Indices { get; set; }

        private vector3f min = new vector3f(0, 0, 0);
        private vector3f max = new vector3f(0, 0, 0);

        public Shader shader;
        public Texture texture;

        protected string texPath;

        public Mesh()
        {
            Vertices = Box.Vertices;
            Indices = Box.Indices;
            texPath = Box.TexturePath;

            InitBorder();
            Load();
        }

        public Mesh(string file_name, string tex_file_name)
        {
            ModelLoader.LoadMesh(file_name, out float[] Vertices, out int[] Indices);
            this.Vertices = Vertices;
            this.Indices = Indices;
            texPath = tex_file_name;

            InitBorder();
            Load();
        }

        public Mesh(float[] Vertices, int[] Indices, string texPath)
        {
            this.Vertices = Vertices;
            this.Indices = Indices;
            this.texPath = texPath;

            InitBorder();
            Load();
        }

        private void InitBorder()
        {
            for (int i = 0; i < Vertices.Length; i += 8)
            {
                if (i == 0)
                {
                    min.x = Vertices[i];
                    min.y = Vertices[i + 1];
                    min.z = Vertices[i + 2];
                    max.x = Vertices[i];
                    max.y = Vertices[i + 1];
                    max.z = Vertices[i + 2];
                }
                else
                {
                    if (Vertices[i] > max.x)
                    {
                        max.x = Vertices[i];
                    }
                    if (Vertices[i] < min.x)
                    {
                        min.x = Vertices[i];
                    }

                    if (Vertices[i + 1] > max.y)
                    {
                        max.y = Vertices[i + 1];
                    }
                    if (Vertices[i + 1] < min.y)
                    {
                        min.y = Vertices[i + 1];
                    }

                    if (Vertices[i + 2] > max.z)
                    {
                        max.z = Vertices[i + 2];
                    }
                    if (Vertices[i + 2] < min.z)
                    {
                        min.z = Vertices[i + 2];
                    }
                }
            }

            if (min.x == max.y || min.y == max.y || min.z == max.z) MustShowHB = false;

            vertices_border = new float[]
            {
                    min.x, max.y, max.z,        //ближняя
                    max.x, max.y, max.z,
                    min.x, min.y, max.z,
                    max.x, min.y, max.z,

                    max.x, max.y, max.z,        //правая 
                    max.x, max.y, min.z,
                    max.x, min.y, max.z,
                    max.x, min.y, min.z,

                    max.x, max.y, min.z,        //дальняя
                    min.x, max.y, min.z,
                    max.x, min.y, min.z,
                    min.x, min.y, min.z,

                    min.x, max.y, min.z,        //левая
                    min.x, max.y, max.z,
                    min.x, min.y, min.z,
                    min.x, min.y, max.z,

                    min.x, max.y, min.z,        //верхняя
                    max.x, max.y, min.z,
                    min.x, max.y, max.z,
                    max.x, max.y, max.z,

                    min.x, min.y, max.z,        //нижняя
                    max.x, min.y, max.z,
                    min.x, min.y, min.z,
                    max.x, min.y, min.z,

            };

            indices_border = new int[]
            {
                        9, 11, 8,
                        8, 11, 10,

                        1, 3, 0,
                        0, 3, 2,

                        5, 7, 4,
                        4, 7, 6,

                        13, 15, 12,
                        12, 15, 14,

                        22, 20, 23,
                        23, 20, 21,

                        17, 19, 16,
                        16, 19, 18

            };

            // Генерация и привязка VAO и VBO
            vao_border = GL.GenVertexArray();
            GL.BindVertexArray(vao_border);

            vbo_border = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_border);
            // Привязываем данные вершины к текущему буферу по умолчанию
            // Static Draw, потому что наши данные о вершинах в буфере не меняются
            GL.BufferData(BufferTarget.ArrayBuffer, vertices_border.Length * sizeof(float), vertices_border, BufferUsageHint.StaticDraw);

            // Element Buffer
            ibo_border = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo_border);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices_border.Length * sizeof(int), indices_border, BufferUsageHint.StaticDraw);

            shader_borber = new Shader(ShaderLoader.LoadShader("C:\\Users\\Lenovo\\source\\repos\\game_2\\Shaders\\Border\\vertShaderBorder.hlsl"),
                ShaderLoader.LoadShader("C:\\Users\\Lenovo\\source\\repos\\game_2\\Shaders\\Border\\fragmShaderBorder.hlsl"));

            // Устанавливаем указатели атрибутов вершины
            var location = shader_borber.GetAttribLocation("aPosition");
            GL.VertexAttribPointer(location, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(location);

            // Развязываем VAO и VBO
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);

            GL.Enable(EnableCap.DepthTest);
        }

        private int vao_border, ibo_border, vbo_border;
        private float[] vertices_border;
        private int[] indices_border;
        private Shader shader_borber;


        protected virtual void Load()
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
        private bool MustShowHB = true;

        public void Draw(Matrix4 matrix)
        {
            texture.Use(TextureUnit.Texture0);
            GL.BindVertexArray(VAO);
            GL.DrawElements(PrimitiveType.Triangles, Indices.Length, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
            shader.setMatrix(matrix);
            shader.Use();

            if (ShowHitBox && MustShowHB)
            {
                GL.BindVertexArray(vao_border);
                GL.DrawElements(PrimitiveType.Triangles, indices_border.Length, DrawElementsType.UnsignedInt, 0);
                GL.BindVertexArray(0);
                shader_borber.setMatrix(matrix);
                shader_borber.Use();
            }            
        }

        public void Dispose()
        {
            GL.DeleteBuffer(VBO);
            GL.DeleteBuffer(IBO);
            GL.DeleteVertexArray(VAO);

            GL.DeleteBuffer(vbo_border);
            GL.DeleteBuffer(ibo_border);
            GL.DeleteVertexArray(vao_border);
        }
    }
}
