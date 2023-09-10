using game_2.Brain.ObjectFolder;
using game_2.FileManagers;
using OpenTK.Graphics.OpenGL4;

namespace game_2.Brain.AssimpFolder
{
    public class AssimpObject : GameObj
    {
        private AssimpMesh assimpModel;
        private List<MeshEntry> meshes;
        private Dictionary<string, Texture> TextureMap = new Dictionary<string, Texture>();

        public AssimpObject(string modelPath)
        {
            assimpModel = new AssimpMesh(modelPath, false);
            meshes = new List<MeshEntry>(assimpModel.meshes);
            pipeline = new Pipeline();

            foreach(MeshEntry m in meshes)
            {
                LoadTextures(m._Paths._DiffusePath, PixelInternalFormat.Rgba, TextureUnit.Texture0);
                LoadTextures(m._Paths._NormalPath, PixelInternalFormat.Rgba, TextureUnit.Texture1);
                LoadTextures(m._Paths._LightMap, PixelInternalFormat.Rgba, TextureUnit.Texture2);
                LoadTextures(m._Paths._EmissivePath, PixelInternalFormat.SrgbAlpha, TextureUnit.Texture3);
                LoadTextures(m._Paths._SpecularPath, PixelInternalFormat.Rgba, TextureUnit.Texture4);
                LoadTextures(m._Paths._HeightPath, PixelInternalFormat.Rgba, TextureUnit.Texture5);
                LoadTextures(m._Paths._MetallicPath, PixelInternalFormat.Rgba, TextureUnit.Texture6);
                LoadTextures(m._Paths._RoughnnesPath, PixelInternalFormat.Rgba, TextureUnit.Texture7);
                LoadTextures(m._Paths._AmbientOcclusionPath, PixelInternalFormat.Rgba, TextureUnit.Texture8);
            }

            GL.Enable(EnableCap.DepthTest);
        }

        public override void Draw()
        {
            CentralizedShaders.ObjectShader.setMatrices(pipeline.getMVP().ToOpenTK());

            foreach (var item in meshes)
            {
                TextureMap[item._Paths._DiffusePath].Use();
                item.Render();
            }
        }

        private void LoadTextures(string tex_path, PixelInternalFormat pixelFormat, TextureUnit unit)
        {
            if (unit == TextureUnit.Texture1 && tex_path != "")
            {

            }

            if (!TextureMap.ContainsKey(tex_path))
            {
                if (tex_path != string.Empty)
                {
                    Texture _texture_map = Texture.Load(tex_path, pixelFormat, unit);
                    TextureMap.Add(tex_path, _texture_map);
                }
            }
        }

        public override void OnDelete()
        {
            for (int i = 0; i < meshes.Count; i++)
                meshes[i].Dispose();

            foreach (var index in TextureMap.Keys)
                TextureMap[index].Dispose();
        }
    }
}
