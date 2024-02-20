using game_2.Brain.Lights.LightStructures;
using game_2.MathFolder;
using OpenTK.Graphics.OpenGL4;

namespace game_2.Brain.Lights
{
    public class LightingTechnique
    {
        private BaseLightLocations _baseLightLocations;
        private DirectionalLightLocations _directionalLightLocations;

        int _cameraPositionLocation;
        int _matSpecularIntensityLocation;
        int _matSpecularPowerLocation;

        const int MAX_POINT_LIGHTS = 10;
        const int MAX_SPOT_LIGHTS = 10;

        int _numPointLightsLocation;
        PointLightLocations[] _pointLightLocations = new PointLightLocations[MAX_POINT_LIGHTS];

        int _numSpotLightsLocation;
        SpotlightLocations[] _spotlightLocations = new SpotlightLocations[MAX_SPOT_LIGHTS];

        public LightingTechnique()
        {
            Init();
            SetDefaultValues();
        }

        private void Init()
        {
            //baseLight
            _baseLightLocations.Color = CentralizedShaders.ObjectShader.GetUniformLocation("gBaseLight.Color");
            _baseLightLocations.Intensity = CentralizedShaders.ObjectShader.GetUniformLocation("gBaseLight.Intensity");

            //dirLight
            _directionalLightLocations.BaseLight.Color = CentralizedShaders.ObjectShader.GetUniformLocation("gDirectionalLight.Base.Color");
            _directionalLightLocations.BaseLight.Intensity = CentralizedShaders.ObjectShader.GetUniformLocation("gDirectionalLight.Base.Intensity");
            _directionalLightLocations.Direction = CentralizedShaders.ObjectShader.GetUniformLocation("gDirectionalLight.Direction");

            //specular
            _cameraPositionLocation = CentralizedShaders.ObjectShader.GetUniformLocation("gCameraPos");
            _matSpecularIntensityLocation = CentralizedShaders.ObjectShader.GetUniformLocation("gMatSpecularIntensity");
            _matSpecularPowerLocation = CentralizedShaders.ObjectShader.GetUniformLocation("gMatSpecularPower");

            //pointLights
            _numPointLightsLocation = CentralizedShaders.ObjectShader.GetUniformLocation("gNumPointLights");
            for (int i = 0; i < MAX_POINT_LIGHTS; i++)
            {
                _pointLightLocations[i].BaseLightLocations.Color = CentralizedShaders.ObjectShader.GetUniformLocation("gPointLights[" + i + "].Base.Color");
                _pointLightLocations[i].BaseLightLocations.Intensity = CentralizedShaders.ObjectShader.GetUniformLocation("gPointLights[" + i + "].Base.Intensity");
                _pointLightLocations[i].Position = CentralizedShaders.ObjectShader.GetUniformLocation("gPointLights[" + i +"].Position");
                _pointLightLocations[i].Attenuation.Exp = CentralizedShaders.ObjectShader.GetUniformLocation("gPointLights[" + i + "].Atten.Exp");
                _pointLightLocations[i].Attenuation.Linear = CentralizedShaders.ObjectShader.GetUniformLocation("gPointLights[" + i + "].Atten.Linear");
                _pointLightLocations[i].Attenuation.Constant = CentralizedShaders.ObjectShader.GetUniformLocation("gPointLights[" + i + "].Atten.Constant");
            }

            //spotLights
            _numSpotLightsLocation = CentralizedShaders.ObjectShader.GetUniformLocation("gNumSpotLights");

            for (int i = 0; i < MAX_SPOT_LIGHTS; i++)
            {
                _spotlightLocations[i].PointLightLocations.BaseLightLocations.Color = CentralizedShaders.ObjectShader.GetUniformLocation("gSpotLights[" + i + "].Base.Base.Color");
                _spotlightLocations[i].PointLightLocations.BaseLightLocations.Intensity = CentralizedShaders.ObjectShader.GetUniformLocation("gSpotLights[" + i + "].Base.Base.Intensity");
                _spotlightLocations[i].PointLightLocations.Position = CentralizedShaders.ObjectShader.GetUniformLocation("gSpotLights[" + i + "].Base.Position");
                _spotlightLocations[i].PointLightLocations.Attenuation.Exp = CentralizedShaders.ObjectShader.GetUniformLocation("gSpotLights[" + i + "].Base.Atten.Exp");
                _spotlightLocations[i].PointLightLocations.Attenuation.Constant = CentralizedShaders.ObjectShader.GetUniformLocation("gSpotLights[" + i + "].Base.Atten.Constant");
                _spotlightLocations[i].PointLightLocations.Attenuation.Linear = CentralizedShaders.ObjectShader.GetUniformLocation("gSpotLights[" + i + "].Base.Atten.Linear");
                _spotlightLocations[i].Direction = CentralizedShaders.ObjectShader.GetUniformLocation("gSpotLights[" + i + "].Direction");
                _spotlightLocations[i].Cutoff1 = CentralizedShaders.ObjectShader.GetUniformLocation("gSpotLights[" + i + "].Cutoff1");
                _spotlightLocations[i].Cutoff2 = CentralizedShaders.ObjectShader.GetUniformLocation("gSpotLights[" + i + "].Cutoff2");
            }
        }

