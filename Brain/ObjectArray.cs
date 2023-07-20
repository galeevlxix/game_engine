using game_2.MathFolder;

namespace game_2.Brain
{
    public class ObjectArray
    {
        private List<GameObj> obj_list;

        public ObjectArray()
        {
            obj_list = new List<GameObj>();
        }

        public ObjectArray(GameObj gameObj)
        {
            obj_list = new List<GameObj>();
            Add(gameObj);
        }

        public ObjectArray(List<GameObj> dict)
        {
            this.obj_list = dict;
        }

        public void Add(GameObj gameObj)
        {
            obj_list.Add(gameObj);
        }

        public void Insert(int index, GameObj gameObj)
        {
            obj_list.Insert(index, gameObj);
        }

        public void RemoveAt(int i)
        {
            obj_list.RemoveAt(i);
        }

        public void Remove(GameObj gameObj)
        {
            obj_list.Remove(gameObj);
        }

        public void Clear()
        {
            obj_list.ForEach(a => a.OnDelete());
        }

        public int Count 
        { 
            get 
            { 
                return obj_list.Count; 
            } 
        }

        public void DrawRev()
        {
            for (int i = this.Count - 1; i >= 0; i--)
            {
                obj_list[i].Draw();
            }
        }

        public void Draw()
        {
            for (int i = 0; i < obj_list.Count; i++)
            {
                obj_list[i].Draw();
            }
        }

        public void Reset()
        {
            obj_list.ForEach(obj => obj.pipeline.Reset());
        }

        public GameObj this [int index]
        {
            get
            {
                return obj_list[index];
            }
            set
            {
                obj_list[index] = value;
            }
        }

        public void Rotate(int index, float x, float y, float z)
        {
            obj_list[index].pipeline.Rotate(x, y, z);
        }

        public void Position(int index, float x, float y, float z)
        {
            obj_list[index].pipeline.Position(x, y, z);
        }

        public void Scale(int index, float x, float y, float z)
        {
            obj_list[index].pipeline.Scale(x, y, z);
        }
    }
}
