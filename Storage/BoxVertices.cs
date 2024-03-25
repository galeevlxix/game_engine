namespace game_2.Storage
{
    public static class BoxVertices
    {
        public static readonly float[] Vertices = new float[]
        {           //cords                 //textures      //normals
                    -1.0f, 1.0f, 1.0f,      0.0f, 1.0f,     0.0f, 0.0f, 1.0f,
                    1.0f, 1.0f, 1.0f,       1.0f, 1.0f,     0.0f, 0.0f, 1.0f,
                    -1.0f, -1.0f, 1.0f,     0.0f, 0.0f,     0.0f, 0.0f, 1.0f,
                    1.0f, -1.0f, 1.0f,      1.0f, 0.0f,     0.0f, 0.0f, 1.0f,

                    1.0f, 1.0f, 1.0f,       0.0f, 1.0f,     1.0f, 0.0f, 0.0f,
                    1.0f, 1.0f, -1.0f,      1.0f, 1.0f,     1.0f, 0.0f, 0.0f,
                    1.0f, -1.0f, 1.0f,      0.0f, 0.0f,     1.0f, 0.0f, 0.0f,
                    1.0f, -1.0f, -1.0f,     1.0f, 0.0f,     1.0f, 0.0f, 0.0f,

                    1.0f, 1.0f, -1.0f,      0.0f, 1.0f,     0.0f, 0.0f, -1.0f,
                    -1.0f, 1.0f, -1.0f,     1.0f, 1.0f,     0.0f, 0.0f, -1.0f,
                    1.0f, -1.0f, -1.0f,     0.0f, 0.0f,     0.0f, 0.0f, -1.0f,
                    -1.0f, -1.0f, -1.0f,    1.0f, 0.0f,     0.0f, 0.0f, -1.0f,

                    -1.0f, 1.0f, -1.0f,     0.0f, 1.0f,     -1.0f, 0.0f, 0.0f,
                    -1.0f, 1.0f, 1.0f,      1.0f, 1.0f,     -1.0f, 0.0f, 0.0f,
                    -1.0f, -1.0f, -1.0f,    0.0f, 0.0f,     -1.0f, 0.0f, 0.0f,
                    -1.0f, -1.0f, 1.0f,     1.0f, 0.0f,     -1.0f, 0.0f, 0.0f,

                    -1.0f, 1.0f, -1.0f,     0.0f, 1.0f,     0.0f, 1.0f, 0.0f,
                    1.0f, 1.0f, -1.0f,      1.0f, 1.0f,     0.0f, 1.0f, 0.0f,
                    -1.0f, 1.0f, 1.0f,      0.0f, 0.0f,     0.0f, 1.0f, 0.0f,
                    1.0f, 1.0f, 1.0f,       1.0f, 0.0f,     0.0f, 1.0f, 0.0f,

                    -1.0f, -1.0f, 1.0f,     0.0f, 1.0f,     0.0f, -1.0f, 0.0f,
                    1.0f, -1.0f, 1.0f,      1.0f, 1.0f,     0.0f, -1.0f, 0.0f,
                    -1.0f, -1.0f, -1.0f,    0.0f, 0.0f,     0.0f, -1.0f, 0.0f,
                    1.0f, -1.0f, -1.0f,     1.0f, 0.0f,     0.0f, -1.0f, 0.0f,
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

        public static readonly string DiffusePath = "..\\..\\..\\Files\\Textures\\container.png";
        public static readonly string NormalPath = "..\\..\\..\\Files\\Textures\\empty_normal2.png";
    }
}
