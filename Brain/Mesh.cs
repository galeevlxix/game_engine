using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

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
            Vertices = new Storage().cubeVertices;
            Indices = new Storage().cubeIndices;

            Load();
        }

        public Mesh(string file_name)
        {

        }

        private void Load()
        {
            VBO = GL.GenBuffer();
            VAO = GL.GenVertexArray();
            IBO = GL.GenBuffer();

            GL.BindVertexArray(VAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, Vertices.Length * sizeof(float), Vertices, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, IBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, Indices.Length * sizeof(uint), Indices, BufferUsageHint.StaticDraw);
            
            GL.EnableVertexAttribArray(0);

            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
        }

        public void Draw()
        {
            GL.BindVertexArray(VAO);
            GL.DrawElements(PrimitiveType.Triangles, Indices.Length, DrawElementsType.UnsignedInt, 0);
        }

        public void Dispose()
        {
            GL.DeleteBuffer(VBO);
            GL.DeleteVertexArray(VAO);
        }
    }
}
