using game_2.Brain.Lights.LightStructures;
using game_2.Brain.MonochromeObjectFolder;
using game_2.MathFolder;
using OpenTK.Mathematics;

namespace game_2.Brain.Lights
{
    public static class LightningManager
    {
        public static LightingTechnique lightConfig;

        private static BaseLight baseLight;
        private static DirectionalLight directionalLight;

        public static PointLight[] pointLights = new PointLight[2];
        public static Spotlight[] spotlights = new Spotlight[2];

        private static MonochromeObject redLamp;
        private static MonochromeObject blueLamp;

        private static MonochromeObject proj;

        public static void Init()
        {
            CreateLamps();
            lightConfig = new LightingTechnique();
            lightConfig.SetSpecular(Camera.Pos, 32);
            ConfigureBaseLight();
            ConfigureDirectionalLight();
            //ConfigurePointLights();
            ConfigureSpotlights();
        }

        private static float counter = 0;
        public static void Render(float deltaTime)
        {
            counter += deltaTime;
            DrawLamps(deltaTime);
            lightConfig.SetCameraPosition(Camera.Pos);
            spotlights[1].Direction = -Camera.Target;
            spotlights[1].PointLight.Position = Camera.Pos;

            UpdateLightsConfiguration();
        }

        private static void UpdateLightsConfiguration()
        {
            //lightConfig.SetPointLights(pointLights);
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
            directionalLight.BaseLight.Intensity = 0.1f;
            directionalLight.Direction = new vector3f(1, -1, -1);

            lightConfig.SetDirectionalLight(directionalLight);
        }

        private static void ConfigurePointLights()
        {
            pointLights[0].Position = new vector3f(-5, 2, 0);
            pointLights[0].Attenuation.Exp = 0.032f;
            pointLights[0].Attenuation.Linear = 0.09f;
            pointLights[0].Attenuation.Constant = 1;
            pointLights[0].BaseLight.Color = new vector3f(1, 0, 0);
            pointLights[0].BaseLight.Intensity = 1f;

            pointLights[1].Position = new vector3f(5, 2, 0);
            pointLights[1].Attenuation.Exp = 0.032f;
            pointLights[1].Attenuation.Linear = 0.09f;
            pointLights[1].Attenuation.Constant = 1;
            pointLights[1].BaseLight.Color = new vector3f(0, 1, 1);
            pointLights[1].BaseLight.Intensity = 1f;

            lightConfig.SetPointLights(pointLights);
        }

        private static void ConfigureSpotlights()
        {
            spotlights[0].PointLight.Position = new vector3f(9, 5, 9);
            spotlights[0].PointLight.BaseLight.Color = new vector3f(1, 1, 0);
            spotlights[0].PointLight.BaseLight.Intensity = 3;
            spotlights[0].PointLight.Attenuation.Constant = 1;
            spotlights[0].PointLight.Attenuation.Linear = 0.027f;
            spotlights[0].PointLight.Attenuation.Exp = 0.0028f;
            spotlights[0].Direction = new vector3f(1, 0, 0);
            spotlights[0].Cutoff1 = 0.20f;

            spotlights[1].PointLight.Position = Camera.Pos;
            spotlights[1].PointLight.BaseLight.Color = new vector3f(1, 0, 1);
            spotlights[1].PointLight.BaseLight.Intensity = 0;
            spotlights[1].PointLight.Attenuation.Constant = 1;
            spotlights[1].PointLight.Attenuation.Linear = 0.027f;
            spotlights[1].PointLight.Attenuation.Exp = 0.0028f;
            spotlights[1].Direction = -Camera.Target;
            spotlights[1].Cutoff1 = 0.97f;

            lightConfig.SetSpotLights(spotlights);
        }
        
        //create & draw light objects
        private static void CreateLamps()
        {
/*            redLamp = new MonochromeObject(new vector3f(1, 0, 0), new vector3f(1, 1, 1));
            redLamp.pipeline.SetScale(0.5f);
            redLamp.pipeline.SetPosition(-5, 2, 0);

            blueLamp = new MonochromeObject(new vector3f(0, 1, 1), new vector3f(1, 1, 1));
            blueLamp.pipeline.SetScale(0.5f);
            blueLamp.pipeline.SetPosition(5, 2, 0);*/

            proj = new MonochromeObject(new vector3f(1, 1, 1), new vector3f(1, 1, 1));
            proj.pipeline.SetScale(0.1f);
            proj.pipeline.SetPosition(9, 1, 0);
        }

        private static void DrawLamps(float deltaTime)
        {
            CentralizedShaders.UseShader(ShaderName.MonochromeShader);

            /*redLamp.pipeline.MoveX(math3d.sin(counter) * 5, deltaTime);
            redLamp.pipeline.MoveY(math3d.sin(counter * 3) * 3, deltaTime);
            redLamp.pipeline.MoveZ(-math3d.cos(counter) * 5, deltaTime);
            redLamp.Draw();
            blueLamp.pipeline.MoveX(-math3d.sin(counter) * 5, deltaTime);
            blueLamp.pipeline.MoveY(math3d.sin(counter * 3) * 3, deltaTime);
            blueLamp.pipeline.MoveZ(math3d.cos(counter) * 5, deltaTime);
            blueLamp.Draw();*/

            var posX = spotlights[0].PointLight.Position.x;
            var posY = spotlights[0].PointLight.Position.y;
            var posZ = spotlights[0].PointLight.Position.z;
            //spotlights[0].PointLight.Position.y += math3d.sin(counter) * deltaTime;
            //spotlights[0].PointLight.Position.z += -math3d.cos(counter) * 5 * deltaTime;

            proj.pipeline.SetPosition(spotlights[0].PointLight.Position);
            //spotlights[0].Direction = new vector3f(math3d.abs(math3d.sin(counter)), 0, math3d.abs(math3d.cos(counter)));
            proj.Draw();
        }

        public static int PointlightsCount { get => pointLights.Length; }
        public static int SpotlightsCount { get => spotlights.Length; }
    }
}
