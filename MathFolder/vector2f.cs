using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game_2.MathFolder
{
    public class vector2f
    {
        public float x;
        public float y;

        public vector2f(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public vector2f() 
        {
            x = 0;
            y = 0;
        }

        public static vector2f Zero
        {
            get
            {
                return new vector2f(0, 0);
            }
        }
        public static vector2f One
        {
            get
            {
                return new vector2f(1, 1);
            }
        }
    }
}
