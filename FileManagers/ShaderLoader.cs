using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game_2.FileManagers
{
    public static class ShaderLoader
    {
        private static string vspath = @"..\..\..\Shaders\VertexShader.hlsl";
        private static string fspath = @"C:..\..\..\Shaders\FragmentShader.hlsl";
        public static string LoadVertexShader()
        {
            return new StreamReader(vspath).ReadToEnd();
        }
        public static string LoadFragmentShader()
        {
            return new StreamReader(fspath).ReadToEnd();
        }
        public static string LoadShader(string path)
        {
            return new StreamReader(path).ReadToEnd();
        }
    }
}
