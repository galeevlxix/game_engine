using game_2.Brain.Lights.LightStructures;
using game_2.Brain.ObjectFolder;
using game_2.MathFolder;
using OpenTK.Mathematics;

namespace game_2.Brain.NewAssimpFolder
{
    public class AEntry
    {
        public AMesh _mesh;

        public AEntry(List<AVertex> Vertices, List<int> Indices, AMaterial material)
        {
            _mesh = new AMesh(Vertices, Indices, material);
        }

        public void Draw(Shader shader, Matrix4 world, Matrix4 view, Matrix4 pers)
        {
            _mesh.Draw(shader, world, view , pers);
        }

        public void OnDelete() => _mesh.Dispose();
    }
}
