using game_2.Brain.Lights;
using game_2.Brain.Lights.LightStructures;
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
            Console.WriteLine("Загрузка шейдеров...");

            m_shaders = new Dictionary<ShaderName, Shader>() {
                {   // OBJECT SHADER
                    ShaderName.ObjectShader,
                    new Shader(
                        ShaderLoader.LoadVertexShader(),
                        ShaderLoader.LoadFragmentShader(),
                        ShaderName.ObjectShader)
                },
                {   // SKYBOX SHADER
                    ShaderName.SkyBoxShader,
                    new Shader(
                        ShaderLoader.LoadShader(FolderPath + "Skybox\\SkyboxVetexShader.hlsl"),
                        ShaderLoader.LoadShader(FolderPath + "Skybox\\SkyboxFragShader.hlsl"),
                        ShaderName.SkyBoxShader)
                },
                {   // STATIC SCREEN SHADER
                    ShaderName.ScreenShader,
                    new Shader(
                        ShaderLoader.LoadShader(FolderPath + "ScreenStatic\\ScreenStaticVertexShader.hlsl"),
                        ShaderLoader.LoadShader(FolderPath + "ScreenStatic\\ScreenStaticFragmentShader.hlsl"),
                        ShaderName.ScreenShader)
                },
                {   // MONOCHROME OBJECT SHADER
                    ShaderName.MonochromeShader,
                    new Shader(
                        ShaderLoader.LoadShader(FolderPath + "MonochromeObject\\MonoVertexShader.hlsl"),
                        ShaderLoader.LoadShader(FolderPath + "MonochromeObject\\MonoFragmentShader.hlsl"),
                        ShaderName.MonochromeShader)
                },
                {   // ASSIMP OBJECT SHADER
                    ShaderName.AssimpShader,
                    new Shader(
                        ShaderLoader.LoadShader(FolderPath + "AssimpObject\\AssimpVertexShader.hlsl"),
                        ShaderLoader.LoadShader(FolderPath + "AssimpObject\\AssimpFragmentShader.hlsl"),
                        ShaderName.AssimpShader)
                },
                {   // SHADOW MAP SHADER
                    ShaderName.ShadowShader,
                    new Shader(
                        ShaderLoader.LoadShader(FolderPath + "Shadow\\ShadowVertexShader.hlsl"),
                        ShaderLoader.LoadShader(FolderPath + "Shadow\\ShadowFragmentShader.hlsl"),
                        ShaderName.ShadowShader)
                },
                {   // SELECTING MAP SHADER
                    ShaderName.SelectingShader,
                    new Shader(
                        ShaderLoader.LoadShader(FolderPath + "Select\\SelectVertexSahder.hlsl"),
                        ShaderLoader.LoadShader(FolderPath + "Select\\SelectFragmentSahder.hlsl"),
                        ShaderName.SelectingShader)
                },
                {
                    //SHADOW CUBE MAP SHADER
                    ShaderName.ShadowCubeMapShader,
                    new Shader(
                        ShaderLoader.LoadShader(FolderPath + "ShadowCube\\ShadowCubeVertexShader.hlsl"),
                        ShaderLoader.LoadShader(FolderPath + "ShadowCube\\ShadowCubeFragmentShader.hlsl"),
                        ShaderName.ShadowCubeMapShader)
                }
            };

            SetValuesForSamplers();
        }

        private static void SetValuesForSamplers()
        {
            SetValue(ShaderName.AssimpShader, "gMaterial.DiffuseMap", 0);
            SetValue(ShaderName.AssimpShader, "gMaterial.NormalMap", 1);
            SetValue(ShaderName.AssimpShader, "gMaterial.SpecularMap", 2);
            SetValue(ShaderName.AssimpShader, "gCubeShadowMap", 3);

            for (int i = 0; i < LightningManager.SpotlightsCount; i++)
            {
                SetValue(ShaderName.AssimpShader, "gSpotLights[" + i + "].gShadowMap", 4 + i);
            }
        }

        public static void Dispose()
        {
            foreach (Shader shader in m_shaders.Values) shader.Dispose();
            m_shaders.Clear();
        }

        // INT
        public static void SetValue(ShaderName shaderName, string uniformName, int value)
        {
            UseShader(shaderName);
            m_shaders[shaderName].setValue(uniformName, value);
        }

        // FLOAT
        public static void SetValue(ShaderName shaderName, string uniformName, float value)
        {
            UseShader(shaderName);
            m_shaders[shaderName].setValue(uniformName, value);
        }

        // VEC3
        public static void SetValue(ShaderName shaderName, string uniformName, vector3f value)
        {
            UseShader(shaderName);
            m_shaders[shaderName].setValue(uniformName, value);
        }

        // MAT4
        public static void SetValue(ShaderName shaderName, string uniformName, Matrix4 value)
        {
            UseShader(shaderName);
            m_shaders[shaderName].setValue(uniformName, value);
        }

        // OBJECT
        public static void SetValue(ShaderName shaderName, Matrix4 world)
        {
            UseShader(shaderName);
            m_shaders[shaderName].setValue(world);
        }

        // SCREEN STATIC 
        public static void SetValue(ShaderName shaderName, Matrix4 world, Matrix4 pers)
        {
            UseShader(shaderName);
            m_shaders[shaderName].setValue(world, pers);
        }

        // SKYBOX
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
        ShadowShader,
        SelectingShader,
        ShadowCubeMapShader
    }
}