using game_2.MathFolder;
using OpenTK.Graphics.ES20;
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
            obj_list = dict;
        }

        private void Init()
        {
            int BoxModel = 1;
            int FloorModel = 2;

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

            GameObj _model = new GameObj("C:\\Users\\Lenovo\\source\\repos\\game_2\\Models\\obj_files\\monkey\\monkey.obj", "C:\\Users\\Lenovo\\source\\repos\\game_2\\Models\\obj_files\\monkey\\DefTexture.png");

            GameObj _house = new GameObj("C:\\Users\\Lenovo\\source\\repos\\game_2\\Models\\obj_files\\warr.obj", "C:\\Users\\Lenovo\\source\\repos\\game_2\\Models\\obj_files\\monkey\\CharTexturesHighRes0_029.png");

            //GameObj steve = new GameObj("C:\\Users\\Lenovo\\source\\repos\\game_2\\Models\\obj_files\\Steve.obj", "C:\\Users\\Lenovo\\source\\repos\\game_2\\Models\\obj_files\\Copy of steve.png");
            
            Add(_model);
            Add(obj2);
            Add(obj3);
            Add(obj4);
            Add(obj5);
            Add(obj6);
            Add(obj7);
            Add(obj8);
            Add(obj9);
            Add(obj10);
            Add(obj11);
            Add(_house);
            //Add(steve);

            this[0].pipeline.SetScale(1f);
            this[0].pipeline.SetPosition(0, 2, 0);
            this[0].pipeline.SetAngle(0, 0, 0);

            this[1].pipeline.SetScale(1f);
            this[1].pipeline.SetPosition(-6, 0, -6);
            this[1].pipeline.SetAngle(0, 0, 0);

            this[2].pipeline.SetScale(1f);
            this[2].pipeline.SetPosition(-6, 0, 0);
            this[2].pipeline.SetAngle(0, 0, 0);

            this[3].pipeline.SetScale(1f);
            this[3].pipeline.SetPosition(-6, 0, 6);
            this[3].pipeline.SetAngle(0, 0, 0);

            this[4].pipeline.SetScale(1f);
            this[4].pipeline.SetPosition(0, 0, -6);
            this[4].pipeline.SetAngle(0, 0, 0);

            this[5].pipeline.SetScale(1f);
            this[5].pipeline.SetPosition(0, 0, 0);
            this[5].pipeline.SetAngle(0, 0, 0);

            this[6].pipeline.SetScale(1f);
            this[6].pipeline.SetPosition(0, 0, 6);
            this[6].pipeline.SetAngle(0, 0, 0);

            this[7].pipeline.SetScale(1f);
            this[7].pipeline.SetPosition(6, 0, -6);
            this[7].pipeline.SetAngle(0, 0, 0);

            this[8].pipeline.SetScale(1f);
            this[8].pipeline.SetPosition(6, 0, 0);
            this[8].pipeline.SetAngle(0, 0, 0);

            this[9].pipeline.SetScale(1f);
            this[9].pipeline.SetPosition(6, 0, 6);
            this[9].pipeline.SetAngle(0, 0, 0);

            this[10].pipeline.SetScale(1f);
            this[10].pipeline.SetAngle(0, 0, 0);
            this[10].pipeline.SetPosition(-6, 1, -6);

            this[11].pipeline.SetScale(2f);
            this[11].pipeline.SetAngle(0, 0, 0);
            this[11].pipeline.SetPosition(3.5f, 0, 0);

/*            this[12].pipeline.SetScale(20f);
            this[12].pipeline.SetAngle(0, 0, 0);
            this[12].pipeline.SetPosition(-3.5f, 0, 0);*/
        }

        float cube_speed = 10;
        float monkey_rotationSpeed = 90;

        public void OnRender(double deltaTime)
        {
            this[0].pipeline.Rotate(0, monkey_rotationSpeed, 0, deltaTime);

            if (this[10].pipeline.PosZ == -6f && this[10].pipeline.PosX + 0.01f <= 6f)
            {
                this[10].pipeline.MoveX(cube_speed, deltaTime);
                if (this[10].pipeline.PosX >= 5.9f && this[10].pipeline.PosX <= 6f)
                    this[10].pipeline.SetPositionX(6f);
            }
            else if (this[10].pipeline.PosX == 6f && this[10].pipeline.PosZ + 0.01f <= 6f)
            {
                this[10].pipeline.MoveZ(cube_speed, deltaTime);
                if (this[10].pipeline.PosZ >= 5.9f && this[10].pipeline.PosZ <= 6f) 
                    this[10].pipeline.SetPositionZ(6f);
            }
            else if (this[10].pipeline.PosZ == 6f && this[10].pipeline.PosX - 0.01f >= -6f)
            {
                this[10].pipeline.MoveX(-cube_speed, deltaTime);
                if (this[10].pipeline.PosX <= -5.9f && this[10].pipeline.PosX >= -6f)
                    this[10].pipeline.SetPositionX(-6f);
            }
            else if (this[10].pipeline.PosX == -6f && this[10].pipeline.PosZ - 0.01f >= -6f)
            {
                this[10].pipeline.MoveZ(-cube_speed, deltaTime);
                if (this[10].pipeline.PosZ <= -5.9f && this[10].pipeline.PosZ >= -6f)
                    this[10].pipeline.SetPositionZ(-6f);
            }
            counter += deltaTime;
            this[11].pipeline.Expand(math3d.sin((float)counter), deltaTime);
        }

        double counter = 0;

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

        public void SetAngle(int index, float x, float y, float z)
        {
            obj_list[index].pipeline.SetAngle(x, y, z);
        }

        public void SetPosition(int index, float x, float y, float z)
        {
            obj_list[index].pipeline.SetPosition(x, y, z);
        }

        public void SetScale(int index, float x, float y, float z)
        {
            obj_list[index].pipeline.SetScale(x, y, z);
        }

        public void ShowHitBoxes()
        {
            obj_list.ForEach(obj => obj.ShowHitBox());
        }
        public void HideHitBoxes()
        {
            obj_list.ForEach(obj => obj.HideHitBox());
        }

        bool hb = false;

        public void ShowOrHideHitBoxes()
        {
            if (hb)
            {
                HideHitBoxes();
                hb = false;
            }
            else
            {
                ShowHitBoxes();
                hb = true;
            }
        }
    }
}
