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

            Add("elep", new AObject(ModelFolderPath + "obj_files\\lion\\source\\model.obj"));
            Add("monkey", new AObject(ModelFolderPath + "obj_files\\monkey\\monkey.obj"));
            
            SetProperties();
        }

        private static void SetProperties()
        {
            SetScale("elep", 0.01f);
            SetPosition("elep", 10, 0, 0);

            SetScale("monkey", 2);
        }

        public static void OnRender(float deltaTime)
        {

        }

        public static void Add(string name, AObject gameObj)
        {
            obj_list.Add(name, gameObj);
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
