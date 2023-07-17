namespace game_2.Storage
{
    public static class Cube
    {
        public static readonly float[] Vertices = new float[]
        {    //куб
                    -1.0f, 1.0f, 1.0f,      0.499f, 0.6666f,
                    1.0f, 1.0f, 1.0f,       0.251f, 0.6666f,
                    -1.0f, -1.0f, 1.0f,     0.499f, 1.0f,
                    1.0f, -1.0f, 1.0f,      0.251f, 1.0f,
        };

        public static readonly int[] Indices = new int[]
        {
                        // áëèæíÿÿ
                        1, 3, 0,
                        0, 3, 2,
        };

        public static readonly float[] cubeTextCords = new float[]
        {

        };

        public static readonly string TexturePath = "C:\\Users\\Lenovo\\source\\repos\\game_2\\Textures\\tnt_texture.png";
    }
}
