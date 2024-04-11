using game_2.Brain.Lights.LightStructures;
using game_2.Brain.NewAssimpFolder;
using game_2.Brain.ObjectFolder;
using game_2.MathFolder;

namespace game_2.Brain
{
    public static class ObjectArray
    {
        private static Dictionary<string, AObject> obj_list;

        private static string ModelFolderPath = "..\\..\\..\\Files\\Models\\";
        private static string TextureFolderPath = "..\\..\\..\\Files\\Textures\\";

        public static void Init()
        {
            obj_list = new Dictionary<string, AObject>();

            Add("back", new AObject(ModelFolderPath + "obj_files\\background\\cube.obj"));

            //Add("ball", new AObject(ModelFolderPath + "obj_files\\Ball\\ball1.obj"));

            //Add("de_dust", new AObject(ModelFolderPath + "obj_files\\de_dust\\source\\new_de_dust2_comp.obj"));
            //Add("monkey", new AObject(ModelFolderPath + "obj_files\\monkey\\monkey.obj"));
            //Add("pika", new AObject("C:\\Users\\Lenovo\\source\\repos\\game_2\\Files\\Models\\obj_files\\pika-girl\\WithPika2.obj"));
            
            
            //Add("ball", new AObject("C:\\Users\\Lenovo\\source\\repos\\game_2\\Files\\Models\\obj_files\\Ball\\ball1.obj"));
            //Add("dingel", new AObject("C:\\Users\\Lenovo\\source\\repos\\game_2\\Files\\Models\\fbx_files\\Dingel\\source\\Dingel_comp.obj"));
            //Add("mococo", new AObject("C:\\Users\\Lenovo\\source\\repos\\game_2\\Files\\Models\\fbx_files\\Mococo\\Mococo_pose.fbx"));
            //Add("sphere", new AObject("C:\\Users\\Lenovo\\source\\repos\\game_2\\Files\\Models\\fbx_files\\stylized-organic-red\\source\\Stylizedground_sphere_comp.obj"));

            SetProperties();
        }

        private static void SetProperties()
        {
            SetAngle("back", 0, -90, 0);
            SetPosition("back", 12, 5, 0);
            SetScale("back", 5);

            //SetScale("de_dust", 0.05f);
            //SetAngle("de_dust", 90, 0, 0);
            //SetPosition("de_dust", -20, 0, 0);

            //SetPosition("monkey", 0, 4, 0);
            //SetScale("monkey", 0.8f);

            //SetPosition("pika", 12, 7.5f, 12);
            //SetAngle("pika", 0, 90, 0);

            //SetScale("ball", 12);
            //SetPosition("ball", -5, 4, 10);

            //SetPosition("ball", 13, 1, 0);
            //SetAngle("ball", 0, 90, 0);
            //SetScale("ball", 0);

            //SetPosition("mococo", 10, 0, -5);
            //SetAngle("mococo", 0, -90, 0);
            //SetScale("mococo", 0.8f);

            //SetScale("sphere", 0.05f);
            //SetPosition("sphere", -9, 5, 0);
        }

        private static float rot_speed = 45;
        public static void OnRender(float deltaTime)
        {
            //Rotate("monkey", 0, rot_speed, 0, deltaTime);
        }

        public static void Add(string name, AObject gameObj)
        {
            obj_list.Add(name, gameObj);
            Console.WriteLine("     Загружена модель " + name);
        }

        public static void Remove(string name)
        {
            obj_list[name].OnDelete();
            obj_list.Remove(name);
        }

        public static void Clear()
        {
            foreach(AObject obj in obj_list.Values)
            {
                obj.OnDelete();
            }
            obj_list.Clear();
        }

        public static int Count 
        { 
            get 
            { 
                return obj_list.Count; 
            } 
        }

        public static void Draw()
        {
            foreach(AObject obj in obj_list.Values)
            {
                obj.Draw();
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
