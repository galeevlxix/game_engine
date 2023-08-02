using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using game_2.Brain.ObjectFolder;
using game_2.FileManagers;
using static OpenTK.Graphics.OpenGL.GL;

namespace game_2.Brain.SkyBoxFolder
{
    public class Skybox
    {
        SkyboxMesh mesh;
        Pipeline pipeline;

        public Skybox()
        {
            Init();
        }

        public void Init()
        {
            mesh = new SkyboxMesh();

            pipeline = new Pipeline();

            pipeline.SetPosition(Camera.Pos.x, Camera.Pos.y, Camera.Pos.z);
            pipeline.SetAngle(0, 0, 0);
            pipeline.SetScale(55);
        }

        public void Draw()
        {
            pipeline.SetPosition(Camera.Pos.x, Camera.Pos.y, Camera.Pos.z);
            mesh.DrawSkybox(pipeline.getMVP().ToOpenTK());
        }

        public void OnDelete()
        {
            mesh.Dispose();
            mesh.shader.Dispose();
            mesh.texture.Dispose();
        }
    }
}
