using game_2.FileManagers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game_2.Brain
{
    public static class CentralizedShaders
    {
        public static Shader ObjectShader;
        public static Shader SkyBoxShader;
        public static Shader ScreenShader;

        public static void Load()
        {
            ObjectShader = new Shader(
                ShaderLoader.LoadVertexShader(), 
                ShaderLoader.LoadFragmentShader());
            SkyBoxShader = new Shader(
                ShaderLoader.LoadShader("C:\\Users\\Lenovo\\source\\repos\\game_2\\Shaders\\Skybox\\SkyboxVetexShader.hlsl"),
                ShaderLoader.LoadShader("C:\\Users\\Lenovo\\source\\repos\\game_2\\Shaders\\Skybox\\SkyboxFragShader.hlsl"));
            ScreenShader = new Shader(
                ShaderLoader.LoadShader("C:\\Users\\Lenovo\\source\\repos\\game_2\\Shaders\\ScreenStatic\\ScreenStaticVertexShader.hlsl"),
                ShaderLoader.LoadShader("C:\\Users\\Lenovo\\source\\repos\\game_2\\Shaders\\ScreenStatic\\ScreenStaticFragmentShader.hlsl"));
        }

        public static void Dispose()
        {
            ObjectShader.Dispose();
            SkyBoxShader.Dispose();
            ScreenShader.Dispose();
        }
    }
}
