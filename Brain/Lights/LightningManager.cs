using game_2.Brain.Lights.LightStructures;
using game_2.MathFolder;

namespace game_2.Brain.Lights
{
    public static class LightningManager
    {
        public static LightingTechnique lightConfig;

        public static BaseLight baseLight;
        public static DirectionalLight directionalLight;

        public static PointLight[] pointLights = new PointLight[2];

        public static Spotlight[] spotlights = new Spotlight[2];

        public static void Init()
        {
            Console.WriteLine("Загрузка света...");

            lightConfig = new LightingTechnique();
            lightConfig.SetSpecular(Camera.Pos, 32);
            ConfigureBaseLight();
            ConfigureDirectionalLight();
            ConfigurePointlights();
            ConfigureSpotlights();
            CreateLamps();
        }

        private static float counter = 0;
        public static void Render(float deltaTime)
        {
            counter += deltaTime;
            DrawLamps(deltaTime);
            lightConfig.SetCameraPosition(Camera.Pos);

            UpdateLightsConfiguration();
        }

        private static void UpdateLightsConfiguration()
        {
            lightConfig.SetPointLights(pointLights);
            lightConfig.SetSpotLights(spotlights);
        }

        //light configuration
        private static void ConfigureBaseLight()
        {
            baseLight.Color = new vector3f(1, 1, 1);
            baseLight.Intensity = 0f;

            lightConfig.SetBaseLight(baseLight);
        }

        private static void ConfigureDirectionalLight()
        {
            directionalLight.BaseLight.Color = new vector3f(1, 1, 1);
            directionalLight.BaseLight.Intensity = 0.5f;
            directionalLight.Direction = new vector3f(1, -1, 1);

            lightConfig.SetDirectionalLight(directionalLight);
        }

        private static void ConfigurePointlights()
        {
            pointLights[0] = new PointLight();
            pointLights[0].SetPosition(10, 2, 0);
            pointLights[0].Attenuation.Exp = 0.032f;
            pointLights[0].Attenuation.Linear = 0.09f;
            pointLights[0].Attenuation.Constant = 1;
            pointLights[0].SetColor(new vector3f(1, 0, 0));
            pointLights[0].SetIntensity(2);

            pointLights[1] = new PointLight();
            pointLights[1].SetPosition(5, 2, 0);
            pointLights[1].Attenuation.Exp = 0.032f;
            pointLights[1].Attenuation.Linear = 0.09f;
            pointLights[1].Attenuation.Constant = 1;
            pointLights[1].SetColor(0, 1, 1);
            pointLights[1].SetIntensity(2);

            lightConfig.SetPointLights(pointLights);
        }

        private static void ConfigureSpotlights()
        {
            spotlights[0] = new Spotlight();
            //spotlights[0].PointLight.SetPosition(35, -7, 3);
            spotlights[0].PointLight.SetPosition(30, -4, 0);
            spotlights[0].PointLight.SetIntensity(3);
            spotlights[0].PointLight.SetColor(1, 1, 1);
            spotlights[0].PointLight.Attenuation.Constant = 1;
            spotlights[0].PointLight.Attenuation.Linear = 0.09f;
            spotlights[0].PointLight.Attenuation.Exp = 0.032f;
            spotlights[0].Direction = new vector3f(1, 0, 0);
            spotlights[0].Direction.Normalize();
            spotlights[0].Cutoff1 = 0.7f;

            spotlights[1] = new Spotlight();
            spotlights[1].PointLight.SetPosition(44.3f, -3.7f, -2.7f);
            spotlights[1].PointLight.SetIntensity(3);
            spotlights[1].PointLight.SetColor(1, 0.5f, 0.5f);
            spotlights[1].PointLight.Attenuation.Constant = 1;
            spotlights[1].PointLight.Attenuation.Linear = 0.09f;
            spotlights[1].PointLight.Attenuation.Exp = 0.032f;
            spotlights[1].Direction = new vector3f(-1, 0, 1);
            spotlights[1].Direction.Normalize();
            spotlights[1].Cutoff1 = 0.90f;

            lightConfig.SetSpotLights(spotlights);
        }
        
        //create & draw light objects
        private static void CreateLamps()
        {
            pointLights[0].SetLampScale(0.5f);
            pointLights[1].SetLampScale(0.5f);

            spotlights[0].PointLight.SetLampScale(0.2f);
            spotlights[1].PointLight.SetLampScale(0.2f);
        }

        private static void DrawLamps(float deltaTime)
        {
            CentralizedShaders.UseShader(ShaderName.MonochromeShader);
            spotlights[0].PointLight.Lamp.Draw();
            spotlights[1].PointLight.Lamp.Draw();
            pointLights[0].Lamp.Draw();
            pointLights[1].Lamp.Draw();
        }

        public static int PointlightsCount { get => pointLights.Length; }
        public static int SpotlightsCount { get => spotlights.Length; }
    }
}
