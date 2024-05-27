using game_2.Brain.Lights;
using game_2.Brain.Lights.LightStructures;
using game_2.Brain.NewAssimpFolder;
using game_2.MathFolder;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;
using game_2.Brain.Selecting;

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

        private static vector3f pickedObjectPosition = new vector3f();

        public static void Init(int Width, int Height)
        {
            shadowShader = CentralizedShaders.GetShader(ShaderName.ShadowShader);
            normalShader = CentralizedShaders.GetShader(ShaderName.AssimpShader);
            selectingShader = CentralizedShaders.GetShader(ShaderName.SelectingShader);

            Console.WriteLine("Загрузка моделей (assimp)...");

            obj_list = new Dictionary<string, AObject>();

            Add("museum", "Museums\\VR_Gallery\\VR_Gallery_comp.obj");
            Add("table1", "Museums\\museum_table\\OPM0032_fin.obj");
            Add("table2", "Museums\\museum_table\\OPM0032_fin.obj");
            Add("table3", "Museums\\museum_table\\OPM0032_fin.obj");
            Add("table4", "Museums\\museum_table\\OPM0032_fin.obj");
            Add("table5", "Museums\\museum_table\\OPM0032_fin.obj");
            Add("table6", "Museums\\museum_table\\OPM0032_fin.obj");
            
            Add("bull", "Museums\\bull\\bull5.obj");
            Add("hans", "Museums\\st1\\HansChristianAndersen-80k_rot.obj");
            Add("head", "Museums\\st2\\MCh_S_12_Rzezba_Popiersie_Rozy_Loewenfeld_fin.obj");
            Add("goat", "Museums\\st3\\goat_rot.obj");
            Add("thinker", "Museums\\st4\\Rodin_Thinker_fin.obj");
            Add("laocoon", "Museums\\st5\\Laocoon-and-his-sons_rot.obj");
            
            WindowWidth = Width;
            WindowHeight = Height;

            selectMap.Init(WindowWidth, WindowHeight);

            foreach(Spotlight spotlight in LightningManager.spotlights)
            {
                spotlight.wvpMatrixFromLight = new Dictionary<string, Matrix4>();
                
                foreach (string obj_name in obj_list.Keys)
                {
                    spotlight.wvpMatrixFromLight.Add(obj_name, Matrix4.Identity);
                }
            }

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

            if (Exists("table1"))
            {
                SetAngle("table1", 0, -90, 0);
                SetPosition("table1", 43, -7.15f, 0);
                SetScale("table1", 0.21f);
            }

            if (Exists("table2"))
            {
                SetAngle("table2", 0, 0, 0);
                SetPosition("table2", 27.5f, -7.15f, -5.5f);
                SetScale("table2", 0.21f);
            }

            if (Exists("table3"))
            {
                SetAngle("table3", 0, 180, 0);
                SetPosition("table3", 27.5f, -7.15f, 5.5f);
                SetScale("table3", 0.21f);
            }

            if (Exists("table4"))
            {
                SetAngle("table4", 0, 180, 0);
                SetPosition("table4", 12.5f, -7.15f, 5.5f);
                SetScale("table4", 0.21f);
            }

            if (Exists("table5"))
            {
                SetAngle("table5", 0, 0, 0);
                SetPosition("table5", 12.5f, -7.15f, -5.5f);
                SetScale("table5", 0.21f);
            }

            if (Exists("table6"))
            {
                SetAngle("table6", 0, 90, 0);
                SetPosition("table6", -3, -7.15f, 0);
                SetScale("table6", 0.21f);
            }

            if (Exists("bull"))
            {
                SculptureAngles.Add(new vector3f(0, 180, 0));
                SetAngle("bull", SculptureAngles[SculptureAngles.Count - 1]);

                SculpturePositions.Add(new vector3f(42.85f, -4.51f, 0f));
                SetPosition("bull", SculpturePositions[SculpturePositions.Count - 1]);

                SculptureScales.Add(0.1f);
                SetScale("bull", SculptureScales[SculptureScales.Count - 1]);

                MakeSculpture("bull");
            }

            if (Exists("hans"))
            {
                SculptureAngles.Add(new vector3f(0, -11, 0));
                SetAngle("hans", SculptureAngles[SculptureAngles.Count - 1]);

                SculpturePositions.Add(new vector3f(12.3f, -3.9f, -5.5f));
                SetPosition("hans", SculpturePositions[SculpturePositions.Count - 1]);

                SculptureScales.Add(0.13f);
                SetScale("hans", SculptureScales[SculptureScales.Count - 1]);

                MakeSculpture("hans");
            }

            if (Exists("head"))
            {
                SculptureAngles.Add(new vector3f(0, 180, 0));
                SetAngle("head", SculptureAngles[SculptureAngles.Count - 1]);

                SculpturePositions.Add(new vector3f(12.3f, -3.8f, 5.5f));
                SetPosition("head", SculpturePositions[SculpturePositions.Count - 1]);

                SculptureScales.Add(0.13f);
                SetScale("head", SculptureScales[SculptureScales.Count - 1]);

                MakeSculpture("head");
            }

            if (Exists("goat"))
            {
                SculptureAngles.Add(new vector3f(0, 180, 0));
                SetAngle("goat", SculptureAngles[SculptureAngles.Count - 1]);

                SculpturePositions.Add(new vector3f(27.4f, -3.7f, 5.3f));
                SetPosition("goat", SculpturePositions[SculpturePositions.Count - 1]);

                SculptureScales.Add(0.08f);
                SetScale("goat", SculptureScales[SculptureScales.Count - 1]);

                MakeSculpture("goat");
            }

            if (Exists("thinker"))
            {
                SculptureAngles.Add(new vector3f(0, 60, 0));
                SetAngle("thinker", SculptureAngles[SculptureAngles.Count - 1]);

                SculpturePositions.Add(new vector3f(27.5f, -5.15f + 0.6f, -5.5f));
                SetPosition("thinker", SculpturePositions[SculpturePositions.Count - 1]);

                SculptureScales.Add(0.1f);
                SetScale("thinker", SculptureScales[SculptureScales.Count - 1]);

                MakeSculpture("thinker");
            }

            if (Exists("laocoon"))
            {
                SculptureAngles.Add(new vector3f(0, 0, 0));
                SetAngle("laocoon", SculptureAngles[SculptureAngles.Count - 1]);

                SculpturePositions.Add(new vector3f(-3, -3.15f + 0.07f, 0));
                SetPosition("laocoon", SculpturePositions[SculpturePositions.Count - 1]);

                SculptureScales.Add(0.08f);
                SetScale("laocoon", SculptureScales[SculptureScales.Count - 1]);

                MakeSculpture("laocoon");
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
                if (obj.isSculpture)
                {
                    selectingShader.setValue("gObjectIndex", i);
                    i++;
                    obj.Draw(selectingShader);
                }
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
                AngularX = 0;
                pickedObjectPosition = Camera.Pos - Camera.Target / 2;
                pickedObjectPosition.y = Camera.player_height;
            }
            else if (mouse_shooter == 1 && pick_mode)
            {
                pick_mode = false;
            }
            else if (mouse_shooter == 1)
                picked_object_index = -1;
        }

        static bool shadows_exist = false;

        public static void DrawShadows()
        {
            shadowShader.Use();

            foreach (Spotlight spotlight in LightningManager.spotlights)
            {
                int shadowMapSize = spotlight.ShadowMapSpotlight.Size;

                // CREATE MATRICES
                Matrix4 projMatrixFromLight = matrix4f.GetInitPersProjTransform(75, shadowMapSize, shadowMapSize, 0.1f, 100).ToOpenTK();
                vector3f pos = spotlight.PointLight.Position;
                vector3f tar = spotlight.Direction;
                vector3f up = vector3f.Cross(tar, vector3f.Right);
                Matrix4 viewMatrixFromLight = (matrix4f.GetInitTranslationTransform(-pos) * matrix4f.GetInitCameraTransform(-tar, -up)).ToOpenTK();

                // RENDER SHADOWS 
                spotlight.ShadowMapSpotlight.BindForWriting(); 

                GL.Viewport(0, 0, shadowMapSize, shadowMapSize);
                GL.Clear(ClearBufferMask.DepthBufferBit);
                
                foreach (string obj_name in obj_list.Keys)
                {
                    Matrix4 WVPFromLight = obj_list[obj_name]._pipeline.getWorld() * viewMatrixFromLight * projMatrixFromLight;
                    obj_list[obj_name].Draw(shadowShader, viewMatrixFromLight, projMatrixFromLight);
                    spotlight.wvpMatrixFromLight[obj_name] = WVPFromLight;
                }

                GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

                shadows_exist = true;
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
                    if (shadows_exist)
                        normalShader.setValue("gSpotLights[" + i + "].LightWVP", LightningManager.spotlights[i].wvpMatrixFromLight[obj_name]);
                }

                // отрисовка объектов:
                // включение режима взаимодействия: установить специальные свойства и свет
                if (obj_list[obj_name].isSculpture && sculpt_object_index == picked_object_index && pick_mode)
                {
                    SetPosition(obj_name, pickedObjectPosition);
                    SetScale(obj_name, ScaleOfPickedObject);

                    SetAngle(obj_name, SculptureAngles[sculpt_object_index].x, SculptureAngles[sculpt_object_index].y + AngularX, SculptureAngles[sculpt_object_index].z);

                    LightningManager.lightConfig.SetDirectionalLightIntensity(ObservedObjectBaseLightIntensity * 2);
                    vector3f dir = -Camera.Target;
                    dir.y -= 0.5f;
                    dir.Normalize();
                    LightningManager.lightConfig.SetDirectionalLightDirection(dir);
                    LightningManager.lightConfig.SetBaseLightIntensity(ObservedObjectBaseLightIntensity / 2);
                    CentralizedShaders.SetValue(ShaderName.AssimpShader, "TurnOnShadows", 0);
                    obj_list[obj_name].Draw(normalShader);
                    CentralizedShaders.SetValue(ShaderName.AssimpShader, "TurnOnShadows", 1);
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
                    LightningManager.lightConfig.SetBaseLightIntensity(LightningManager.baseLight.Intensity + ObservedObjectBaseLightIntensity);
                    obj_list[obj_name].Draw(normalShader);
                    LightningManager.lightConfig.SetBaseLightIntensity(LightningManager.baseLight.Intensity);

                    sculpt_object_index++;
                    continue;
                }
                else if (obj_list[obj_name].isSculpture)
                {
                    sculpt_object_index++;
                }

                obj_list[obj_name].Draw(normalShader);
            }
        }

        private static float AngularX = 0;

        public static void RotatePickedObject(float dX)
        {
            if (dX != 0)
            {
                AngularX += dX;
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

        private static void MakeMuseum(string name)
        {
            obj_list[name].isMuseum = true;
        }

        public static void Resize(int width, int height)
        {
            WindowWidth = width;
            WindowHeight = height;

            selectMap.Init(width, height);
        }
    }
}
