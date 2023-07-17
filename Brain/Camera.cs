using game_2.MathFolder;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace game_2.Brain
{
    public class Camera
    {
        public vector3f Pos { get; private set; }
        public vector3f Target { get; private set; }
        public vector3f Up { get; private set; }

        private vector3f Left = new vector3f();

        private float angle_h;  //горизонтальный поворот
        private float angle_v;  //вертикальный поворот

        private float velocity = 0.00015f;
        private float sensitivity = 0.002f;
        private float brakingKeyBo = 0.01f;
        private float brakingMouse = 0.04f;

        private float speedX;
        private float speedY;
        private float speedZ;

        private float angularX;
        private float angularY;

        public Camera()
        {
            Pos = vector3f.Zero;
            Target = vector3f.Ford;
            Target.Normalize();
            Up = vector3f.Up;

            Init();
        }

        public Camera(vector3f cameraPos, vector3f cameraTarget, vector3f cameraUp)
        {
            Pos = cameraPos;
            Target = cameraTarget;
            Target.Normalize();

            Up = cameraUp;
            Up.Normalize();

            Init();
        }

        private void Init()
        {
            vector3f HTarget = new vector3f(Target.x, 0, Target.z);
            HTarget.Normalize();

            if (HTarget.z >= 0.0f)
            {
                if (HTarget.x >= 0.0f)
                {
                    angle_h = 360.0f - math3d.ToDegree(math3d.asin(HTarget.z));
                }
                else
                {
                    angle_h = 180.0f + math3d.ToDegree(math3d.asin(HTarget.z));
                }
            }
            else
            {
                if (HTarget.x >= 0.0f)
                {
                    angle_h = math3d.ToDegree(math3d.asin(-HTarget.z));
                }
                else
                {
                    angle_h = 90.0f + math3d.ToDegree(math3d.asin(-HTarget.z));
                }
            }

            angle_v = -math3d.ToDegree(math3d.asin(Target.y));

            speedX = 0.00f;
            speedY = 0.00f;
            speedZ = 0.00f;
            angularX = 0;
            angularY = 0;
        }

        public void OnKeyboard(KeyboardState key)
        {
            if (key.IsKeyDown(Keys.LeftControl))
            {
                velocity = 0.00001f;
            }
            else
            {
                velocity = 0.00015f;
            }

            if (key.IsKeyDown(Keys.W))
            {
                speedZ -= velocity;
            }
            if (key.IsKeyDown(Keys.S))
            {
                speedZ += velocity;
            }
            if (key.IsKeyDown(Keys.A))
            {
                speedX += velocity;
            }
            if (key.IsKeyDown(Keys.D))
            {
                speedX -= velocity;
            }
            if (key.IsKeyDown(Keys.Space))
            {
                speedY += velocity;
            }
            if (key.IsKeyDown(Keys.LeftShift))
            {
                speedY -= velocity;
            }
        }

        public void OnMouse(float DeltaX, float DeltaY)       //сюда реальные координаты мыши, а не дельта
        {
            //if ((DeltaX == 0) && (DeltaY == 0)) return;

            angularX += DeltaX * sensitivity;
            angularY += DeltaY * sensitivity;

            Update();
        }

        public void OnRender()
        {
            speedX *= 1 - brakingKeyBo;
            speedY *= 1 - brakingKeyBo;
            speedZ *= 1 - brakingKeyBo;

            angularX *= 1 - brakingMouse;
            angularY *= 1 - brakingMouse;

            Pos += Target * speedZ;

            Left = vector3f.Cross(Target, Up);
            Left.Normalize();
            Pos += Left * speedX;

            Pos += vector3f.Up * speedY;

            angle_h += angularX;

            if (angle_v + angularY < 90 && angle_v + angularY > -90)
                angle_v += angularY;
        }

        private void Update()
        {
            vector3f Vaxis = vector3f.Up;

            // Поворачиваем вектор вида на горизонтальный угол вокруг вертикальной оси
            vector3f View = vector3f.Right;
            View.Rotate(angle_h, Vaxis);
            View.Normalize();

            // Поворачиваем вектор вида на вертикальный угол вокруг горизонтальной оси
            vector3f Haxis = vector3f.Cross(Vaxis, View);
            Haxis.Normalize();
            View.Rotate(angle_v, Haxis);

            Target = View;
            Target.Normalize();

            Up = vector3f.Cross(Target, Haxis);
            Up.Normalize();
        }
    }
}
