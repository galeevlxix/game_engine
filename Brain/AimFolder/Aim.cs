using game_2.Brain.ObjectFolder;
using game_2.MathFolder;

namespace game_2.Brain.AimFolder
{
    public class Aim : GameObj
    {
        public Aim()
        {
            Console.WriteLine("Загрузка прицела...");
            mesh = new AimMesh();

            pipeline = new Pipeline();

            pipeline.SetPosition(0f, 0f, -1f);
            pipeline.SetScale(0.01f);
        }

        public override void Draw()
        {
            mesh.Draw(pipeline.getWorld());
        }

        public override void OnDelete()
        {
            base.OnDelete();
        }
    }
}
