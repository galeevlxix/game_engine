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

        public void PrintTexturesMap()
        {
            Console.WriteLine("Meshes Maps Contains...");
            _Print("DiffusePath", _Paths._DiffusePath);
            _Print("SpecularPath", _Paths._SpecularPath);
            _Print("NormalPath", _Paths._NormalPath);
            _Print("HeightMap", _Paths._HeightPath);
            _Print("MetallicPath", _Paths._MetallicPath);
            _Print("RoughnnesPath", _Paths._RoughnnesPath);
            _Print("LightMap", _Paths._LightMap);
            _Print("EmissivePath", _Paths._EmissivePath);
            _Print("AmbientOcclusionPath", _Paths._AmbientOcclusionPath);
            Console.WriteLine("\n -------------------------------------------------------------- \n");
        }

        private void _Print(string TypeTexture, string pathTex)
        {
            if (pathTex != string.Empty)
                Console.WriteLine($"{TypeTexture} : {pathTex}");
        }
    }
}
