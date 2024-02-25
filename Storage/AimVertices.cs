namespace game_2.Storage
{
    public static class AimVertices
    {
        public static readonly float[] Vertices = new float[]
        {   //cords             //textures
            -1f,  1f, 0,    0, 1,
             1f,  1f, 0,    1, 1,
             1f, -1f, 0,    1, 0,
            -1f, -1f, 0,    0, 0,
        };

        public static readonly int[] Indices = new int[]
        {
            0, 1, 3,
            1, 2, 3,
        };

        public static readonly string TexturePath = "C:\\Users\\Lenovo\\source\\repos\\game_2\\Files\\Textures\\cross2.png";
    }
}
