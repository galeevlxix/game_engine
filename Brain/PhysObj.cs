using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game_2.Brain
{
    public class PhysObj : GameObj
    {
        public vector3 velocity { get; set; }   //скорость

        public PhysObj()
        {
            Reset();
        }

        public override void Reset()
        {
            base.Reset();
            velocity = new vector3(0, 0, 0);
        }

        public override void Update(MouseState mouse, KeyboardState keyboard)
        {
            velocity *= (1.0f - 0.0009f);
            pipeline.PositionVector += velocity * GameTime.Delta;
        }
    }
}
