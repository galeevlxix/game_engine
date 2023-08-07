using game_2.Brain.ObjectFolder;

namespace game_2.Brain.SkyBoxFolder
{
    public class Skybox : GameObj
    {
        public Skybox() : base() 
        {
            mesh = new SkyboxMesh();
            pipeline = new Pipeline();

            pipeline.SetPosition(Camera.Pos.x, Camera.Pos.y, Camera.Pos.z);
            pipeline.SetAngle(0, 0, 0);
            pipeline.SetScale(55);
        }

        public override void Draw()
        {
            pipeline.SetPosition(Camera.Pos.x, Camera.Pos.y, Camera.Pos.z);
            mesh.Draw(pipeline.getMVP().ToOpenTK());
        }
    }
}
