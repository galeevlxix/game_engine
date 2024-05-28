using game_2.Brain.AimFolder;
using game_2.Brain.ObjectFolder;

namespace game_2.Brain.InfoPanelFolder
{
    public class Symbol
    {
        private SymbolMesh mesh;
        public Pipeline pipeline;
        public Symbol(int sym_number, float scale) 
        {
            mesh = new SymbolMesh(sym_number % 10, 9 - sym_number / 10);
            pipeline = new Pipeline();

            pipeline.SetPosition(0f, 0f, -1f);
            pipeline.SetScale(scale);
        }

        public void Draw()
        {
            mesh.Draw(pipeline.getWorld());
        }

        public void OnDelete()
        {
            mesh.Dispose();
        }
    }
}
