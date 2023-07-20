using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game_2.Storage
{
    public class Floor
    {
        public static readonly float[] Vertices = new float[]
        {   //cords    //textures
            -3, 0, -3,  0, 0,
             3, 0, -3,  1, 0,
             3, 0,  3,  1,  1,
            -3, 0,  3,  0,  1
        };

        public static readonly int[] Indices = new int[]
        {
            0, 1, 2,
            0, 2, 3
        };

        public static readonly string TexturePath = "C:\\Users\\Lenovo\\source\\repos\\game_2\\Textures\\grass.png";
    }
}
