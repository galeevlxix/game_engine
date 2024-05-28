using OpenTK.Graphics.OpenGL4;

namespace game_2.Brain.PictureOnScreen
{
    public static class PictureArrayOfvertices
    {
        public static float[] cords = new float[]
        {
            //cords
            -1f,  1f, 0,    0, 1,
             1f,  1f, 0,    1, 1,
             1f, -1f, 0,    1, 0,
            -1f, -1f, 0,    0, 0,
        };
        public static int[] inds
        {
            get
            {
                return new int[]
                {
                    0, 1, 3,
                    1, 2, 3
                };
            }
        }
    }
}
