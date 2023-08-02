using game_2.Brain;
using game_2.Storage;

namespace game_2.Brain.ObjectFolder
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
        }

        public GameObj(string file_name, string tex_file_name)
        {
            mesh = new Mesh(file_name, tex_file_name);
            pipeline = new Pipeline();

        }

        public virtual void Draw()
        {
            mesh.Draw(pipeline.getMVP().ToOpenTK());
        }

        public void ShowHitBox()
        {
            mesh.ShowHitBox = true;
        }

        public void HideHitBox()
        {
            mesh.ShowHitBox = false;
        }

        public void OnDelete()
        {
            mesh.Dispose();
            mesh.shader.Dispose();
            mesh.texture.Dispose();
        }
    }
}
