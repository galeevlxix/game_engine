﻿using game_2.Brain.AssimpFolder;
using game_2.Brain.ObjectFolder;
using game_2.MathFolder;
using System.Reflection;

namespace game_2.Brain
{
    public class ObjectArray
    {
        private Dictionary<string, GameObj> obj_list;

        private const string ModelFolderPath = "C:\\Users\\Lenovo\\source\\repos\\game_2\\Models\\";
        private const string TextureFolderPath = "C:\\Users\\Lenovo\\source\\repos\\game_2\\Textures\\";

        public ObjectArray()
        {
            obj_list = new Dictionary<string, GameObj>();
            Init();
        }

        public ObjectArray(GameObj gameObj)
        {
            obj_list = new Dictionary<string, GameObj>();
            Add("default object", gameObj);
        }

        public ObjectArray(Dictionary<string, GameObj> dict)
        {
            obj_list = dict;
        }

        private void Init()
        {
            int BoxModel = 1;
            int FloorModel = 2;
            int TntModel = 3;
            int TableModel = 4;

            int FieldWidth = 2;

            Add("monkey", new GameObj(ModelFolderPath + "obj_files\\monkey\\monkey.obj", TextureFolderPath + "DefTexture.png"));
            Add("box", new GameObj(BoxModel));
            Add("man", new AssimpObject(ModelFolderPath + "obj_files\\warr\\warr.obj"));
            Add("tnt1", new GameObj(TntModel));
            Add("tnt2", new GameObj(TntModel));
            Add("tnt3", new GameObj(TntModel));
            Add("tnt4", new GameObj(TntModel));
            Add("tnt5", new GameObj(TntModel));
            Add("steve", new GameObj(ModelFolderPath + "obj_files\\Steve.obj", TextureFolderPath + "Copy of steve.png"));
            Add("a_pikagirl", new AssimpObject(ModelFolderPath + "obj_files\\pika-girl\\WithPika.obj"));
            Add("mococo", new AssimpObject(ModelFolderPath + "fbx_files\\Mococo\\Mococo_pose.fbx"));
            Add("ball", new AssimpObject(ModelFolderPath + "obj_files\\Ball\\uploads_files_2222080_ball_obj.obj"));

            Add("field", new FieldFolder.FieldObject(100, FieldFolder.FieldObject.FieldType.Crooked, 4));
            Add("grass", new FieldFolder.FieldObject(5, FieldFolder.FieldObject.FieldType.Flat, 2, TextureFolderPath + "grass.png"));

            SetProperties();
        }

        private void SetProperties()
        {
            this["monkey"].pipeline.SetScale(1f);
            this["monkey"].pipeline.SetPosition(0, 2, 0);
            this["monkey"].pipeline.SetAngle(0, 0, 0);

            this["box"].pipeline.SetScale(1f);
            this["box"].pipeline.SetAngle(0, 0, 0);
            this["box"].pipeline.SetPosition(-6, 1, -6);

            this["man"].pipeline.SetScale(5f);
            this["man"].pipeline.SetAngle(0, 0, 0);
            this["man"].pipeline.SetPosition(3.5f, 0, -15);

            this["tnt1"].pipeline.SetScale(1f);
            this["tnt1"].pipeline.SetAngle(0, 0, 0);
            this["tnt1"].pipeline.SetPosition(-10f, 1, -12);

            this["tnt2"].pipeline.SetScale(1f);
            this["tnt2"].pipeline.SetAngle(0, 0, 0);
            this["tnt2"].pipeline.SetPosition(-12f, 1, -10);

            this["tnt3"].pipeline.SetScale(1f);
            this["tnt3"].pipeline.SetAngle(0, 0, 0);
            this["tnt3"].pipeline.SetPosition(-10f, 1, -8);

            this["tnt4"].pipeline.SetScale(1f);
            this["tnt4"].pipeline.SetAngle(0, 0, 0);
            this["tnt4"].pipeline.SetPosition(-8f, 1, -10);

            this["tnt5"].pipeline.SetScale(1f);
            this["tnt5"].pipeline.SetAngle(0, 0, 0);
            this["tnt5"].pipeline.SetPosition(-10f, 3, -10);

            this["steve"].pipeline.SetScale(0.5f);
            this["steve"].pipeline.SetAngle(0, -45, 0);
            this["steve"].pipeline.SetPosition(-8, -0.2f, -8);

            this["a_pikagirl"].pipeline.SetScale(1f);
            this["a_pikagirl"].pipeline.SetPosition(12, 7.4f, 6);
            this["a_pikagirl"].pipeline.SetAngle(0, 90, 0);

            this["mococo"].pipeline.SetScale(1f);
            this["mococo"].pipeline.SetPosition(12, 0, -6);
            this["mococo"].pipeline.SetAngle(0, -90, 0);

            this["field"].pipeline.SetPosition(0, -10, 0);
            this["field"].pipeline.SetScale(2);

            this["ball"].pipeline.SetPosition(-7, 3, 7);
            this["ball"].pipeline.SetScale(10);
            this["ball"].pipeline.SetAngle(0, 90, 0);

            this["grass"].pipeline.SetScale(8);
        }

