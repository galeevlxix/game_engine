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
                mesh = new Mesh(BoxVertices.Vertices, BoxVertices.Indices, BoxVertices.TexturePath);
            }
            else if (g == 2)
            {
                mesh = new Mesh(FloorVertices.Vertices, FloorVertices.Indices, FloorVertices.TexturePath);
            }
            else if (g == 3)
            {
                mesh = new Mesh(TntBlockVertices.Vertices, TntBlockVertices.Indices, TntBlockVertices.TexturePath);
            }
            else if (g == 4)
            {
                mesh = new Mesh(TableVertices.Vertices, TableVertices.Indices, TableVertices.TexturePath);
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

        public virtual void Draw() => mesh.Draw(pipeline.getMVP().ToOpenTK());

        public virtual void Draw(matrix4f cameraPos, matrix4f cameraRot, matrix4f PersProj) => mesh.Draw(pipeline.getMVP().ToOpenTK(), cameraPos.ToOpenTK(), cameraRot.ToOpenTK(), PersProj.ToOpenTK());

        public virtual void Draw(int dist) => mesh.Draw(pipeline.getMVP().ToOpenTK(), Check_Distance(dist));

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

        public virtual void OnDelete() => mesh.Dispose();
    }
}
