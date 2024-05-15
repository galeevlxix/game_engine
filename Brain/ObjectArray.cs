using game_2.Brain.Lights;
using game_2.Brain.Lights.LightStructures;
using game_2.Brain.NewAssimpFolder;
using game_2.MathFolder;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;

namespace game_2.Brain
{
    public static class ObjectArray
    {
        private static Dictionary<string, AObject> obj_list;

        private static string ModelFolderPath = "..\\..\\..\\Files\\Models\\";

        private static Shader? shadowShader = CentralizedShaders.GetShader(ShaderName.ShadowShader);
        private static Shader? normalShader = CentralizedShaders.GetShader(ShaderName.AssimpShader);

        public static void Init()
        {
            Console.WriteLine("Загрузка моделей (assimp)...");

            obj_list = new Dictionary<string, AObject>();

            Add("museum", "Museums\\VR_Gallery\\VR_Gallery_comp.obj");
            Add("table", "Museums\\museum_table\\OPM0032.fbx");
            Add("sculpt", "Museums\\bull\\bull3.obj");

            SetProperties();
        }

        private static void SetProperties()
        {
            SetAngle("museum", 0, 0, 0);
            SetPosition("museum", 20, 0, 0);
            SetScale("museum", 0.01f);

            SetAngle("table", 90, 0, 90);
            SetPosition("table", 43, -8.8f, 0);
            SetScale("table", 4);

            SetAngle("sculpt", 0, 180, 0);
            SetPosition("sculpt", 42.8f, -5.75f, -0.5f);
            SetScale("sculpt", 0.15f);
        }

        private static float bull_speedY = 0;
        public static void OnRender(float deltaTime)
        {
            bull_speedY += 3 * deltaTime;
            if (bull_speedY >= 2 * math3d.PI)
                bull_speedY = 0;

            Move("sculpt", 0, math3d.sin(bull_speedY) / 2, 0, deltaTime);
        }

        public static void Add(string name, string filepath)
        {
            obj_list.Add(name, new AObject(ModelFolderPath + filepath));
            Console.WriteLine("     Загружена модель " + name);
        }

        public static void Remove(string name)
        {
            obj_list[name].OnDelete();
            obj_list.Remove(name);
        }

        public static void Clear()
        {
            foreach (AObject obj in obj_list.Values)
            {
                obj.OnDelete();
            }
            obj_list.Clear();
        }

        public static int Count 
        {
            get => obj_list.Count;
        }

        public static void DrawShadows()
        {
            shadowShader.Use();

            foreach (Spotlight spotlight in LightningManager.spotlights)
            {
                int shadowMapSize = spotlight.ShadowMapSpotlight.Size;

                // CREATE MATRICES
                Matrix4 projMatrixFromLight = matrix4f.GetInitPersProjTransform(100, shadowMapSize, shadowMapSize, 0.1f, 100).ToOpenTK();
                vector3f pos = spotlight.PointLight.Position;
                vector3f tar = spotlight.Direction;
                Matrix4 viewMatrixFromLight = (matrix4f.GetInitTranslationTransform(-pos) * matrix4f.GetInitCameraTransform(-tar, vector3f.Up)).ToOpenTK();

                // RENDER SHADOWS 
                spotlight.ShadowMapSpotlight.BindForWriting();

                GL.Viewport(0, 0, shadowMapSize, shadowMapSize);
                GL.Clear(ClearBufferMask.DepthBufferBit);
                
                if (spotlight.mvpMatrixFromLight.Count > 0)
                    spotlight.mvpMatrixFromLight.Clear();

                foreach (string obj_name in obj_list.Keys)
                {
                    obj_list[obj_name].Draw(shadowShader, viewMatrixFromLight, projMatrixFromLight);
                    spotlight.mvpMatrixFromLight.Add(obj_name, obj_list[obj_name]._pipeline.getWorld() * viewMatrixFromLight * projMatrixFromLight);
                }

                GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            }
        }

        public static void DrawScene()
        {
            normalShader.Use();

            for (int i = 0; i < LightningManager.SpotlightsCount; i++)
            {
                LightningManager.spotlights[i].ShadowMapSpotlight.BindForReading(TextureUnit.Texture10 + i);
            }

            foreach (string obj_name in obj_list.Keys)
            {
                for (int i = 0; i < LightningManager.SpotlightsCount; i++)       //ОПТИМИЗИРОВАТЬ
                {
                    if (LightningManager.spotlights[i].mvpMatrixFromLight.Count > 0)
                        normalShader.setValue(
                            "gSpotLights[" + i + "].LightWVP",
                            LightningManager.spotlights[i].mvpMatrixFromLight[obj_name]);
                }
                obj_list[obj_name].Draw(normalShader);
            }
        }

        public static void Reset()
        {
            foreach (AObject obj in obj_list.Values)
            {
                obj.Reset();
            }
        }

        public static bool Exists(string name)
        {
            return obj_list.ContainsKey(name);
        }

        // УСТАНОВИТЬ
        public static void SetAngle(string name, float x, float y, float z)
        {
            obj_list[name].SetAngle(x, y, z);
        }

        public static void SetPosition(string name, float x, float y, float z)
        {
            obj_list[name].SetPosition(x, y, z);
        }

        public static void SetScale(string name, float x, float y, float z)
        {
            obj_list[name].SetScale(x, y, z);
        }

        public static void SetScale(string name, float val)
        {
            obj_list[name].SetScale(val);
        }

        // НЕМЕДЛЕННО ОБНОВИТЬ
        public static void ExpandImmediately(string name, float scaleX, float scaleY, float scaleZ)
        {
            obj_list[name].ExpandImmediately(scaleX, scaleY, scaleZ);
        }

        public static void ExpandImmediately(string name, float value)
        {
            obj_list[name].ExpandImmediately(value);
        }

        public static void RotateImmediately(string name, float angleX, float angleY, float angleZ)
        {
            obj_list[name].RotateImmediately(angleX, angleY, angleZ);
        }

        public static void MoveImmediately(string name, float PosX, float PosY, float PosZ)
        {
            obj_list[name].MoveImmediately(PosX, PosY, PosZ);   
        }

        // СКОРОСТЬ * ВРЕМЯ
        private static void Rotate(string name, float speedX, float speedY, float speedZ, float time)
        {
            obj_list[name].Rotate(speedX, speedY, speedZ, time);
        }

        private static void Move(string name, float speedX, float speedY, float speedZ, float time)
        {
            obj_list[name].Move(speedX, speedY, speedZ, time);
        }

        private static void Expand(string name, float speedX, float speedY, float speedZ, float time)
        {
            obj_list[name].Expand(speedX, speedY, speedZ, time);
        }

        private static void Expand(string name, float speedVal, float time)
        {
            obj_list[name].Expand(speedVal, time);
        }
    }
}
