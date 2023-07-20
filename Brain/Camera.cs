using game_2.MathFolder;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace game_2.Brain
{
    public static class Camera
    {
        public static vector3f Pos { get; set; }
        public static vector3f Target { get; private set; }
        public static vector3f Up { get; private set; }
        private static vector3f Left;

        private static float angle_h;  //горизонтальный поворот
        private static float angle_v;  //вертикальный поворот

        private static float velocity = 0.00015f;
        private static float sensitivity = 0.002f;
        private static float brakingKeyBo = 0.01f;
        private static float brakingMouse = 0.03f;

        private static float zeroLimit = 0.00004f;

        private static float speedX;
        private static float speedY;
        private static float speedZ;

        private static float angularX;
        private static float angularY;

        public static matrix4f CameraTranslation { get; private set; }
        public static matrix4f CameraRotation { get; private set; }

        public static void InitCamera()
        {
            Pos = vector3f.Zero;
            Target = vector3f.Ford;
            Target.Normalize();
            Up = vector3f.Up;

            Left = new vector3f();

            CameraTranslation = new matrix4f();
            CameraRotation = new matrix4f();

            Init();
        }

        public static void InitCamera(vector3f cameraPos, vector3f cameraTarget, vector3f cameraUp)
        {
            Pos = cameraPos;
            Target = cameraTarget;
            Target.Normalize();

            Up = cameraUp;
            Up.Normalize();

            Left = new vector3f();

            CameraTranslation = new matrix4f();
            CameraRotation = new matrix4f();

            Init();
        }

        private static void Init()
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

        public static void OnKeyboard(KeyboardState key)
        {
            if (!key.IsAnyKeyDown) return;

            if (key.IsKeyDown(Keys.LeftControl))
            {
                velocity = 0.00005f;
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

        public static void OnMouse(float DeltaX, float DeltaY)       //сюда реальные координаты мыши, а не дельта
        {
            //if ((DeltaX == 0) && (DeltaY == 0)) return;

            angularX += DeltaX * sensitivity;
            angularY += DeltaY * sensitivity;

        }

        public static void OnRender()
        {
            float m_speedX = speedX * (1 - brakingKeyBo);
            float m_speedY = speedY * (1 - brakingKeyBo);
            float m_speedZ = speedZ * (1 - brakingKeyBo);

            float m_angularX = angularX * (1 - brakingMouse);
            float m_angularY = angularY * (1 - brakingMouse);

            if (m_speedX < zeroLimit && m_speedX > -zeroLimit)
            {
                speedX = 0;
            }
            else
            {
                speedX = m_speedX;
            }

            if (m_speedY < zeroLimit && m_speedY > -zeroLimit)
            {
                speedY = 0;
            }
            else
            {
                speedY = m_speedY;
            }

            if (m_speedZ < zeroLimit && m_speedZ > -zeroLimit)
            {
                speedZ = 0;
            }
            else
            {
                speedZ = m_speedZ;
            }

            if (m_angularX < zeroLimit && m_angularX > -zeroLimit)
            {
                angularX = 0;
            }
            else
            {
                angularX = m_angularX;
            }

            if (m_angularY < zeroLimit && m_angularY > -zeroLimit)
            {
                angularY = 0;
            }
            else
            {
                angularY = m_angularY;
            }

            Pos += Target * speedZ;

            Left = vector3f.Cross(Target, Up);
            Left.Normalize();
            Pos += Left * speedX;

            Pos += vector3f.Up * speedY;

            angle_h += angularX;

            if (angle_v + angularY < 90 && angle_v + angularY > -90)
                angle_v += angularY;

            Update();

            CameraTranslation.InitTranslationTransform(-Pos.x, -Pos.y, -Pos.z);
            CameraRotation.InitCameraTransform(Target, Up);
        }

        private static void Update()
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
