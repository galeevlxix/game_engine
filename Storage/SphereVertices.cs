using game_2.MathFolder;

namespace game_2.Storage
{
    public static class SphereVertices
    {
        private static int gx_segments = 10;
        private static int gy_segments = 10;
        private static float PI = math3d.PI;
        public static void InitSegments(int SegmentsCount)
        {
            gx_segments = SegmentsCount;
            gy_segments = SegmentsCount;
        }

        public static float[] GetVertices()
        {
            List<float> vertices = new List<float>();
            for (int y = 0; y <= gy_segments; y++)
            {
                for (int x = 0; x <= gx_segments; x++)
                {
                    float xSegment = (float)x / (float)gx_segments;
                    float ySegment = (float)y / (float)gy_segments;
                    float xPos = math3d.cos(xSegment * 2.0f * PI) * math3d.sin(ySegment * PI);
                    float yPos = math3d.cos(ySegment * PI);
                    float zPos = math3d.sin(xSegment * 2.0f * PI) * math3d.sin(ySegment * PI);
                    vertices.Add(xPos);
                    vertices.Add(yPos);
                    vertices.Add(zPos);
                }
            }
            return vertices.ToArray();
        }

        public static int[] GetIndices()
        {
            List <int> indices = new List<int>();
            for (int i = 0; i < gy_segments; i++)
            {
                for (int j = 0; j < gx_segments; j++)
                {
                    indices.Add(i * (gx_segments + 1) + j);
                    indices.Add((i + 1) * (gx_segments + 1) + j);
                    indices.Add((i + 1) * (gx_segments + 1) + j + 1);
                    indices.Add(i * (gx_segments + 1) + j);
                    indices.Add((i + 1) * (gx_segments + 1) + j + 1);
                    indices.Add(i * (gx_segments + 1) + j + 1);
                }
            }
            return indices.ToArray();
        }
    }
}
