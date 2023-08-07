using game_2.Brain.ObjectFolder;

namespace game_2.Brain.AimFolder
{
    public class Aim : GameObj
    {
        public Aim()
        {
            mesh = new AimMesh();
            pipeline = new Pipeline();

            pipeline.SetPosition(0, 0, -1);
            pipeline.SetScale(0.02f);
        }
    }
}
