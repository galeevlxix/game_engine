using game_2.Brain.Lights.LightStructures;
using game_2.Brain.ObjectFolder;
using game_2.MathFolder;
using OpenTK.Mathematics;

namespace game_2.Brain.NewAssimpFolder
{
    public class AEntry
    {
        private AMesh _mesh;

        public AEntry(List<AVertex> Vertices, List<int> Indices, AMaterial material)
        {
            _mesh = new AMesh(Vertices, Indices, material);
        }

        public void Draw(Matrix4 world, Matrix4 wvp)  
        {
            _mesh.Draw(world, wvp);
        }

        public void Draw(ShaderName shader, Matrix4 wvp)
        {
            _mesh.Draw(shader, wvp);
        }
        public void Draw(ShaderName shader, Matrix4 wvp, Matrix4 light_wvp, Matrix4 world)
        {
            _mesh.Draw(shader, wvp, light_wvp, world);
        }       

        public void OnDelete() => _mesh.Dispose();
    }
}
