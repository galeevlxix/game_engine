namespace game_2.Storage
{
    public class FloorVertices
    {
        public static readonly float[] Vertices = new float[]
        {   //cords     //textures  //normals
            -3, 0, -3,  0,  0,      0, 1, 0,
             3, 0, -3,  1,  0,      0, 1, 0,
             3, 0,  3,  1,  1,      0, 1, 0,
            -3, 0,  3,  0,  1,      0, 1, 0,
        };

        public static readonly int[] Indices = new int[]
        {
            0, 1, 2,
            0, 2, 3,
        };

        public static readonly string TexturePath = "C:\\Users\\Lenovo\\source\\repos\\game_2\\Files\\Textures\\summ_grass8-xt.png";
    }
}
