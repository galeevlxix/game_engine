using game_2.Brain.Lights.LightStructures;
using game_2.Brain.ObjectFolder;
using game_2.MathFolder;

namespace game_2.Brain.NewAssimpFolder
{
    public class AEntry
    {
        private AMesh _mesh;
        public Pipeline pipeline;

        public AEntry(List<AVertex> Vertices, List<int> Indices, AMaterial material)
        {
            _mesh = new AMesh(Vertices, Indices, material);
            pipeline = new Pipeline();
        }

        public void Draw()  
        {
            _mesh.Draw(pipeline.getWorld(), pipeline.getWVP());
        }

        public void Draw(Spotlight light)
        {
            _mesh.Draw(pipeline.getWorld(), pipeline.getWVP(), pipeline.getWVP(light));
        }

        public void DrawInShadowShader(Spotlight light)
        {
            _mesh.DrawInShadowShader(pipeline.getWVP(light));
        }

        public void OnDelete() => _mesh.Dispose();
    }
}
