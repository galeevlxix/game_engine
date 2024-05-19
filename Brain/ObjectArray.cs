using game_2.Brain.Lights;
using game_2.Brain.Lights.LightStructures;
using game_2.Brain.NewAssimpFolder;
using game_2.MathFolder;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;
using game_2.Brain.Selecting;
using System.Net;

namespace game_2.Brain
{
    public static class ObjectArray
    {
        private static Dictionary<string, AObject> obj_list;

        private static string ModelFolderPath = "..\\..\\..\\Files\\Models\\";

        private static Shader? shadowShader;
        private static Shader? normalShader;
        private static Shader? selectingShader;

        private static SelectingMapFBO selectMap = new SelectingMapFBO();

        public static int WindowWidth, WindowHeight;

        private const float ObservedObjectBaseLightIntensity = 0.2f;

        private static int observed_object_index = -1;
        private static int picked_object_index = -1;
        public static bool pick_mode = false;

        private static List<vector3f> SculpturePositions = new List<vector3f>();
        private static List<vector3f> SculptureAngles = new List<vector3f>();
        private static List<float> SculptureScales = new List<float>();

        public static float ScaleOfPickedObject = 0.01f;
        public static void Init(int Width, int Height)
        {
            shadowShader = CentralizedShaders.GetShader(ShaderName.ShadowShader);
            normalShader = CentralizedShaders.GetShader(ShaderName.AssimpShader);
            selectingShader = CentralizedShaders.GetShader(ShaderName.SelectingShader);

            Console.WriteLine("Загрузка моделей (assimp)...");

            obj_list = new Dictionary<string, AObject>();

            Add("museum", "Museums\\VR_Gallery\\VR_Gallery_comp.obj");
            Add("bull", "Museums\\bull\\bull3.obj");
            Add("table", "Museums\\museum_table\\OPM0032.fbx");

            WindowWidth = Width;
            WindowHeight = Height;

            selectMap.Init(WindowWidth, WindowHeight);

            SetProperties();
        }

        private static void SetProperties()
        {
            if (Exists("museum"))
            {
                SetAngle("museum", 0, 0, 0);
                SetPosition("museum", 20, 0, 0);
                SetScale("museum", 0.01f);
            }

            if (Exists("table"))
            {
                SetAngle("table", 90, 0, 90);
                SetPosition("table", 43, -8.8f, 0);
                SetScale("table", 4);
            }

            if (Exists("bull"))
            {
                SculptureAngles.Add(new vector3f(0, 180, 0));
                SetAngle("bull", 0, 180, 0);
                SculpturePositions.Add(new vector3f(42.8f, -5.75f, -0.5f));
                SetPosition("bull", 42.8f, -5.75f, -0.5f);
                SculptureScales.Add(0.15f);
                SetScale("bull", 0.15f);
                MakeSculpture("bull");
            }
        }

