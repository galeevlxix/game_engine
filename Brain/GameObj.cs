using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
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
        protected Pipeline pipeline;

        public GameObj()
        {
            mesh = new Mesh();
            shader = new Shader(Storage.vertexShader, Storage.fragmentShader);
            pipeline = new Pipeline();
            Position(0, 0, -2);
            Rotate(0, 0, 0);
            Scale(0, 0, 0);
            //pipeline.mPersProj = stor.GetPersProj;
        }

        public GameObj(string file_name)
        {
            mesh = new Mesh(file_name);
            shader = new Shader(Storage.vertexShader, Storage.fragmentShader);
            pipeline = new Pipeline();
            Position(0, 0, -2);
            Rotate(0, 0, 0);
            Scale(0, 0, 0);
            //pipeline.mPersProj = stor.GetPersProj;
        }

        public void Draw(Camera cam)
        {
            shader.setMatrix(pipeline.getMVP_without_proj() * cam.getMatrix);
            shader.Use();
            mesh.Draw();
        }

        public virtual void Update(MouseState mouse, KeyboardState keyboard)
        {
            
        }

        public virtual void Reset()
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
        
        public Matrix4 LocalToWorld()
        {
            return pipeline.getMVP_without_proj();
        }

        public Matrix4 WorldToLocal()
        {
            Matrix4 v = Pipeline.WorldToLocal(pipeline);
            return v;
        }
    }
}
