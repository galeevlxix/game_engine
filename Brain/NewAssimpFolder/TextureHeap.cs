using OpenTK.Graphics.OpenGL4;

namespace game_2.Brain.NewAssimpFolder
{
    public static class TextureHeap
    {
        private static Dictionary<string, Texture> _textureHeap = new Dictionary<string, Texture>();

        public static string empty_normal_map = "..\\..\\..\\Files\\Textures\\EmptyNormalMap.png";
        
        public static string empty_specular_map = "..\\..\\..\\Files\\Textures\\EmptySpecularMap.png";

        public static string empty_diffuse_map = "..\\..\\..\\Files\\Textures\\EmptyDiffuseMap.png";

        public static void Add(string file_path, TextureUnit unit = TextureUnit.Texture0, PixelInternalFormat format = PixelInternalFormat.Rgba)
        {
            if (!_textureHeap.ContainsKey(file_path))
            {
                _textureHeap.Add(file_path, Texture.Load(file_path, format, unit));
            }
        }

        public static void Use(string file_path)
        {
            _textureHeap[file_path].Use();
        }

        public static void Use(ModelTexturePaths paths)
        {
            _textureHeap[paths._DiffusePath].Use();
            _textureHeap[paths._NormalPath].Use();
            _textureHeap[paths._SpecularPath].Use();
        }
    }
}