        //set null values to attributes
        private void SetDefaultValues() 
        {
            Use();
            //baseLight
            GL.Uniform3(_baseLightLocations.Color, 0, 0, 0);
            GL.Uniform1(_baseLightLocations.Intensity, 0);

            //dirLight
            GL.Uniform3(_directionalLightLocations.BaseLight.Color, 0, 0, 0);
            GL.Uniform1(_directionalLightLocations.BaseLight.Intensity, 0);
            GL.Uniform3(_directionalLightLocations.Direction, 0, 0, 0);

            //specular
            GL.Uniform3(_cameraPositionLocation, 0, 0, 0);
            GL.Uniform1(_matSpecularIntensityLocation, 0);
            GL.Uniform1(_matSpecularPowerLocation, 0);

            //pointlights
            GL.Uniform1(_numPointLightsLocation, 0);

            for (int i = 0; i < MAX_POINT_LIGHTS; i++)
            {
                GL.Uniform3(_pointLightLocations[i].BaseLightLocations.Color, 0, 0, 0);
                GL.Uniform1(_pointLightLocations[i].BaseLightLocations.Intensity, 0);
                GL.Uniform3(_pointLightLocations[i].Position, 0, 0, 0);
                GL.Uniform1(_pointLightLocations[i].Attenuation.Exp, 0);
                GL.Uniform1(_pointLightLocations[i].Attenuation.Linear, 0);
                GL.Uniform1(_pointLightLocations[i].Attenuation.Constant, 0);
            }

            //spotLights
            GL.Uniform1(_numSpotLightsLocation, 0);
            for (int i = 0; i < MAX_SPOT_LIGHTS; i++)
            {
                GL.Uniform3(_spotlightLocations[i].PointLightLocations.BaseLightLocations.Color, 0, 0, 0);
                GL.Uniform1(_spotlightLocations[i].PointLightLocations.BaseLightLocations.Intensity, 0);
                GL.Uniform3(_spotlightLocations[i].PointLightLocations.Position, 0, 0, 0);
                GL.Uniform1(_spotlightLocations[i].PointLightLocations.Attenuation.Exp, 0);
                GL.Uniform1(_spotlightLocations[i].PointLightLocations.Attenuation.Constant, 0);
                GL.Uniform1(_spotlightLocations[i].PointLightLocations.Attenuation.Linear, 0);
                GL.Uniform3(_spotlightLocations[i].Direction, 0, 0, 0);
                GL.Uniform1(_spotlightLocations[i].Cutoff1, 0);
                GL.Uniform1(_spotlightLocations[i].Cutoff2, 0);
            }
        }

        //baseLight
        public void SetBaseLight(BaseLight baseLight)
        {
            Use();
            GL.Uniform3(_baseLightLocations.Color, baseLight.Color.x, baseLight.Color.y, baseLight.Color.z);
            GL.Uniform1(_baseLightLocations.Intensity, baseLight.Intensity);
        }

        public void SetBaseLightColor(vector3f color)
        {
            Use();
            GL.Uniform3(_baseLightLocations.Color, color.x, color.y, color.z);
        }

        public void SetBaseLightIntensity(float intensity)
        {
            GL.Uniform1(_baseLightLocations.Intensity, intensity);
        }

        //directionalLight
        public void SetDirectionalLight(DirectionalLight directionalLight)
        {
            Use();
            GL.Uniform3(_directionalLightLocations.BaseLight.Color, 
                directionalLight.BaseLight.Color.x, 
                directionalLight.BaseLight.Color.y, 
                directionalLight.BaseLight.Color.z);
            GL.Uniform1(_directionalLightLocations.BaseLight.Intensity, 
                directionalLight.BaseLight.Intensity);
            GL.Uniform3(_directionalLightLocations.Direction,
                directionalLight.Direction.x,
                directionalLight.Direction.y,
                directionalLight.Direction.z);
        }

