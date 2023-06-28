using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game_2.Brain
{
    public class GameObj
    {
        private Mesh mesh;
        private Shader shader;
        private Pipeline pipeline;

        public GameObj()
        {
            Storage stor = new Storage();
            mesh = new Mesh();
            shader = new Shader(stor.vertexShader, stor.fragmentShader);
            pipeline = new Pipeline();
            Position(0, 0, -2);
            Rotate(0, 0, 0);
            Scale(0, 0, 0);
            pipeline.mPersProj = stor.GetPersProj;
        }

        public GameObj(string file_name)
        {
            Storage stor = new Storage();
            mesh = new Mesh(file_name);
            shader = new Shader(stor.vertexShader, stor.fragmentShader);
            pipeline = new Pipeline();
            Position(0, 0, -2);
            Rotate(0, 0, 0);
            Scale(0, 0, 0);
            pipeline.mPersProj = stor.GetPersProj;
        }

        public void Draw()
        {
            shader.setMatrix(pipeline.getMVP());
            shader.Use();
            mesh.Draw();
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
