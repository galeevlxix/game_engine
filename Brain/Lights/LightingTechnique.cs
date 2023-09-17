using game_2.MathFolder;
using OpenTK.Graphics.OpenGL4;
using System.Drawing;

namespace game_2.Brain.Lights
{
    public class LightingTechnique
    {
        private BaseLightLocations _baseLightLocations;
        private DirectionalLightLocations _directionalLightLocations;

        int _cameraPositionLocation;
        int _matSpecularIntensityLocation;
        int _matSpecularPowerLocation;

        public LightingTechnique()
        {
            _baseLightLocations = new BaseLightLocations();
            Init();
            SetDefaultValues();
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

            //specular
            _cameraPositionLocation = CentralizedShaders.ObjectShader.GetUniformLocation("gCameraPos");
            _matSpecularIntensityLocation = CentralizedShaders.ObjectShader.GetUniformLocation("gMatSpecularIntensity");
            _matSpecularPowerLocation = CentralizedShaders.ObjectShader.GetUniformLocation("gMatSpecularPower");
        }

        private void SetDefaultValues()
        {
            Use();

            //baseLight
            GL.Uniform3(_baseLightLocations.Color, 0, 0, 0);
            GL.Uniform1(_baseLightLocations.AmbientIntensity, 0);

            //dirLight
            GL.Uniform3(_directionalLightLocations.BaseLightLocations.Color, 0, 0, 0);
            GL.Uniform1(_directionalLightLocations.BaseLightLocations.AmbientIntensity, 0);
            GL.Uniform1(_directionalLightLocations.BaseLightLocations.DiffuseIntensity, 0);
            GL.Uniform3(_directionalLightLocations.Direction, 0, 0, 0);

            //specular
            GL.Uniform3(_cameraPositionLocation, 0, 0, 0);
            GL.Uniform1(_matSpecularIntensityLocation, 0);
            GL.Uniform1(_matSpecularPowerLocation, 0);
        }

        //baseLight
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

        //directionalLight
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

        //specular
        public void SetSpecular(vector3f Position, float SpecularIntensity, float SpecularPower)
        {
            Use();
            GL.Uniform3(_cameraPositionLocation, Position.x, Position.x, Position.z);
            GL.Uniform1(_matSpecularIntensityLocation, SpecularIntensity);
            GL.Uniform1(_matSpecularPowerLocation, SpecularPower);
        }

        public void SetCameraPosition(vector3f Position)
        {
            Use();
            GL.Uniform3(_cameraPositionLocation, Position.x, Position.x, Position.z);
        }

        public void SetMatSpecularIntensity(float SpecularIntensity)
        {
            Use();
            GL.Uniform1(_matSpecularIntensityLocation, SpecularIntensity);
        }

        public void SetMatSpecularPower(float SpecularPower)
        {
            Use();
            GL.Uniform1(_matSpecularPowerLocation, SpecularPower);
        }

        private void Use() => CentralizedShaders.ObjectShader.Use();
    }
}
