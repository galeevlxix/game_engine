namespace game_2.Storage
{
    public static class Box
    {
        public static readonly float[] Vertices = new float[]
        {           //куб
                    -1.0f, 1.0f, 1.0f,      0.0f, 1.0f,
                    1.0f, 1.0f, 1.0f,       1.0f, 1.0f,
                    -1.0f, -1.0f, 1.0f,     0.0f, 0.0f,
                    1.0f, -1.0f, 1.0f,      1.0f, 0.0f,

                    1.0f, 1.0f, 1.0f,       0.0f, 1.0f,
                    1.0f, 1.0f, -1.0f,      1.0f, 1.0f,
                    1.0f, -1.0f, 1.0f,      0.0f, 0.0f,
                    1.0f, -1.0f, -1.0f,     1.0f, 0.0f,

                    1.0f, 1.0f, -1.0f,      0.0f, 1.0f,
                    -1.0f, 1.0f, -1.0f,     1.0f, 1.0f,
                    1.0f, -1.0f, -1.0f,     0.0f, 0.0f,
                    -1.0f, -1.0f, -1.0f,    1.0f, 0.0f,

                    -1.0f, 1.0f, -1.0f,     0.0f, 1.0f,
                    -1.0f, 1.0f, 1.0f,      1.0f, 1.0f,
                    -1.0f, -1.0f, -1.0f,    0.0f, 0.0f,
                    -1.0f, -1.0f, 1.0f,     1.0f, 0.0f,

                    -1.0f, 1.0f, -1.0f,     0.0f, 1.0f,
                    1.0f, 1.0f, -1.0f,      1.0f, 1.0f,
                    -1.0f, 1.0f, 1.0f,      0.0f, 0.0f,
                    1.0f, 1.0f, 1.0f,       1.0f, 0.0f,

                    -1.0f, -1.0f, 1.0f,     0.0f, 1.0f,
                    1.0f, -1.0f, 1.0f,      1.0f, 1.0f,
                    -1.0f, -1.0f, -1.0f,    0.0f, 0.0f,
                    1.0f, -1.0f, -1.0f,     1.0f, 0.0f,
        };

        public static readonly int[] Indices = new int[]
        {
                        // äàëüíÿÿ
                        9, 11, 8,
                        8, 11, 10,

                        // áëèæíÿÿ
                        1, 3, 0,
                        0, 3, 2,

                        // ïðàâàÿ
                        5, 7, 4,
                        4, 7, 6,

                        // ëåâàÿ
                        13, 15, 12,
                        12, 15, 14,

                        // íèæíÿÿ
                        22, 20, 23,
                        23, 20, 21,

                        // âåðõíÿÿ
                        17, 19, 16,
                        16, 19, 18
        };

        public static readonly string TexturePath = "C:\\Users\\Lenovo\\source\\repos\\game_2\\Textures\\container.png";
    }
}
