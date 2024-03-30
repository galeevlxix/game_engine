using game_2.Brain.Lights;
using OpenTK.Graphics.OpenGL4;

namespace game_2.Brain.NewAssimpFolder
{
    public class AMaterial
    {
        private ModelTexturePaths m_paths;
        private float m_specular_power;

        private AMaterial()
        {
            m_paths = new ModelTexturePaths();
        }

        public static AMaterial Init(ModelTexturePaths paths)
        {
            AMaterial material = new AMaterial();
            material.m_paths = paths;
            material.LoadTextures();

            return material;
        }

        public void Use()
        {
            UseTextures();
            LightningManager.lightConfig.SetMatSpecularPower(m_specular_power);
        }

        private void UseTextures()
        {
            TextureHeap.Use(m_paths);
        }
        
        private void LoadTextures()
        {
            //missing maps
            if (m_paths._NormalPath == string.Empty) m_paths._NormalPath = TextureHeap.empty_normal_map;
            if (m_paths._SpecularPath == string.Empty) m_paths._SpecularPath = TextureHeap.empty_specular_map;

            //add
            TextureHeap.Add(m_paths._DiffusePath);
            TextureHeap.Add(m_paths._NormalPath, TextureUnit.Texture1);
            TextureHeap.Add(m_paths._SpecularPath, TextureUnit.Texture2);
        }

        public void SetSpecularPower(float value)
        {
            m_specular_power = value;
        }
    }
}
