namespace game_2.FileManagers
{
    public static class ShaderLoader
    {
        private static string vspath = @"..\..\..\Files\Shaders\Object\VertexShader.hlsl";
        private static string fspath = @"..\..\..\Files\Shaders\Object\FragmentShader.hlsl";
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
