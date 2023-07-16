using game_2.MathFolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OpenTK.Graphics.OpenGL.GL;

namespace game_2.Brain
{
    public class Aim : GameObj
    {
        public Aim() : base()
        {
            pipeline.Position(0, 0, -1);
            pipeline.Scale(0.1f);
        }

        public override void Draw()
        {
            mesh.shader.setMatrix(matrix4f.ToFloatArray(pipeline.GetCursorTranformation()));
            mesh.shader.Use();
            mesh.Draw();
        }
    }
}
