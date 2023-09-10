using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;

namespace game_2.Brain.AssimpFolder
{
    public class MeshEntry : IDisposable
    {
        private int _IndicesCount;
        public ModelTexturePaths _Paths;

        private VertexArrayObject VAO;
        private BufferObject<Vertex> VBO;
        private BufferObject<int> IBO;

        public unsafe MeshEntry(List<Vertex> Vertices, List<int> Indices, ModelTexturePaths Paths)
        {
            _IndicesCount = Indices.Count;
            _Paths = Paths;

            VAO = new VertexArrayObject();
            VBO = new BufferObject<Vertex>(Vertices.ToArray(), BufferTarget.ArrayBuffer);
            VAO.LinkBufferObject(ref VBO);

            VAO.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, sizeof(Vertex), IntPtr.Zero);
            VAO.VertexAttributePointer(2, 3, VertexAttribPointerType.Float, sizeof(Vertex), Marshal.OffsetOf(typeof(Vertex), "Normal"));
            VAO.VertexAttributePointer(1, 2, VertexAttribPointerType.Float, sizeof(Vertex), Marshal.OffsetOf(typeof(Vertex), "Tex"));
            VAO.VertexAttributePointer(3, 3, VertexAttribPointerType.Float, sizeof(Vertex), Marshal.OffsetOf(typeof(Vertex), "Tangent"));
            VAO.VertexAttributePointer(4, 3, VertexAttribPointerType.Float, sizeof(Vertex), Marshal.OffsetOf(typeof(Vertex), "Bitangent"));

            IBO = new BufferObject<int>(Indices.ToArray(), BufferTarget.ElementArrayBuffer);
            VAO.LinkBufferObject(ref IBO);

            Vertices.Clear();
            Indices.Clear();
        }
        
        public void Render()
        {
            VAO.Bind();
            GL.DrawElements(PrimitiveType.Triangles, _IndicesCount, DrawElementsType.UnsignedInt, 0);
        }

        public void Dispose() => VAO.Dispose();
    }
}
