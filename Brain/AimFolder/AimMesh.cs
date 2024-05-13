using game_2.Brain.SkyBoxFolder;
using game_2.Storage;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using game_2.MathFolder;

namespace game_2.Brain.AimFolder
{
    public class AimMesh : SkyboxMesh
    {
        public Matrix4 pers_proj;

        public AimMesh()
        {
            texture = Texture.Load(AimVertices.TexturePath);
            pers_proj = pers_mat();

            Load(AimVertices.Vertices, AimVertices.Indices);
        }

        public override void Draw(Matrix4 world)
        {
            CentralizedShaders.SetValue(ShaderName.ScreenShader, world, pers_proj);
            GL.BindVertexArray(VAO);
            UseTextures();
            GL.DrawElements(PrimitiveType.Triangles, indicesCount, DrawElementsType.UnsignedInt, 0);
        }

        protected Matrix4 pers_mat()
        {
            float FOV = 50;
            float width = 1920;
            float height = 1080;
            float zNear = 1f;
            float zFar = 200;

            return matrix4f.GetInitPersProjTransform(FOV, width, height, zNear, zFar).ToOpenTK();
        }

        public void ResizeWindow(int WindowWidth, int WindowHeigth)
        {
            float FOV = 50;
            float width = WindowWidth;
            float height = WindowHeigth;
            float zNear = 1f;
            float zFar = 200;

            pers_proj = matrix4f.GetInitPersProjTransform(FOV, width, height, zNear, zFar).ToOpenTK();
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}