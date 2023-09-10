using game_2.Brain.ObjectFolder;

namespace game_2.Brain.AimFolder
{
    public class Aim : GameObj
    {
        public Aim()
        {
            mesh = new AimMesh();
            pipeline = new Pipeline();

            pipeline.SetPosition(0f, 0f, -1f);
            pipeline.SetScale(0.02f);
        }

        public override void Draw()
        {
            base.Draw();
        }

        public override void OnDelete()
        {
            base.OnDelete();
        }
    }
}
