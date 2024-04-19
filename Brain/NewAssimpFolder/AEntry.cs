using game_2.Brain.Lights.LightStructures;
using game_2.Brain.ObjectFolder;
using game_2.MathFolder;
using OpenTK.Mathematics;

namespace game_2.Brain.NewAssimpFolder
{
    public class AEntry
    {
        //в будущем сделать свой пайплайн
        public AMesh _mesh;

        public AEntry(List<AVertex> Vertices, List<int> Indices, AMaterial material)
        {
            _mesh = new AMesh(Vertices, Indices, material);
        }

        public void Draw() => _mesh.Draw();

        public void OnDelete() => _mesh.Dispose();
    }
}
