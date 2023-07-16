using System.ComponentModel;
using game_2.FileManagers;
using game_2.MathFolder;

namespace game_2.Brain
{
    public class GameObj
    {
        protected Mesh mesh;
        public Pipeline pipeline;

        public GameObj()
        {
            mesh = new Mesh();
            pipeline = new Pipeline();

            pipeline.mPersProj = new mPersProj();
        }

        public GameObj(string file_name)
        {
            mesh = new Mesh(file_name);
            pipeline = new Pipeline();

            pipeline.mPersProj = new mPersProj();
        }

        public virtual void Draw()
        {
            mesh.Draw();
            mesh.shader.setMatrix(matrix4f.ToFloatArray(pipeline.getMVP()));
            mesh.shader.Use();
        }
    }
}
