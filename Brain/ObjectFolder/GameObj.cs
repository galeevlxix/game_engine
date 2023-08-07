using game_2.MathFolder;
using game_2.Storage;

namespace game_2.Brain.ObjectFolder
{
    public class GameObj
    {
        protected Mesh mesh;
        public Pipeline pipeline;

        public GameObj()
        {
            mesh = new Mesh();
            pipeline = new Pipeline();
        }

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

        public virtual void Draw(int dist)
        {
            mesh.Draw(pipeline.getMVP().ToOpenTK(), Check_Distance(dist));
        }

        public void ShowHitBox()
        {
            mesh.ShowHitBox = true;
        }

        public void HideHitBox()
        {
            mesh.ShowHitBox = false;
        }

        public bool Check_Distance(float d)
        {
            var dx = Camera.Pos.x - pipeline.PositionVector.x;
            var dy = Camera.Pos.y - pipeline.PositionVector.y;
            var dz = Camera.Pos.z - pipeline.PositionVector.z;

            if (dx * dx + dy * dy + dz * dz <= d * d)
            {
                return true;
            }
            return false;
        }

        public void OnDelete()
        {
            mesh.Dispose();
        }
    }
}
