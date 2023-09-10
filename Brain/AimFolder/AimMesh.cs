using game_2.Brain.SkyBoxFolder;
using game_2.Storage;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using game_2.MathFolder;
using game_2.FileManagers;

namespace game_2.Brain.AimFolder
{
    public class AimMesh : SkyboxMesh
    {
        public AimMesh()
        {
            Vertices = AimVertices.Vertices;
            Indices = AimVertices.Indices;
            texture = Texture.Load(AimVertices.TexturePath);
            pers_proj = pers_mat();

            Load();
        }

        public override void Draw(Matrix4 matrix)
        {
            GL.BindVertexArray(VAO);
            UseTextures();
            CentralizedShaders.ScreenShader.setMatrices(matrix, pers_proj);
            GL.DrawElements(PrimitiveType.Triangles, Indices.Length, DrawElementsType.UnsignedInt, 0);
        }

        protected Matrix4 pers_proj;

        protected Matrix4 pers_mat()
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

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
