using game_2.FileManagers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public static Shader MonochromeShader;
        public static Shader AssimpShader; //добавлены карты нормалей

        public static void Load()
        {
            ObjectShader = new Shader(
                ShaderLoader.LoadVertexShader(),
                ShaderLoader.LoadFragmentShader());
            SkyBoxShader = new Shader(
                ShaderLoader.LoadShader("C:\\Users\\Lenovo\\source\\repos\\game_2\\Files\\Shaders\\Skybox\\SkyboxVetexShader.hlsl"),
                ShaderLoader.LoadShader("C:\\Users\\Lenovo\\source\\repos\\game_2\\Files\\Shaders\\Skybox\\SkyboxFragShader.hlsl"));
            ScreenShader = new Shader(
                ShaderLoader.LoadShader("C:\\Users\\Lenovo\\source\\repos\\game_2\\Files\\Shaders\\ScreenStatic\\ScreenStaticVertexShader.hlsl"),
                ShaderLoader.LoadShader("C:\\Users\\Lenovo\\source\\repos\\game_2\\Files\\Shaders\\ScreenStatic\\ScreenStaticFragmentShader.hlsl"));
            MonochromeShader = new Shader(
                ShaderLoader.LoadShader("C:\\Users\\Lenovo\\source\\repos\\game_2\\Files\\Shaders\\MonochromeObject\\MonoVertexShader.hlsl"),
                ShaderLoader.LoadShader("C:\\Users\\Lenovo\\source\\repos\\game_2\\Files\\Shaders\\MonochromeObject\\MonoFragmentShader.hlsl"));
            AssimpShader = new Shader(
                ShaderLoader.LoadShader("C:\\Users\\Lenovo\\source\\repos\\game_2\\Files\\Shaders\\AssimpObject\\VertexShader.hlsl"),
                ShaderLoader.LoadShader("C:\\Users\\Lenovo\\source\\repos\\game_2\\Files\\Shaders\\AssimpObject\\FragmentShader.hlsl"));
        }

        public static void Dispose()
        {
            ObjectShader.Dispose();
            SkyBoxShader.Dispose();
            ScreenShader.Dispose();
            MonochromeShader.Dispose();
        }
    }
}
