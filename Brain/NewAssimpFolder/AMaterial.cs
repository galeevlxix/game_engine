using game_2.Brain.Lights;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game_2.Brain.NewAssimpFolder
{
    public class AMaterial
    {
        private Dictionary<string, Texture> m_textures;
        private float m_specular_power;

        private AMaterial()
        {
            m_textures = new Dictionary<string, Texture>();
            m_specular_power = 32.0f;
        }

        public static AMaterial Init(ModelTexturePaths paths)
        {
            AMaterial material = new AMaterial();
            material.LoadTextures(paths);

            return material;
        }

        public void Use()
        {
            UseTextures();
            LightningManager.lightConfig.SetMatSpecularPower(m_specular_power);
        }

        private void UseTextures()
        {
            foreach (Texture texture in m_textures.Values) texture.Use();
        }
        
        private void LoadTextures(ModelTexturePaths paths)
        {
            LoadOneMap(paths._DiffusePath, TextureUnit.Texture0);
            LoadOneMap(paths._NormalPath, TextureUnit.Texture1);
        }

        private void LoadOneMap(string file_path, TextureUnit unit)
        {
            if (file_path != string.Empty && !m_textures.ContainsKey(file_path))
            {
                Texture _texture_map = Texture.Load(file_path, PixelInternalFormat.Rgba, unit, true);
                m_textures.Add(file_path, _texture_map);
            }
            else if (file_path == string.Empty && !m_textures.ContainsKey(file_path) && unit == TextureUnit.Texture1)
            {
                string empty_normal_map = "C:\\Users\\Lenovo\\source\\repos\\game_2\\Files\\Textures\\white_list.bmp";
                Texture _texture_map = Texture.Load(empty_normal_map, PixelInternalFormat.Rgba, unit, true);
                m_textures.Add(empty_normal_map, _texture_map);
            }
        }

        public void SetSpecularPower(float value)
        {
            m_specular_power = value;
        }
    }
}
