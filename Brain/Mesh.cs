using OpenTK.Graphics.OpenGL;
using game_2.Storage;
using System.Text.RegularExpressions;
using System.Globalization;
using game_2.FileManagers;

namespace game_2.Brain
{
    public class Mesh : IDisposable
    {
        private int VBO { get; set; }
        private int VAO { get; set; }
        private int IBO { get; set; }

        private float[] Vertices { get; set; }
        private int[] Indices { get; set; }

        public Mesh()
        {
            Vertices = Cube.cubeVertices;
            Indices = Cube.cubeIndices;

            Load();
        }

        public Mesh(string file_name)
        {
            ModelLoader.LoadMesh(file_name, out float[] Vertices, out int[] Indices);
            this.Vertices = Vertices;
            this.Indices = Indices;
            Load();
        }

        private void Load()
        {
            VBO = GL.GenBuffer();
            VAO = GL.GenVertexArray();
            GL.BindVertexArray(VAO);
            IBO = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, Vertices.Length * sizeof(float), Vertices, BufferUsageHint.DynamicDraw);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0); // 3 * sizeof(float)

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, IBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, Indices.Length * sizeof(uint), Indices, BufferUsageHint.DynamicDraw);

            GL.BindVertexArray(0);

            GL.CullFace(CullFaceMode.Front);
            GL.Enable(EnableCap.DepthTest);
        }

        public void Draw()
        {
            GL.BindVertexArray(VAO);
            GL.DrawElements(PrimitiveType.Triangles, Indices.Length, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(1);
        }

        public void Dispose()
        {
            GL.DeleteBuffer(VBO);
            GL.DeleteVertexArray(VAO);
        }

        
    }
}
