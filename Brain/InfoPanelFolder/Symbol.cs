using game_2.Brain.AimFolder;

namespace game_2.Brain.InfoPanelFolder
{
    public class Symbol : Aim
    {
        public Symbol(int sym_number) 
        {
            mesh = new SymbolMesh(sym_number % 10, 9 - sym_number / 10);
            pipeline = new Pipeline();

            pipeline.SetScale(0.05f);
        }

        public override void OnDelete()
        {
            base.OnDelete();
        }
    }
}
