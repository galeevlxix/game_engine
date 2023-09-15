using game_2.MathFolder;
using OpenTK.Graphics.OpenGL4;

namespace game_2.Brain.Lights
{
    public class LightingTechnique
    {
        BaseLightLocations _baseLightLocations;

        public LightingTechnique()
        {
            _baseLightLocations = new BaseLightLocations();
            Init();
        }

        private void Init()
        {
            //baseLight
            _baseLightLocations.Color = CentralizedShaders.ObjectShader.GetUniformLocation("gBaseLight.Color");
            _baseLightLocations.AmbientIntensity = CentralizedShaders.ObjectShader.GetUniformLocation("gBaseLight.AmbientIntensity");
        }

        public void SetBaseLight(BaseLight baseLight)
        {
            GL.Uniform3(_baseLightLocations.Color, baseLight.Color.x, baseLight.Color.y, baseLight.Color.z);
            GL.Uniform1(_baseLightLocations.AmbientIntensity, baseLight.AmbientIntensity);
        }

        public void SetBaseLight(vector3f Color, float AmbientIntensity)
        {
            GL.Uniform3(_baseLightLocations.Color, Color.x, Color.y, Color.z);
            GL.Uniform1(_baseLightLocations.AmbientIntensity, AmbientIntensity);
        }
    }
}
