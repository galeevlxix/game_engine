using game_2.Brain.AimFolder;
using game_2.Brain.ObjectFolder;

namespace game_2.Brain.InfoPanelFolder
{
    public class Symbol : GameObj
    {
        public Symbol(int sym_number) 
        {
            mesh = new SymbolMesh(sym_number % 10, 9 - sym_number / 10);
            pipeline = new Pipeline();

            pipeline.SetPosition(0f, 0f, -1f);
            pipeline.SetScale(0.05f);
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
