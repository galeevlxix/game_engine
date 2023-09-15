﻿using OpenTK.Graphics.OpenGL4;
using game_2.Brain.ObjectFolder;
using OpenTK.Mathematics;
using game_2.Storage;

namespace game_2.Brain.SkyBoxFolder
{
    public class SkyboxMesh : Mesh
    {
        public SkyboxMesh()
        {
            Vertices = SkyboxVertices.Vertices;
            Indices = SkyboxVertices.Indices;
            texture = Texture.Load(SkyboxVertices.TexturePath);

            Load();
        }

        protected override void Load()
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
            var location = CentralizedShaders.SkyBoxShader.GetAttribLocation("aPosition");
            GL.VertexAttribPointer(location, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.EnableVertexAttribArray(location);

            var texCordLocation = CentralizedShaders.SkyBoxShader.GetAttribLocation("aTexCoord");
            GL.VertexAttribPointer(texCordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(texCordLocation);

            // Развязываем VAO и VBO
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
        }

        public override void Draw(Matrix4 matrix)
        {
            GL.BindVertexArray(VAO);
            UseTextures();
            CentralizedShaders.SkyBoxShader.setMatrices(matrix, Camera.CameraRotation.ToOpenTK(), mPersProj.PersProjMatrix.ToOpenTK());
            GL.DrawElements(BeginMode.Triangles, Indices.Length, DrawElementsType.UnsignedInt, 0);
        }
    }
}
