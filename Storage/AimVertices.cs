using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game_2.Storage
{
    public static class AimVertices
    {
        public static readonly float[] Vertices = new float[]
        {   //cords             //textures
            -0.1f,  0.5f, 0,    0, 1,   
             0.1f,  0.5f, 0,    1, 1,
             0.1f, -0.5f, 0,    1, 0,
            -0.1f, -0.5f, 0,    0, 0,

             0.5f,  0.1f, 0,    0, 1,
             0.5f, -0.1f, 0,    1, 1,
            -0.5f, -0.1f, 0,    1, 0,
            -0.5f,  0.1f, 0,    0, 0,
        };

        public static readonly int[] Indices = new int[]
        {
            0, 1, 3,
            1, 2, 3,

            4, 5, 7,
            5, 6, 7
        };

        public static readonly string TexturePath = "C:\\Users\\Lenovo\\source\\repos\\game_2\\Textures\\ain.png";
    }
}
