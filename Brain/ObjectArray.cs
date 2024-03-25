using game_2.Brain.NewAssimpFolder;
using game_2.Brain.ObjectFolder;
using game_2.MathFolder;

namespace game_2.Brain
{
    public class ObjectArray
    {
        private Dictionary<string, AObject> obj_list;

        protected const string ModelFolderPath = "..\\..\\..\\Files\\Models\\";
        protected const string TextureFolderPath = "..\\..\\..\\Files\\Textures\\";

        public ObjectArray()
        {
            obj_list = new Dictionary<string, AObject>();
            Init();
        }

        public ObjectArray(AObject gameObj)
        {
            obj_list = new Dictionary<string, AObject>();
            Add("default object", gameObj);
        }

        public ObjectArray(Dictionary<string, AObject> dict)
        {
            obj_list = dict;
        }

        private void Init()
        {
            Add("lion", new AObject("C:\\Users\\Lenovo\\source\\repos\\game_2\\Files\\Models\\obj_files\\lion\\source\\model.obj"));
            Add("monkey", new AObject("C:\\Users\\Lenovo\\source\\repos\\game_2\\Files\\Models\\obj_files\\monkey\\monkey.obj"));
            
            SetProperties();
        }

        private void SetProperties()
        {
            this["lion"].SetScale(0.01f);
            this["lion"].SetPosition(10, 0, 0);

            //this["dust"].SetScale(0.01f);
        }

        public void OnRender(float deltaTime)
        {

        }

        double counter = 0;

        public void Add(string name, AObject gameObj)
        {
            obj_list.Add(name, gameObj);
        }

        public void Remove(string name)
        {
            obj_list[name].OnDelete();
            obj_list.Remove(name);
        }

        public void Clear()
        {
            foreach(AObject obj in obj_list.Values)
            {
                obj.OnDelete();
            }
            obj_list.Clear();
        }

        public int Count 
        { 
            get 
            { 
                return obj_list.Count; 
            } 
        }

        public void Draw()
        {
            foreach(AObject obj in obj_list.Values)
            {
                obj.Draw();
            }
        }

        public void Reset()
        {
            foreach (AObject obj in obj_list.Values)
            {
                obj.Reset();
            }
        }

        public AObject this [string name]
        {
            get
            {
                return obj_list[name];
            }
            set
            {
                obj_list[name] = value;
            }
        }

        public void SetAngle(string name, float x, float y, float z)
        {
            obj_list[name].SetAngle(x, y, z);
        }

        public void SetPosition(string name, float x, float y, float z)
        {
            obj_list[name].SetPosition(x, y, z);
        }

        public void SetScale(string name, float x, float y, float z)
        {
            obj_list[name].SetScale(x, y, z);
        }
    }
}
