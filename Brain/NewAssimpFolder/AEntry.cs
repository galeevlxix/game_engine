using game_2.Brain.ObjectFolder;

namespace game_2.Brain.NewAssimpFolder
{
    public class AEntry
    {
        private AMesh _mesh;
        public Pipeline pipeline;

        public AEntry(List<AVertex> Vertices, List<int> Indices, ModelTexturePaths Paths)
        {
            _mesh = new AMesh(Vertices, Indices, Paths);
            pipeline = new Pipeline();
        }

        public void Draw()  
        {
            _mesh.Draw(pipeline.getMVP().ToOpenTK());
        }

        public void OnDelete() => _mesh.Dispose();
    }
}
