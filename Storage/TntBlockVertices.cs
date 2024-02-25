namespace game_2.Storage
{
    public static class TntBlockVertices
    {
        public static readonly float[] Vertices = new float[]
        {            //cords                 //textures    
                    -1.0f,  1.0f,  1.0f,     1.0f,  2f / 3f,        0.0f, 0.0f, 1.0f,   //ближний!
                     1.0f,  1.0f,  1.0f,     0.75f, 2f / 3f,        0.0f, 0.0f, 1.0f,
                    -1.0f, -1.0f,  1.0f,     1.0f,  1f / 3f,        0.0f, 0.0f, 1.0f,
                     1.0f, -1.0f,  1.0f,     0.75f, 1f / 3f,        0.0f, 0.0f, 1.0f,

                     1.0f,  1.0f,  1.0f,     0.75f, 2f / 3f,        1.0f, 0.0f, 0.0f,   //правый!
                     1.0f,  1.0f, -1.0f,     0.5f,  2f / 3f,        1.0f, 0.0f, 0.0f,
                     1.0f, -1.0f,  1.0f,     0.75f, 1f / 3f,        1.0f, 0.0f, 0.0f,
                     1.0f, -1.0f, -1.0f,     0.5f,  1f / 3f,        1.0f, 0.0f, 0.0f,

                     1.0f,  1.0f, -1.0f,     0.5f,  2f / 3f,        0.0f, 0.0f, -1.0f,  //дальний!
                    -1.0f,  1.0f, -1.0f,     0.25f, 2f / 3f,        0.0f, 0.0f, -1.0f,
                     1.0f, -1.0f, -1.0f,     0.5f,  1f / 3f,        0.0f, 0.0f, -1.0f,
                    -1.0f, -1.0f, -1.0f,     0.25f, 1f / 3f,        0.0f, 0.0f, -1.0f,

                    -1.0f,  1.0f, -1.0f,     0.25f, 2f / 3f,        -1.0f, 0.0f, 0.0f,       //левый!
                    -1.0f,  1.0f,  1.0f,     0.0f,  2f / 3f,        -1.0f, 0.0f, 0.0f,
                    -1.0f, -1.0f, -1.0f,     0.25f, 1f / 3f,        -1.0f, 0.0f, 0.0f,
                    -1.0f, -1.0f,  1.0f,     0.0f,  1f / 3f,        -1.0f, 0.0f, 0.0f,

                    -1.0f,  1.0f, -1.0f,     0.25f, 2f / 3f,        0.0f, 1.0f, 0.0f,       //верхний!
                     1.0f,  1.0f, -1.0f,     0.5f,  2f / 3f,        0.0f, 1.0f, 0.0f,
                    -1.0f,  1.0f,  1.0f,     0.25f, 1.0f,           0.0f, 1.0f, 0.0f,
                     1.0f,  1.0f,  1.0f,     0.5f,  1.0f,           0.0f, 1.0f, 0.0f,

                    -1.0f, -1.0f,  1.0f,     0.25f, 0.0f,           0.0f, -1.0f, 0.0f,           //нижний!
                     1.0f, -1.0f,  1.0f,     0.5f,  0.0f,           0.0f, -1.0f, 0.0f,
                    -1.0f, -1.0f, -1.0f,     0.25f, 1f / 3f,        0.0f, -1.0f, 0.0f,
                     1.0f, -1.0f, -1.0f,     0.5f,  1f / 3f,        0.0f, -1.0f, 0.0f,
        };
        public static readonly int[] Indices = new int[]
        {
                        9, 11, 8,
                        8, 11, 10,

                        1, 3, 0,
                        0, 3, 2,

                        5, 7, 4,
                        4, 7, 6,

                        13, 15, 12,
                        12, 15, 14,

                        22, 20, 23,
                        23, 20, 21,

                        17, 19, 16,
                        16, 19, 18
        };

        public static readonly string TexturePath = "C:\\Users\\Lenovo\\source\\repos\\game_2\\Files\\Textures\\tnt_texture.png";
    }
}