        public void SetDirectionalLightColor(vector3f color)
        {
            Use();
            GL.Uniform3(_directionalLightLocations.BaseLight.Color, color.x, color.y, color.z);
        }

        public void SetDirectionalLightIntensity(float intensity)
        {
            Use();
            GL.Uniform1(_directionalLightLocations.BaseLight.Intensity, intensity);
        }

        public void SetDirectionalLightDirection(vector3f direction)
        {
            Use();
            GL.Uniform3(_directionalLightLocations.Direction, direction.x, direction.y, direction.z);
        }

        public void SetDirectionalLightDirection(float x, float y, float z)
        {
            Use();
            GL.Uniform3(_directionalLightLocations.Direction, x, y, z);
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
            GL.Uniform3(_cameraPositionLocation, Position.x, Position.y, Position.z);
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

        //pointlights

        public void SetPointLights(PointLight[] pointLights)
        {
            Use();
            GL.Uniform1(_numPointLightsLocation, pointLights.Length);

            for (int i = 0; i < pointLights.Length; i++)
            {
                GL.Uniform3(
                    _pointLightLocations[i].BaseLightLocations.Color, 
                    pointLights[i].BaseLight.Color.x, 
                    pointLights[i].BaseLight.Color.y, 
                    pointLights[i].BaseLight.Color.z);
                GL.Uniform1(
                    _pointLightLocations[i].BaseLightLocations.Intensity, 
                    pointLights[i].BaseLight.Intensity);
                GL.Uniform3(
                    _pointLightLocations[i].Position, 
                    pointLights[i].Position.x, 
                    pointLights[i].Position.y, 
                    pointLights[i].Position.z);
                GL.Uniform1(
                    _pointLightLocations[i].Attenuation.Exp, 
                    pointLights[i].Attenuation.Exp);
                GL.Uniform1(
                    _pointLightLocations[i].Attenuation.Linear, 
                    pointLights[i].Attenuation.Linear);
                GL.Uniform1(
                    _pointLightLocations[i].Attenuation.Constant, 
                    pointLights[i].Attenuation.Constant);
            }
        }

        //spotlights
        public void SetSpotLights(Spotlight[] spotLights)
        {
            Use();
            GL.Uniform1(_numSpotLightsLocation, spotLights.Length);

            for (int i = 0; i < spotLights.Length; i++)
            {
                GL.Uniform3(
                    _spotlightLocations[i].PointLightLocations.BaseLightLocations.Color, 
                    spotLights[i].PointLight.BaseLight.Color.x, 
                    spotLights[i].PointLight.BaseLight.Color.y, 
                    spotLights[i].PointLight.BaseLight.Color.z);
                GL.Uniform1(
                    _spotlightLocations[i].PointLightLocations.BaseLightLocations.Intensity,
                    spotLights[i].PointLight.BaseLight.Intensity);
                GL.Uniform3(
                    _spotlightLocations[i].PointLightLocations.Position,
                    spotLights[i].PointLight.Position.x,
                    spotLights[i].PointLight.Position.y,
                    spotLights[i].PointLight.Position.z);
                GL.Uniform1(
                    _spotlightLocations[i].PointLightLocations.Attenuation.Exp,
                    spotLights[i].PointLight.Attenuation.Exp);
                GL.Uniform1(
                    _spotlightLocations[i].PointLightLocations.Attenuation.Constant,
                    spotLights[i].PointLight.Attenuation.Constant);
                GL.Uniform1(
                    _spotlightLocations[i].PointLightLocations.Attenuation.Linear,
                    spotLights[i].PointLight.Attenuation.Linear);
                GL.Uniform3(
                    _spotlightLocations[i].Direction,
                    spotLights[i].Direction.x,
                    spotLights[i].Direction.y,
                    spotLights[i].Direction.z);
                GL.Uniform1(
                    _spotlightLocations[i].Cutoff1,
                    spotLights[i].Cutoff1);
                GL.Uniform1(
                _spotlightLocations[i].Cutoff2,
                spotLights[i].Cutoff2);
            }
        }

        private void Use() => CentralizedShaders.ObjectShader.Use();
    }
}
