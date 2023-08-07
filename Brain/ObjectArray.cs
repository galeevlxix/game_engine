using game_2.Brain.ObjectFolder;
using game_2.MathFolder;

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
            int FieldWidth = 2;

            GameObj obj11 = new GameObj(BoxModel);

            GameObj _monkey = new GameObj("C:\\Users\\Lenovo\\source\\repos\\game_2\\Models\\obj_files\\monkey\\monkey.obj", "C:\\Users\\Lenovo\\source\\repos\\game_2\\Models\\obj_files\\monkey\\DefTexture.png");

            GameObj _man = new GameObj("C:\\Users\\Lenovo\\source\\repos\\game_2\\Models\\obj_files\\warr.obj", "C:\\Users\\Lenovo\\source\\repos\\game_2\\Models\\obj_files\\monkey\\CharTexturesHighRes0_029.png");

            Add(_monkey);
            Add(obj11);
            Add(_man);

            for(int i = -FieldWidth; i <= FieldWidth; i++)
            {
                for (int j = -FieldWidth; j <= FieldWidth; j++)
                {
                    GameObj floorObj = new GameObj(FloorModel);
                    floorObj.pipeline.SetPosition(i * 6, 0, j * 6);
                    floorObj.pipeline.SetScale(1);

                    Add(floorObj);
                }
            }

            this[0].pipeline.SetScale(1f);
            this[0].pipeline.SetPosition(0, 2, 0);
            this[0].pipeline.SetAngle(0, 0, 0);

            this[1].pipeline.SetScale(1f);
            this[1].pipeline.SetAngle(0, 0, 0);
            this[1].pipeline.SetPosition(-6, 1, -6);

            this[2].pipeline.SetScale(2f);
            this[2].pipeline.SetAngle(0, 0, 0);
            this[2].pipeline.SetPosition(3.5f, 0, 0);
        }

        float cube_speed = 10;
        float monkey_rotationSpeed = 90;

        bool cube_moving = true;

        public void OnRender(float deltaTime)
        {
            this[0].pipeline.Rotate(0, monkey_rotationSpeed, 0, deltaTime);

            if (cube_moving)
                if (this[1].pipeline.PosZ == -6f && this[1].pipeline.PosX + cube_speed * deltaTime < 6f)
                {
                    this[1].pipeline.MoveX(cube_speed, deltaTime);
                    if (math3d.abs(this[1].pipeline.PosX + cube_speed * deltaTime - 6f) < 0.1f)
                        this[1].pipeline.SetPositionX(6f);
                }
                else if (this[1].pipeline.PosX == 6f && this[1].pipeline.PosZ + cube_speed * deltaTime < 6f)
                {
                    this[1].pipeline.MoveZ(cube_speed, deltaTime);
                    if (math3d.abs(this[1].pipeline.PosZ + cube_speed * deltaTime - 6f) < 0.1f)
                        this[1].pipeline.SetPositionZ(6f);
                }
                else if (this[1].pipeline.PosZ == 6f && this[1].pipeline.PosX - cube_speed * deltaTime > -6f)
                {
                    this[1].pipeline.MoveX(-cube_speed, deltaTime);
                    if (math3d.abs(this[1].pipeline.PosX - cube_speed * deltaTime + 6f) < 0.1f)
                        this[1].pipeline.SetPositionX(-6f);
                }
                else if (this[1].pipeline.PosX == -6f && this[1].pipeline.PosZ - cube_speed * deltaTime > -6f)
                {
                    this[1].pipeline.MoveZ(-cube_speed, deltaTime);
                    if (math3d.abs(this[1].pipeline.PosZ - cube_speed * deltaTime + 6f) < 0.1f)
                        this[1].pipeline.SetPositionZ(-6f);
                }

            counter += deltaTime;
            this[2].pipeline.Expand(math3d.sin((float)counter), deltaTime);
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
                obj_list[i].Draw(30);
            }
        }

        private void Generate()
        {

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
