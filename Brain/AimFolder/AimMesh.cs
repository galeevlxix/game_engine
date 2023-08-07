﻿using game_2.Brain.SkyBoxFolder;
using game_2.Storage;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;
using game_2.MathFolder;

namespace game_2.Brain.AimFolder
{
    public class AimMesh : SkyboxMesh
    {
        public AimMesh()
        {
            Vertices = AimVertices.Vertices;
            Indices = AimVertices.Indices;
            texture_file_name =  AimVertices.TexturePath ;
            pers_proj = pers_mat();

            Load();
        }

        public override void Draw(Matrix4 matrix)
        {
            for (int i = 0; i < texture_file_name.Length; i++)
            {
                texture.Use(TextureUnit.Texture0);
            }
            GL.BindVertexArray(VAO);
            shader.setMatrices(matrix, Matrix4.Identity, Matrix4.Identity, pers_proj);
            GL.DrawElements(PrimitiveType.Triangles, Indices.Length, DrawElementsType.UnsignedInt, 0);
        }

        private Matrix4 pers_proj;

        private Matrix4 pers_mat()
        {
            float FOV = 50;
            float width = 1920;
            float height = 1080;
            float zNear = 1f;
            float zFar = 200;

            matrix4f pers = new matrix4f();
            pers.InitPersProjTransform(FOV, width, height, zNear, zFar);

            return pers.ToOpenTK();
        }
    }
}