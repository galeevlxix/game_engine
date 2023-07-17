using OpenTK.Graphics.OpenGL;
using game_2.Storage;
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
        private float[] TextCords { get; set; }

        public Shader shader;
        public Texture texture;

        public Mesh()
        {
            Vertices = Cube.Vertices;
            Indices = Cube.Indices;

            Load();
        }

        public Mesh(string file_name)
        {
            ModelLoader.LoadMesh(file_name, out float[] Vertices, out int[] Indices, out float[] TextCords);
            this.Vertices = Vertices;
            this.Indices = Indices;
            Load();
        }

        private void Load()
        {
            VAO = GL.GenVertexArray();
            GL.BindVertexArray(VAO);

            VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, Vertices.Length * sizeof(float), Vertices, BufferUsageHint.StaticDraw);

            IBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, IBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, Indices.Length * sizeof(uint), Indices, BufferUsageHint.StaticDraw);

            shader = new Shader(ShaderLoader.LoadVertexShader(), ShaderLoader.LoadFragmentShader());
            shader.Use();

            var location = shader.GetAttribLocation("aPosition");

            GL.EnableVertexAttribArray(location);
            GL.VertexAttribPointer(location, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0); // 3 * sizeof(float)

            var texCordLocation = shader.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCordLocation);
            GL.VertexAttribPointer(texCordLocation, 2, VertexAttribPointerType.Float, false, 3 * sizeof(float), 3 * sizeof(float)); ///////HUI

            texture = Texture.Load(Cube.TexturePath);
            texture.Use(TextureUnit.Texture0);

            GL.CullFace(CullFaceMode.Front);
            GL.Enable(EnableCap.DepthTest);
        }

        public void Draw()
        {
            GL.BindVertexArray(VAO);
            GL.DrawElements(PrimitiveType.Triangles, Indices.Length, DrawElementsType.UnsignedInt, 0);
            

            //texture.Use(TextureUnit.Texture0);
        }

        public void Dispose()
        {
            GL.DeleteBuffer(VBO);
            GL.DeleteVertexArray(VAO);
        }

        
    }
}
