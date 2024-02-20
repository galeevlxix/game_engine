﻿namespace game_2.Storage
{
    public static class TrianglesVertices
    {
        public static readonly float[] Vertices =
        {
            // Position         Texture coordinates
             0.5f,  0.5f, 0.0f, 1.0f, 1.0f, // top right
             0.5f, -0.5f, 0.0f, 1.0f, 0.0f, // bottom right
            -0.5f, -0.5f, 0.0f, 0.0f, 0.0f, // bottom left
            -0.5f,  0.5f, 0.0f, 0.0f, 1.0f  // top left 
        };

        public static readonly int[] Indices =
        {
            0, 1, 3,
            1, 2, 3
        };

        public static readonly string TexturePath = "C:\\Users\\Lenovo\\source\\repos\\game_2\\Textures\\container.png";
    }
}