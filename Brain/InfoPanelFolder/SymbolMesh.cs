using game_2.Brain.AimFolder;
using game_2.FileManagers;

namespace game_2.Brain.InfoPanelFolder
{
    public class SymbolMesh : AimMesh
    {
        public SymbolMesh(int col, int raw)
        {
            SymbolArrayOfVertices.GetVertices(col, raw, out float[] vertices, out int[] indices);
            texture = SymbolArrayOfVertices.texture;
            pers_proj = pers_mat();

            Load(vertices, indices);
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
