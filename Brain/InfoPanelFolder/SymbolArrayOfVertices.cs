using OpenTK.Graphics.OpenGL4;

namespace game_2.Brain.InfoPanelFolder
{
    public static class SymbolArrayOfVertices
    {
        private static float[] cords = new float[]
        {
            //cords
            -1, -1,  0,
            -1,  1,  0,
             1,  1,  0,
             1, -1,  0
        };

        private static float stepScale = 0.1f;

        private static int[] inds 
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

        public static void GetVertices(int col, int row, out float[] vertices, out int[] indices)
        {
            vertices = new float[20];

            indices = inds;

            float[] textures = new float[]
            {
                stepScale * col,                stepScale * row,
                stepScale * col,                stepScale * row + stepScale,
                stepScale * col + stepScale,    stepScale * row + stepScale,
                stepScale * col + stepScale,    stepScale * row,
            };

            for (int i = 0; i < 4; i++)
            {
                vertices[i * 5] =     cords[i * 3];
                vertices[i * 5 + 1] = cords[i * 3 + 1];
                vertices[i * 5 + 2] = cords[i * 3 + 2];

                vertices[i * 5 + 3] = textures[i * 2];
                vertices[i * 5 + 4] = textures[i * 2 + 1];
            }
        }

        public static Texture texture { get; private set; }

        public static void LoadTexture(string path)
        {
            texture = Texture.Load(
                path,
                PixelInternalFormat.Rgba,
                TextureUnit.Texture0,
                false);
        }
    }
}
