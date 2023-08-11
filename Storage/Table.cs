namespace game_2.Storage
{
    public static class Table
    {
        public static readonly float[] Vertices = new float[]
        {            //cords                 //textures    
                    -1.0f,  1.0f,  1.0f,     0, 1,        0.0f, 0.0f, 1.0f,   //ближний!
                     1.0f,  1.0f,  1.0f,     1, 1,        0.0f, 0.0f, 1.0f,
                    -1.0f, -1.0f,  1.0f,     0, 0,        0.0f, 0.0f, 1.0f,
                     1.0f, -1.0f,  1.0f,     1, 0,        0.0f, 0.0f, 1.0f,
        };
        public static readonly int[] Indices = new int[]
        {
                        1, 3, 0,
                        0, 3, 2,
        };

        public static readonly string TexturePath = "C:\\Users\\Lenovo\\source\\repos\\game_2\\Textures\\minecraft_font.png";
    }
}