        private static float bull_speedY = 0;
        public static void OnRender(float deltaTime)
        {
            bull_speedY += 3 * deltaTime;
            if (bull_speedY >= 2 * math3d.PI)
                bull_speedY = 0;

            //Move("bull", -math3d.sin(bull_speedY) * 3, 0, 0, deltaTime);
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

        public static SelectingMapFBO.PixelInfo GetObservedPixel()
        {
            selectingShader.Use();

            selectMap.Enable();
            GL.Viewport(0, 0, WindowWidth, WindowHeight);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            int i = 0;
            foreach (AObject obj in obj_list.Values)
            {
                if (!obj.isSculpture) continue;
                selectingShader.setValue("gObjectIndex", i);
                i++;
                obj.Draw(selectingShader);
            }

            selectMap.Disable();

            return selectMap.ReadPixel(WindowWidth / 2, WindowHeight / 2);
        }

        public static void GetObservedObject()
        {
            SelectingMapFBO.PixelInfo pixel = GetObservedPixel();

            if (pixel.PrimID != 0 &&
                pixel.PrimID != 1042066440 &&
                pixel.ObjectID != 1037100384)
            {
                observed_object_index = pixel.ObjectID;
            }
            else 
                observed_object_index = -1;
        }

        public static void GetPickedObject(short mouse_shooter)
        {
            if (mouse_shooter == 1 && observed_object_index != -1 && !pick_mode)
            {
                picked_object_index = observed_object_index;
                pick_mode = true;
                ScaleOfPickedObject = 0.01f;
                AngularX = 180;
                AngularY = 0;
            }
            else if (mouse_shooter == 1 && pick_mode)
            {
                pick_mode = false;
            }
            else if (mouse_shooter == 1)
                picked_object_index = -1;
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
            GL.Viewport(0, 0, WindowWidth, WindowHeight);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            normalShader.Use();

            for (int i = 0; i < LightningManager.SpotlightsCount; i++)
            {
                LightningManager.spotlights[i].ShadowMapSpotlight.BindForReading(TextureUnit.Texture10 + i);
            }

            int sculpt_object_index = 0;

            foreach (string obj_name in obj_list.Keys)
            {
                // ввод матриц lightWVP
                for (int i = 0; i < LightningManager.SpotlightsCount; i++)       //ОПТИМИЗИРОВАТЬ
                {
                    if (LightningManager.spotlights[i].mvpMatrixFromLight.Count > 0)
                        normalShader.setValue("gSpotLights[" + i + "].LightWVP", LightningManager.spotlights[i].mvpMatrixFromLight[obj_name]);
                }

                // отрисовка объектов:
                // включение режима взаимодействия: установить специальные свойства и свет
                if (obj_list[obj_name].isSculpture && sculpt_object_index == picked_object_index && pick_mode)
                {
                    SetPosition(obj_name, Camera.Pos.x - Camera.Target.x / 2, Camera.Pos.y - Camera.Target.y / 2 - ScaleOfPickedObject * 10, Camera.Pos.z - Camera.Target.z / 2);
                    SetScale(obj_name, ScaleOfPickedObject);
                    SetAngle(obj_name, 0, AngularX, AngularY);

                    LightningManager.lightConfig.SetDirectionalLightIntensity(ObservedObjectBaseLightIntensity * 2);
                    vector3f dir = -Camera.Target;
                    dir.y -= 0.5f;
                    dir.Normalize();
                    LightningManager.lightConfig.SetDirectionalLightDirection(dir);
                    LightningManager.lightConfig.SetBaseLightIntensity(ObservedObjectBaseLightIntensity / 2);
                    obj_list[obj_name].Draw(normalShader);
                    LightningManager.lightConfig.SetDirectionalLightIntensity(LightningManager.directionalLight.BaseLight.Intensity);
                    LightningManager.lightConfig.SetDirectionalLightDirection(LightningManager.directionalLight.Direction);
                    LightningManager.lightConfig.SetBaseLightIntensity(LightningManager.baseLight.Intensity);

                    sculpt_object_index++;
                    continue;
                }
                // выход из режима взаимодействия: установить начальные свойства и разорвать связь с выбранным объектом
                else if (obj_list[obj_name].isSculpture && sculpt_object_index == picked_object_index && !pick_mode)
                {
                    SetPosition(obj_name, SculpturePositions[sculpt_object_index]);
                    SetScale(obj_name, SculptureScales[sculpt_object_index]);
                    SetAngle(obj_name, SculptureAngles[sculpt_object_index]);
                    picked_object_index = -1;
                    sculpt_object_index++;
                }
                // упал взгляд на объект скульптуры
                else if (obj_list[obj_name].isSculpture && sculpt_object_index == observed_object_index && !pick_mode)
                {
                    LightningManager.lightConfig.SetBaseLightIntensity(ObservedObjectBaseLightIntensity);
                    obj_list[obj_name].Draw(normalShader);
                    LightningManager.lightConfig.SetBaseLightIntensity(LightningManager.baseLight.Intensity);

                    sculpt_object_index++;
                    continue;
                }

                obj_list[obj_name].Draw(normalShader);
            }
        }

        private static float AngularX = 0;
        private static float AngularY = 0;


        public static void RotatePickedObject(float dX, float dY)
        {
            if (dX != 0 || dY != 0)
            {
                AngularX += dX / 8;
                AngularY += dY / 32;
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

        public static void SetAngle(string name, vector3f angle)
        {
            obj_list[name].SetAngle(angle.x, angle.y, angle.z);
        }

        public static void SetPosition(string name, float x, float y, float z)
        {
            obj_list[name].SetPosition(x, y, z);
        }

        public static void SetPosition(string name, vector3f position)
        {
            obj_list[name].SetPosition(position.x, position.y, position.z);
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

        private static void MakeSculpture(string name)
        {
            obj_list[name].isSculpture = true;
        }

        public static void Resize(int width, int height)
        {
            WindowWidth = width;
            WindowHeight = height;

            selectMap.Init(width, height);
        }
    }
}
