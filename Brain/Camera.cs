using game_2.MathFolder;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace game_2.Brain
{
    public class Camera
    {
        public vector3f Pos { get; private set; }
        public vector3f Target { get; private set; }
        public vector3f Up { get; private set; }

        private float angle_h;  //горизонтальный поворот
        private float angle_v;  //вертикальный поворот

        private float velocity = 0.01f;
        private float sensitivity = 0.1f;
        private float braking = 0.005f;

        private float speedX;
        private float speedY;
        private float speedZ;

        private float angularX;
        private float angularY;

        public Camera(int window_width, int window_height)
        {
            Pos = vector3f.Zero;
            Target = vector3f.Ford;
            Target.Normalize();
            Up = vector3f.Up;

            speedX = 0.00f;
            speedY = 0.00f;
            speedZ = 0.00f;
            angularX = 0;
            angularY = 0;

            Init();
        }

        public Camera(int window_width, int window_height, vector3f cameraPos, vector3f cameraTarget, vector3f cameraUp)
        {
            Pos = cameraPos;
            Target = cameraTarget;
            Target.Normalize();

            Up = cameraUp;
            Up.Normalize();

            speedX = 0.00f;
            speedY = 0.00f;
            speedZ = 0.00f;
            angularX = 0;
            angularY = 0;

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
        }

        public void OnKeyboard(KeyboardState key)
        {
            if (key.IsKeyDown(Keys.W))
            {
                Pos -= Target * velocity;
            }
            else if (key.IsKeyDown(Keys.S))
            {
                Pos += Target * velocity;
            }
            else if (key.IsKeyDown(Keys.A))
            {
                vector3f left = vector3f.Cross(Target, Up);
                left.Normalize();

                Pos += left * velocity;
            }
            else if (key.IsKeyDown(Keys.D))
            {
                vector3f rigth = vector3f.Cross(Up, Target);
                rigth.Normalize();

                Pos += rigth * velocity;
            }
            else if (key.IsKeyDown(Keys.Space))
            {
                Pos += vector3f.Up * velocity;
            }
            else if (key.IsKeyDown(Keys.LeftShift))
            {
                Pos -= vector3f.Up * velocity;
            }

        }

        public void OnMouse(float DeltaX, float DeltaY)       //сюда реальные координаты мыши, а не дельта
        {
            if ((DeltaX == 0) && (DeltaY == 0)) return;

            angle_h += DeltaX * sensitivity;
            angle_v += DeltaY * sensitivity;

            Update();
        }

        public void OnRender()
        {
            speedX *= 1 - braking;
            speedY *= 1 - braking;
            speedZ *= 1 - braking;

            angularX *= 1 - braking;
            angularY *= 1 - braking;

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
