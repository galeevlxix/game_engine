using game_2.MathFolder;
using OpenTK.Graphics.OpenGL4;

namespace game_2.Brain.Lights
{
    public class LightingTechnique
    {
        BaseLightLocations _baseLightLocations;
        DirectionalLightLocations _directionalLightLocations;

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
            
            //dirLight
            _directionalLightLocations.BaseLightLocations.Color = CentralizedShaders.ObjectShader.GetUniformLocation("gDirectionalLight.Base.Color");
            _directionalLightLocations.BaseLightLocations.AmbientIntensity = CentralizedShaders.ObjectShader.GetUniformLocation("gDirectionalLight.Base.AmbientIntensity");
            _directionalLightLocations.BaseLightLocations.DiffuseIntensity = CentralizedShaders.ObjectShader.GetUniformLocation("gDirectionalLight.Base.DiffuseIntensity");
            _directionalLightLocations.Direction = CentralizedShaders.ObjectShader.GetUniformLocation("gDirectionalLight.Direction");
        }

        public void SetBaseLight(BaseLight baseLight)
        {
            Use();
            GL.Uniform3(_baseLightLocations.Color, baseLight.Color.x, baseLight.Color.y, baseLight.Color.z);
            GL.Uniform1(_baseLightLocations.AmbientIntensity, baseLight.AmbientIntensity);
        }

        public void SetBaseLight(vector3f Color, float AmbientIntensity)
        {
            Use();
            GL.Uniform3(_baseLightLocations.Color, Color.x, Color.y, Color.z);
            GL.Uniform1(_baseLightLocations.AmbientIntensity, AmbientIntensity);
        }

        public void SetDirectionalLight(DirectionalLight directionalLight)
        {
            Use();
            GL.Uniform3(_directionalLightLocations.BaseLightLocations.Color, 
                directionalLight.BaseLight.Color.x, 
                directionalLight.BaseLight.Color.y, 
                directionalLight.BaseLight.Color.z);
            GL.Uniform1(_directionalLightLocations.BaseLightLocations.AmbientIntensity, 
                directionalLight.BaseLight.AmbientIntensity);
            GL.Uniform1(_directionalLightLocations.BaseLightLocations.DiffuseIntensity,
                directionalLight.BaseLight.DiffuseIntensity);
            GL.Uniform3(_directionalLightLocations.Direction,
                directionalLight.Direction.x,
                directionalLight.Direction.y,
                directionalLight.Direction.z);
        }

        public void SetDirectionalLight(vector3f Color, float AmbientIntensity, float DiffuseIntensity ,vector3f Direction)
        {
            Use();
            GL.Uniform3(_directionalLightLocations.BaseLightLocations.Color,
                Color.x,
                Color.y,
                Color.z);
            GL.Uniform1(_directionalLightLocations.BaseLightLocations.AmbientIntensity,
                AmbientIntensity);
            GL.Uniform1(_directionalLightLocations.BaseLightLocations.DiffuseIntensity,
                DiffuseIntensity);
            GL.Uniform3(_directionalLightLocations.Direction,
                Direction.x,
                Direction.y,
                Direction.z);
        }

        private void Use() => CentralizedShaders.ObjectShader.Use();
    }
}
