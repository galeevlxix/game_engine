using System.ComponentModel;
using game_2.FileManagers;
using game_2.MathFolder;

namespace game_2.Brain
{
    public class GameObj
    {
        private Mesh mesh;
        private Shader shader;
        public Pipeline pipeline;

        public GameObj()
        {
            mesh = new Mesh();
            shader = new Shader(ShaderLoader.LoadVertexShader(), ShaderLoader.LoadFragmentShader());
            pipeline = new Pipeline();

            pipeline.mPersProj = new mPersProj();
        }

        public GameObj(string file_name)
        {
            mesh = new Mesh(file_name);
            shader = new Shader(ShaderLoader.LoadVertexShader(), ShaderLoader.LoadFragmentShader());
            pipeline = new Pipeline();

            pipeline.mPersProj = new mPersProj();
        }

        public void Draw()
        {
            shader.setMatrix(matrix4f.ToFloatArray(pipeline.getTransformation()));
            shader.Use();
            mesh.Draw();
        }
    }
}
