﻿using OpenTK.Graphics.OpenGL;
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

        protected Shader shader;
        protected Texture[] textures;

        public Mesh()
        {
            Vertices = Box.Vertices;
            Indices = Box.Indices;
            textures = new Texture[] { Texture.Load(Box.TexturePath) };
            
            Load();
        }

        public Mesh(string file_name, string tex_file_name)
        {
            ModelLoader.LoadMesh(file_name, out float[] Vertices, out int[] Indices);
            this.Vertices = Vertices;
            this.Indices = Indices;
            textures = new Texture[] { Texture.Load(tex_file_name) };

            Load();
        }

        public Mesh(string file_name, string[] tex_file_names)
        {
            ModelLoader.LoadMesh(file_name, out float[] Vertices, out int[] Indices);
            this.Vertices = Vertices;
            this.Indices = Indices;
            textures = new Texture[tex_file_names.Length];
            for (int i = 0; i < tex_file_names.Length; i++)
            {
                textures[i] = Texture.Load(tex_file_names[i]);
                if (i == 0)
                {
                    textures[i].Use(TextureUnit.Texture0);
                }
                else
                {
                    textures[i].Use(TextureUnit.Texture1);
                }
            }

            Load();
        }

        public Mesh(float[] Vertices, int[] Indices, string tex_file_name)
        {
            this.Vertices = Vertices;
            this.Indices = Indices;
            textures = new Texture[] { Texture.Load(tex_file_name) };

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
            shader.Use();

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

            shader.setInt("texture0", 0);
            //shader.setInt("texture1", 1);

            // Развязываем VAO и VBO
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);

            GL.Enable(EnableCap.DepthTest);
        }

        public bool ShowHitBox = false;

        public virtual void Draw(Matrix4 matrix)
        {
            GL.BindVertexArray(VAO);
            UseTextures();
            shader.setMatrices(matrix);
            GL.DrawElements(PrimitiveType.Triangles, Indices.Length, DrawElementsType.UnsignedInt, 0);
        }

        public virtual void Draw(Matrix4 matrix, Matrix4 cameraPos, Matrix4 cameraRot, Matrix4 PersProj)
        {
            GL.BindVertexArray(VAO);
            UseTextures();
            shader.setMatrices(matrix, cameraPos, cameraRot, PersProj);
            GL.DrawElements(PrimitiveType.Triangles, Indices.Length, DrawElementsType.UnsignedInt, 0);
        }

        public virtual void Draw(Matrix4 matrix, bool check)
        {
            if (check)
            {
                GL.BindVertexArray(VAO);
                UseTextures();
                shader.setMatrices(matrix);
                GL.DrawElements(PrimitiveType.Triangles, Indices.Length, DrawElementsType.UnsignedInt, 0);
            }
        }

        protected void UseTextures()
        {
            textures[0].Use(TextureUnit.Texture0);
            //if(textures.Length > 1)
                //textures[1].Use(TextureUnit.Texture1);
        }

        public void Dispose()
        {
            shader.Dispose();
            DisposeTextures();
            GL.DeleteBuffer(VBO);
            GL.DeleteBuffer(IBO);
            GL.DeleteVertexArray(VAO);
        }

        private void DisposeTextures()
        {
            for (int i = 0; i < textures.Length; i++)
            {
                textures[i].Dispose();
            }
        }
    }
}
