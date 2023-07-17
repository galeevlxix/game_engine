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

        private string texPath;

        public Mesh()
        {
            Vertices = Cube.Vertices;
            Indices = Cube.Indices;
            texPath = Cube.TexturePath;

            Load();
        }

        public Mesh(string file_name, string tex_file_name)
        {
            ModelLoader.LoadMesh(file_name, out float[] Vertices, out int[] Indices, out float[] TextCords);
            this.Vertices = Vertices;
            this.Indices = Indices;
            texPath = tex_file_name;
            Load();
        }

        private void Load()
        {
            VAO = GL.GenVertexArray();
            GL.BindVertexArray(VAO);

            VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, Vertices.Length * sizeof(float), Vertices, BufferUsageHint.DynamicDraw);

            IBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, IBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, Indices.Length * sizeof(int), Indices, BufferUsageHint.DynamicDraw);

            shader = new Shader(ShaderLoader.LoadVertexShader(), ShaderLoader.LoadFragmentShader());

            var location = shader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(location);
            GL.VertexAttribPointer(location, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

            var texCordLocation = shader.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCordLocation);
            GL.VertexAttribPointer(texCordLocation, 2, VertexAttribPointerType.Float, false, 5  * sizeof(float), 3 * sizeof(float));

            texture = Texture.Load(texPath);

            GL.CullFace(CullFaceMode.Front);
            GL.Enable(EnableCap.DepthTest);
        }

        public void Draw()
        {
            GL.BindVertexArray(VAO);
            GL.DrawElements(PrimitiveType.Triangles, Indices.Length, DrawElementsType.UnsignedInt, 0);
            texture.Use(TextureUnit.Texture0);
        }

        public void Dispose()
        {
            GL.DeleteBuffer(VBO);
            GL.DeleteVertexArray(VAO);
        }
    }
}
