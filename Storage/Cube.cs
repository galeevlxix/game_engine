namespace game_2.Storage
{
    public static class Cube
    {
        public static float[] cubeVertices = new float[]{    //куб
                  0.5f, -0.5f, -0.5f,
                  0.5f, -0.5f,  0.5f,
                 -0.5f, -0.5f,  0.5f,
                 -0.5f, -0.5f, -0.5f,
                  0.5f,  0.5f, -0.5f,
                  0.5f,  0.5f,  0.5f,
                 -0.5f,  0.5f,  0.5f,
                 -0.5f,  0.5f, -0.5f
                };
        public static int[] cubeIndices = new int[]
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
    }
}
