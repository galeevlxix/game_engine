using game_2.FileManagers;
using game_2.MathFolder;
using OpenTK.Mathematics;
using System.Reflection.Metadata;

namespace game_2.Brain
{
    public static class CentralizedShaders
    {
        private static string FolderPath = "..\\..\\..\\Files\\Shaders\\";

        private static Dictionary<ShaderName, Shader> m_shaders;
        private static ShaderName activeShader;

        public static void Load()
        {
            m_shaders = new Dictionary<ShaderName, Shader>();

            m_shaders.Add(ShaderName.ObjectShader,      
                new Shader(ShaderLoader.LoadVertexShader(), ShaderLoader.LoadFragmentShader()));
            m_shaders.Add(ShaderName.SkyBoxShader,      
                new Shader(ShaderLoader.LoadShader(FolderPath + "Skybox\\SkyboxVetexShader.hlsl"), ShaderLoader.LoadShader(FolderPath + "Skybox\\SkyboxFragShader.hlsl")));
            m_shaders.Add(ShaderName.ScreenShader,      
                new Shader(ShaderLoader.LoadShader(FolderPath + "ScreenStatic\\ScreenStaticVertexShader.hlsl"), ShaderLoader.LoadShader(FolderPath + "ScreenStatic\\ScreenStaticFragmentShader.hlsl")));
            m_shaders.Add(ShaderName.MonochromeShader,  
                new Shader(ShaderLoader.LoadShader(FolderPath + "MonochromeObject\\MonoVertexShader.hlsl"), ShaderLoader.LoadShader(FolderPath + "MonochromeObject\\MonoFragmentShader.hlsl")));
            m_shaders.Add(ShaderName.AssimpShader,      
                new Shader(ShaderLoader.LoadShader(FolderPath + "AssimpObject\\AssimpVertexShader.hlsl"), ShaderLoader.LoadShader(FolderPath + "AssimpObject\\AssimpFragmentShader.hlsl")));
            m_shaders.Add(ShaderName.ShadowShader,      
                new Shader(ShaderLoader.LoadShader(FolderPath + "Shadow\\ShadowVertexShader.hlsl"), ShaderLoader.LoadShader(FolderPath + "Shadow\\ShadowFragmentShader.hlsl")));
        }

        public static void Dispose()
        {
            foreach (Shader shader in m_shaders.Values) shader.Dispose();
            m_shaders.Clear();
        }

        public static void SetValue(ShaderName shaderName, string uniformName, int value)
        {
            UseShader(shaderName);
            m_shaders[shaderName].setValue(uniformName, value);
        }

        public static void SetValue(ShaderName shaderName, string uniformName, float value)
        {
            UseShader(shaderName);
            m_shaders[shaderName].setValue(uniformName, value);
        }

        public static void SetValue(ShaderName shaderName, string uniformName, vector3f value)
        {
            UseShader(shaderName);
            m_shaders[shaderName].setValue(uniformName, value);
        }

        public static void SetValue(ShaderName shaderName, string uniformName, Matrix4 value)
        {
            UseShader(shaderName);
            m_shaders[shaderName].setValue(uniformName, value);
        }

        public static void SetValue(ShaderName shaderName, Matrix4 world)
        {
            UseShader(shaderName);
            m_shaders[shaderName].setValue(world);
        }

        public static void SetValue(ShaderName shaderName, Matrix4 world, Matrix4 p)
        {
            UseShader(shaderName);
            m_shaders[shaderName].setValue(world, p);
        }

        public static void SetValue(ShaderName shaderName, Matrix4 world, Matrix4 c_rot, Matrix4 p)
        {
            UseShader(shaderName);
            m_shaders[shaderName].setValue(world, c_rot, p);
        }

        public static void UseShader(ShaderName shaderName)
        {
            if (shaderName == activeShader) return;
            m_shaders[shaderName].Use();
            activeShader = shaderName;
        }

        public static int GetAttribLocation(ShaderName shaderName, string attribName)
        {
            UseShader(shaderName);
            return m_shaders[shaderName].GetAttribLocation(attribName);
        }

        public static int GetUniformLocation(ShaderName shaderName, string uniformName)
        {
            UseShader(shaderName);
            return m_shaders[shaderName].GetUniformLocation(uniformName);
        }

        public static Shader GetShader(ShaderName shaderName)
        {
            return m_shaders[shaderName];
        }
    }
    public enum ShaderName
    {
        ObjectShader,
        SkyBoxShader,
        ScreenShader,
        MonochromeShader,
        AssimpShader,
        ShadowShader
    }
}



