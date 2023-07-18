using System.ComponentModel;
using game_2.FileManagers;
using game_2.MathFolder;
using game_2.Storage;

namespace game_2.Brain
{
    public class GameObj
    {
        protected Mesh mesh;
        public Pipeline pipeline;

        public GameObj(int g)
        {
            if (g == 1)
            {
                mesh = new Mesh(Box.Vertices, Box.Indices, Box.TexturePath);
            }
            else if (g == 2)
            {
                mesh = new Mesh(Floor.Vertices, Floor.Indices, Floor.TexturePath);
            }
            else
            {
                mesh = new Mesh();
            }
            
            pipeline = new Pipeline();
            pipeline.mPersProj = new mPersProj();
        }

        public GameObj(string file_name, string tex_file_name)
        {
            mesh = new Mesh(file_name, tex_file_name);
            pipeline = new Pipeline();

            pipeline.mPersProj = new mPersProj();
        }

        public virtual void Draw()
        {
            mesh.Draw();
            mesh.shader.setMatrix(matrix4f.ToFloatArray(pipeline.getMVP()));
            mesh.shader.Use();            
        }

        public void Clear()
        {
            mesh.Dispose();
            mesh.shader.Dispose();
        }
    }
}
