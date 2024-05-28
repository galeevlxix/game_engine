using game_2.Brain.SkyBoxFolder;
using game_2.Storage;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using game_2.MathFolder;
using game_2.Brain.InfoPanelFolder;

namespace game_2.Brain.AimFolder
{
    public class AimMesh : SkyboxMesh
    {
        public AimMesh()
        {
            texture = Texture.Load(AimVertices.TexturePath);
            Load(AimVertices.Vertices, AimVertices.Indices);
        }

        public override void Draw(Matrix4 world)
        {
            CentralizedShaders.SetValue(ShaderName.ScreenShader, world, ScreenStaticPersProjMat.PersProjMatrix.ToOpenTK());
            GL.BindVertexArray(VAO);
            UseTextures();
            GL.DrawElements(PrimitiveType.Triangles, indicesCount, DrawElementsType.UnsignedInt, 0);
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}