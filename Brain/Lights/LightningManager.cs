using game_2.Brain.Lights.LightStructures;
using game_2.MathFolder;

namespace game_2.Brain.Lights
{
    public static class LightningManager
    {
        public static LightingTechnique lightConfig;

        public static BaseLight baseLight;
        public static DirectionalLight directionalLight;

        public static PointLight[] pointLights = new PointLight[0];

        public static Spotlight[] spotlights = new Spotlight[12];

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
            baseLight.Intensity = 0.1f;

            lightConfig.SetBaseLight(baseLight);
        }

        private static void ConfigureDirectionalLight()
        {
            directionalLight.BaseLight.Color = new vector3f(1, 1, 1);
            directionalLight.BaseLight.Intensity = 0.0f;
            directionalLight.Direction = new vector3f(0, 1, 0);

            lightConfig.SetDirectionalLight(directionalLight);
        }

        private static void ConfigurePointlights()
        {
            /*pointLights[0] = new PointLight();
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

            lightConfig.SetPointLights(pointLights);*/
        }

        private static void ConfigureSpotlights()
        {
            vector3f MapCenter = (Camera.max_point + Camera.min_point) / 2;

            for (int i = 0; i < spotlights.Length; i++)
            {
                spotlights[i] = new Spotlight();
                spotlights[i].PointLight.SetIntensity(3); //3
                spotlights[i].PointLight.SetColor(1, 1, 1);
                spotlights[i].PointLight.Attenuation.Constant = 1;
                spotlights[i].PointLight.Attenuation.Linear = 0.09f;
                spotlights[i].PointLight.Attenuation.Exp = 0.032f;
                spotlights[i].Cutoff1 = 0.80f;
            }
            
            spotlights[0].PointLight.SetPosition(27.5f, 4.6f, 0);
            spotlights[0].Direction = new vector3f(1, -0.5f, 0);
            spotlights[0].PointLight.SetIntensity(5); //5
            
            spotlights[1].PointLight.SetPosition(12.5f, 4.6f, 0f);
            spotlights[1].Direction = new vector3f(-1, -0.5f, 0);
            spotlights[1].PointLight.SetIntensity(5); //5

            spotlights[2].PointLight.SetPosition(27.5f, 4.5f, 1);
            spotlights[2].Direction = new vector3f(0, -1, 0.5f);

            spotlights[3].PointLight.SetPosition(27.5f, 4.5f, -1f);
            spotlights[3].Direction = new vector3f(0, -1, -0.5f);

            spotlights[4].PointLight.SetPosition(12.5f, 4.5f, 1);
            spotlights[4].Direction = new vector3f(0, -1, 0.5f);

            spotlights[5].PointLight.SetPosition(12.5f, 4.5f, -1f);
            spotlights[5].Direction = new vector3f(0, -1, -0.5f);

            spotlights[6].PointLight.SetPosition(20, 4.5f, 1);
            spotlights[6].Direction = new vector3f(0, -1, 0.5f);

            spotlights[7].PointLight.SetPosition(20, 4.5f, -1f);
            spotlights[7].Direction = new vector3f(0, -1, -0.5f);

            spotlights[8].PointLight.SetPosition(35, 4.5f, 1);
            spotlights[8].Direction = new vector3f(0, -1, 0.5f);

            spotlights[9].PointLight.SetPosition(35, 4.5f, -1f);
            spotlights[9].Direction = new vector3f(0, -1, -0.5f);

            spotlights[10].PointLight.SetPosition(5, 4.5f, 1);
            spotlights[10].Direction = new vector3f(0, -1, 0.5f);

            spotlights[11].PointLight.SetPosition(5, 4.5f, -1f);
            spotlights[11].Direction = new vector3f(0, -1, -0.5f);
            

            for (int i = 0; i < spotlights.Length; i++)
            {
                spotlights[i].Direction.Normalize();
            }

            lightConfig.SetSpotLights(spotlights);
        }
        
        //create & draw light objects
        private static void CreateLamps()
        {
            //pointLights[0].SetLampScale(0.5f);
            //pointLights[1].SetLampScale(0.5f);

            spotlights[0].PointLight.SetLampScale(0.3f);
            
            spotlights[1].PointLight.SetLampScale(0.3f);

            spotlights[2].PointLight.SetLampScale(0.2f);
            spotlights[3].PointLight.SetLampScale(0.2f);

            spotlights[4].PointLight.SetLampScale(0.2f);
            spotlights[5].PointLight.SetLampScale(0.2f);

            spotlights[6].PointLight.SetLampScale(0.2f);
            spotlights[7].PointLight.SetLampScale(0.2f);

            spotlights[8].PointLight.SetLampScale(0.2f);
            spotlights[9].PointLight.SetLampScale(0.2f);

            spotlights[10].PointLight.SetLampScale(0.2f);
            spotlights[11].PointLight.SetLampScale(0.2f);
            
        }

        private static void DrawLamps(float deltaTime)
        {
            CentralizedShaders.UseShader(ShaderName.MonochromeShader);
            spotlights[0].PointLight.Lamp.Draw();
            
            spotlights[1].PointLight.Lamp.Draw();

            spotlights[2].PointLight.Lamp.Draw();
            spotlights[3].PointLight.Lamp.Draw();

            spotlights[4].PointLight.Lamp.Draw();
            spotlights[5].PointLight.Lamp.Draw();

            spotlights[6].PointLight.Lamp.Draw();
            spotlights[7].PointLight.Lamp.Draw();

            spotlights[8].PointLight.Lamp.Draw();
            spotlights[9].PointLight.Lamp.Draw();

            spotlights[10].PointLight.Lamp.Draw();
            spotlights[11].PointLight.Lamp.Draw();
            
            //pointLights[0].Lamp.Draw();
            //pointLights[1].Lamp.Draw();
        }

        public static int PointlightsCount { get => pointLights.Length; }
        public static int SpotlightsCount { get => spotlights.Length; }
    }
}
