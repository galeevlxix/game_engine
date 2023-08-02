﻿using OpenTK.Graphics.OpenGL;
using game_2.Brain.ObjectFolder;
using OpenTK.Mathematics;
using game_2.Storage;
using game_2.FileManagers;

namespace game_2.Brain.SkyBoxFolder
{
    public class SkyboxMesh
    {
        protected int VBO { get; set; }
        protected int VAO { get; set; }
        protected int IBO { get; set; }

        protected float[] Vertices { get; set; }
        protected int[] Indices { get; set; }

        public Shader shader;
        public Texture texture;

        protected string texPath;

        public SkyboxMesh() 
        {
            Vertices = SkyboxVertices.Vertices;
            Indices = SkyboxVertices.Indices;
            texPath = SkyboxVertices.TexturePath;

            Load();
        }

        private void Load()
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
            shader = new Shader(
                ShaderLoader.LoadShader("C:\\Users\\Lenovo\\source\\repos\\game_2\\Shaders\\Object\\VertexShader.hlsl"), 
                ShaderLoader.LoadShader("C:\\Users\\Lenovo\\source\\repos\\game_2\\Shaders\\Object\\FragmentShader.hlsl"));

            // Устанавливаем указатели атрибутов вершины
            var location = shader.GetAttribLocation("aPosition");
            GL.VertexAttribPointer(location, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.EnableVertexAttribArray(location);

            var texCordLocation = shader.GetAttribLocation("aTexCoord");
            GL.VertexAttribPointer(texCordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(texCordLocation);

            texture = Texture.Load(texPath);

            // Развязываем VAO и VBO
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);

            GL.Enable(EnableCap.DepthTest);
        }

        public void DrawSkybox(Matrix4 matrix)
        {
            texture.Use(TextureUnit.Texture0);
            GL.BindVertexArray(VAO);
            GL.DrawElements(PrimitiveType.Triangles, Indices.Length, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);

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