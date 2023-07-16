namespace game_2.Storage
{
    public static class Cube
    {
        public static readonly float[] Vertices = new float[]
        {    //куб
                  0.5f, -0.5f, -0.5f, 0, 0,
                  0.5f, -0.5f,  0.5f, 0, 1,
                 -0.5f, -0.5f,  0.5f, 1, 1,
                 -0.5f, -0.5f, -0.5f, 1, 0,
                  0.5f,  0.5f, -0.5f, 0, 0,
                  0.5f,  0.5f,  0.5f, 0, 1,
                 -0.5f,  0.5f,  0.5f, 1, 1,
                 -0.5f,  0.5f, -0.5f, 1, 0
                };

        public static readonly int[] Indices = new int[]
        {
                0,1,2, // передняя сторона
                2,3,0,

                6,5,4, // задняя сторона
                4,7,6,

                4,0,3, // левый бок
                3,7,4,

                1,5,6, // правый бок
                6,2,1,

                4,5,1, // вверх
                1,0,4,

                3,2,6, // низ
                6,7,3
        };

        public static readonly float[] cubeTextCords = new float[]
        {

        };

        public static readonly string TexturePath = "C:\\Users\\Lenovo\\source\\repos\\game_2\\Textures\\tnt_texture.png";
    }
}
