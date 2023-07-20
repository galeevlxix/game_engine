using game_2.MathFolder;
using System.Reflection;

namespace game_2.Brain
{
    public class ObjectArray
    {
        private List<GameObj> obj_list;

        public ObjectArray()
        {
            obj_list = new List<GameObj>();
            Init();
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

        private void Init()
        {
            int BoxModel = 1;
            int FloorModel = 2;

            GameObj obj1 = new GameObj(BoxModel);
            GameObj obj2 = new GameObj(FloorModel);
            GameObj obj3 = new GameObj(FloorModel);
            GameObj obj4 = new GameObj(FloorModel);
            GameObj obj5 = new GameObj(FloorModel);
            GameObj obj6 = new GameObj(FloorModel);
            GameObj obj7 = new GameObj(FloorModel);
            GameObj obj8 = new GameObj(FloorModel);
            GameObj obj9 = new GameObj(FloorModel);
            GameObj obj10 = new GameObj(FloorModel);
            GameObj obj11 = new GameObj(BoxModel);

            this.Add(obj1);
            this.Add(obj2);
            this.Add(obj3);
            this.Add(obj4);
            this.Add(obj5);
            this.Add(obj6);
            this.Add(obj7);
            this.Add(obj8);
            this.Add(obj9);
            this.Add(obj10);
            this.Add(obj11);
        }

        float posx = -6, posz = -6;
        public void OnRender()
        {
            this[0].pipeline.Scale(1f);
            this[0].pipeline.Position(0, 1, 0);
            this[0].pipeline.Rotate(0, GameTime.Time/10, 0);

            this[1].pipeline.Scale(1f);
            this[1].pipeline.Position(-6, 0, -6);
            this[1].pipeline.Rotate(0, 0, 0);

            this[2].pipeline.Scale(1f);
            this[2].pipeline.Position(-6, 0, 0);
            this[2].pipeline.Rotate(0, 0, 0);

            this[3].pipeline.Scale(1f);
            this[3].pipeline.Position(-6, 0, 6);
            this[3].pipeline.Rotate(0, 0, 0);

            this[4].pipeline.Scale(1f);
            this[4].pipeline.Position(0, 0, -6);
            this[4].pipeline.Rotate(0, 0, 0);

            this[5].pipeline.Scale(1f);
            this[5].pipeline.Position(0, 0, 0);
            this[5].pipeline.Rotate(0, 0, 0);

            this[6].pipeline.Scale(1f);
            this[6].pipeline.Position(0, 0, 6);
            this[6].pipeline.Rotate(0, 0, 0);

            this[7].pipeline.Scale(1f);
            this[7].pipeline.Position(6, 0, -6);
            this[7].pipeline.Rotate(0, 0, 0);

            this[8].pipeline.Scale(1f);
            this[8].pipeline.Position(6, 0, 0);
            this[8].pipeline.Rotate(0, 0, 0);

            this[9].pipeline.Scale(1f);
            this[9].pipeline.Position(6, 0, 6);
            this[9].pipeline.Rotate(0, 0, 0);

            if (posz == -6f && posx + 0.01f <= 6f)
            {
                posx += 0.01f;
                if (posx >= 5.9f && posx <= 6f) posx = 6f;
            }
            else if (posx == 6f && posz + 0.01f <= 6f)
            {
                posz += 0.01f;
                if (posz >= 5.9f && posz <= 6f) posz = 6f;
            }
            else if (posz == 6f && posx - 0.01f >= -6f)
            {
                posx += -0.01f;
                if (posx <= -5.9f && posx >= -6f) posx = -6f;
            }
            else if (posx == -6f && posz - 0.01f >= -6f)
            {
                posz += -0.01f;
                if (posz <= -5.9f && posz >= -6f) posz = -6f;
            }

            this[10].pipeline.Scale(1f);
            this[10].pipeline.Position(posx, 1, posz);
            this[10].pipeline.Rotate(0, 0, 0);
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