        float cube_speed = 10;
        float monkey_rotationSpeed = 90;
        float ball_rotation_speed = 45;
        float ball_position_speed = 0;

        bool cube_moving = true;

        public void OnRender(float deltaTime)
        {
            this["monkey"].pipeline.Rotate(0, monkey_rotationSpeed, 0, deltaTime);

            if (cube_moving)
                if (this["box"].pipeline.PosZ == -6f && this["box"].pipeline.PosX + cube_speed * deltaTime < 6f)
                {
                    this["box"].pipeline.MoveX(cube_speed, deltaTime);
                    if (math3d.abs(this["box"].pipeline.PosX + cube_speed * deltaTime - 6f) < 0.2f)
                        this["box"].pipeline.SetPositionX(6f);
                }
                else if (this["box"].pipeline.PosX == 6f && this["box"].pipeline.PosZ + cube_speed * deltaTime < 6f)
                {
                    this["box"].pipeline.MoveZ(cube_speed, deltaTime);
                    if (math3d.abs(this["box"].pipeline.PosZ + cube_speed * deltaTime - 6f) < 0.2f)
                        this["box"].pipeline.SetPositionZ(6f);
                }
                else if (this["box"].pipeline.PosZ == 6f && this["box"].pipeline.PosX - cube_speed * deltaTime > -6f)
                {
                    this["box"].pipeline.MoveX(-cube_speed, deltaTime);
                    if (math3d.abs(this["box"].pipeline.PosX - cube_speed * deltaTime + 6f) < 0.2f)
                        this["box"].pipeline.SetPositionX(-6f);
                }
                else if (this["box"].pipeline.PosX == -6f && this["box"].pipeline.PosZ - cube_speed * deltaTime > -6f)
                {
                    this["box"].pipeline.MoveZ(-cube_speed, deltaTime);
                    if (math3d.abs(this["box"].pipeline.PosZ - cube_speed * deltaTime + 6f) < 0.2f)
                        this["box"].pipeline.SetPositionZ(-6f);
                }

/*            ball_position_speed += (float)deltaTime * 2;

            if (ball_position_speed >= 2 * Math.PI)
            {
                ball_position_speed = 0;
            }*/

            this["ball"].pipeline.Rotate(0, 0, ball_rotation_speed, deltaTime);
            /*this["ball"].pipeline.SetPositionY(math3d.abs(math3d.sin(ball_position_speed) * 5) + 1f);
            this["ball"].pipeline.SetPositionZ(math3d.cos(ball_position_speed) * 5 + 7);
            counter += deltaTime;*/
            /*if (counter >= 2 * Math.PI) 
                counter = 0;*/
            //this["man"].pipeline.Expand(math3d.sin((float)counter), deltaTime);
        }

        double counter = 0;

        public void Add(string name, GameObj gameObj)
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
            foreach(GameObj obj in obj_list.Values)
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
            foreach(GameObj obj in obj_list.Values)
            {
                obj.Draw();
            }
        }

        public void Reset()
        {
            foreach (GameObj obj in obj_list.Values)
            {
                obj.pipeline.Reset();
            }
        }

        public GameObj this [string name]
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
            obj_list[name].pipeline.SetAngle(x, y, z);
        }

        public void SetPosition(string name, float x, float y, float z)
        {
            obj_list[name].pipeline.SetPosition(x, y, z);
        }

        public void SetScale(string name, float x, float y, float z)
        {
            obj_list[name].pipeline.SetScale(x, y, z);
        }
    }
}
