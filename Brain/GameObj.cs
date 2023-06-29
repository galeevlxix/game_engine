namespace game_2.Brain
{
    public class GameObj
    {
        private Mesh mesh;
        private Shader shader;
        protected Pipeline pipeline;

        public GameObj()
        {
            mesh = new Mesh();
            shader = new Shader(Storage.vertexShader, Storage.fragmentShader);
            pipeline = new Pipeline();
            Position(0, 0, -2);
            Rotate(0, 0, 0);
            Scale(0, 0, 0);
            pipeline.mPersProj = new mPersProj();
        }

        public GameObj(string file_name)
        {
            mesh = new Mesh(file_name);
            shader = new Shader(Storage.vertexShader, Storage.fragmentShader);
            pipeline = new Pipeline();
            Position(0, 0, -2);
            Rotate(0, 0, 0);
            Scale(0, 0, 0);
            pipeline.mPersProj = new mPersProj();
        }

        public void Draw()
        {
            shader.setMatrix(pipeline.getMVP());
            shader.Use();
            mesh.Draw();
        }

        public void Reset()
        {
            Position(0, 0, 0);
            Rotate(0, 0, 0);
            Scale(1, 1, 1);
        }

        public void Rotate(float x, float y, float z)
        {
            pipeline.Rotate(x, y, z);
        }

        public void Position(float x, float y, float z)
        {
            pipeline.Position(x, y, z);
        }

        public void Scale(float x, float y, float z)
        {
            pipeline.Scale(x, y, z);
        }
    }
}
