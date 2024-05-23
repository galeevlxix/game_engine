using game_2.MathFolder;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace game_2.Brain
{
    public static class Camera
    {
        public static vector3f Pos { get; private set; }
        public static vector3f Target { get; private set; }
        public static vector3f Up { get; private set; }

        public static void SetCameraPosition(float x, float y, float z)
        {
            Pos.y = y; Pos.x = x; Pos.z = z;
        }

        private static vector3f Left;

        private static float angle_h;  //горизонтальный поворот
        private static float angle_v;  //вертикальный поворот

        private static float velocity;
        private static float sensitivity = 0.5f;
        private static float brakingKeyBo = 5f;
        private static float brakingMouse = 20f;

        private static float min_speed = 0.00004f;
        private static float max_speed;

        private static float max_normal_speed = 10;
        private static float max_fast_speed = 15;

        private static float normal_velocity = 60f;
        private static float fast_velocity = 90f;

        private static float speedX;
        private static float speedY;
        private static float speedZ;

        private static float angularX;
        private static float angularY;

        public static matrix4f CameraTranslation { get; private set; }
        public static matrix4f CameraRotation { get; private set; }

        public static void InitCamera()
        {
            Console.WriteLine("Загрузка камеры...");

            Pos = vector3f.Zero;
            Target = new vector3f (0, 0 , -1);
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

        private static bool isCrawl = false;

        public static void OnKeyboard(KeyboardState key, float deltaTime)
        {
            if (!key.IsAnyKeyDown) return;

            // TODO оптимизировать
            if (key.IsKeyDown(Keys.RightControl) || key.IsKeyDown(Keys.LeftControl))
            {
                velocity = fast_velocity;
                max_speed = max_fast_speed;
            }
            else
            {
                velocity = normal_velocity;
                max_speed = max_normal_speed;
            }

            if (key.IsKeyDown(Keys.Space))
            {
                isCrawl = false;
            }
            if (key.IsKeyDown(Keys.LeftShift))
            {
                isCrawl = true;
            }

            // перемещение в нескольких направлениях
            // TODO: доделать с вертикальными направлениями (2 и 3 направления)

            if (key.IsKeyDown(Keys.W) && key.IsKeyDown(Keys.D))
            {
                if ((speedX - velocity * deltaTime) * (speedX - velocity * deltaTime) + speedY * speedY + (speedZ - velocity * deltaTime) * (speedZ - velocity * deltaTime) <= max_speed * max_speed)
                {
                    speedZ -= velocity * deltaTime;
                    speedX -= velocity * deltaTime;
                }
                return;
            }
            else if (key.IsKeyDown(Keys.W) && key.IsKeyDown(Keys.A))
            {
                if ((speedX + velocity * deltaTime) * (speedX + velocity * deltaTime) + speedY * speedY + (speedZ - velocity * deltaTime) * (speedZ - velocity * deltaTime) <= max_speed * max_speed)
                {
                    speedZ -= velocity * deltaTime;
                    speedX += velocity * deltaTime;
                }
                return;
            }
            else if (key.IsKeyDown(Keys.S) && key.IsKeyDown(Keys.A))
            {
                if ((speedZ + velocity * deltaTime) * (speedZ + velocity * deltaTime) + speedY * speedY + (speedZ + velocity * deltaTime) * (speedZ + velocity * deltaTime) <= max_speed * max_speed)
                {
                    speedZ += velocity * deltaTime;
                    speedX += velocity * deltaTime;
                }
                return;
            }
            else if (key.IsKeyDown(Keys.S) && key.IsKeyDown(Keys.D))
            {
                if ((speedX - velocity * deltaTime) * (speedX - velocity * deltaTime) + speedY * speedY + (speedZ + velocity * deltaTime) * (speedZ + velocity * deltaTime) <= max_speed * max_speed)
                {
                    speedZ += velocity * deltaTime;
                    speedX -= velocity * deltaTime;
                }
                return;
            }

            if (key.IsKeyDown(Keys.W))
            {
                if (speedX * speedX + speedY * speedY + (speedZ - velocity * deltaTime) * (speedZ - velocity * deltaTime) <= max_speed * max_speed)
                    speedZ -= velocity * deltaTime;
            } 
            if (key.IsKeyDown(Keys.S))
            {
                if (speedX * speedX + speedY * speedY + (speedZ + velocity * deltaTime) * (speedZ + velocity * deltaTime) <= max_speed * max_speed)
                    speedZ += velocity * deltaTime;
            }
            if (key.IsKeyDown(Keys.A))
            {
                if ((speedX + velocity * deltaTime) * (speedX + velocity * deltaTime) + speedY * speedY + speedZ * speedZ <= max_speed * max_speed)
                    speedX += velocity * deltaTime;
            }
            if (key.IsKeyDown(Keys.D))
            {
                if ((speedX - velocity * deltaTime) * (speedX - velocity * deltaTime) + speedY * speedY + speedZ * speedZ <= max_speed * max_speed)
                    speedX -= velocity * deltaTime;
            }
            //убрать
            if (key.IsKeyDown(Keys.Space))
            {
                if (speedX * speedX + (speedY + velocity * deltaTime) * (speedY + velocity * deltaTime) + speedZ * speedZ <= max_speed * max_speed)
                    speedY += velocity * deltaTime;
            }
            if (key.IsKeyDown(Keys.LeftShift))
            {
                if (speedX * speedX + (speedY - velocity * deltaTime) * (speedY - velocity * deltaTime) + speedZ * speedZ <= max_speed * max_speed)
                    speedY -= velocity * deltaTime;
            }
        }

        public static void OnMouse(float DeltaX, float DeltaY)
        {
            angularX += DeltaX * sensitivity;
            angularY += DeltaY * sensitivity;
        }

        public static void OnRender(float deltaTime)
        {
            Braking(deltaTime);

            Pos = SmartMoving(Pos, Target * speedZ * deltaTime);

            Left = vector3f.Cross(Target, Up);
            Left.Normalize();
            Pos = SmartMoving(Pos, Left * speedX * deltaTime);

            Pos = SmartMoving(Pos, vector3f.Up * speedY * deltaTime);

            if (isCrawl && player_height >= min_point.y)
            {
                MakeCrawl(deltaTime);
            } 
            else if (!isCrawl && player_height <= max_point.y)
            {
                MakeGetUp(deltaTime);
            }

            angle_h += angularX * deltaTime;

            if (angle_v + angularY * deltaTime < 90 && angle_v + angularY * deltaTime > -90)
                angle_v += angularY * deltaTime;

            Update();

            CameraTranslation.InitTranslationTransform(-Pos);
            CameraRotation.InitCameraTransform(Target, Up);
        }

        // границы карты музея
        private static vector3f min_point = new vector3f(-1f, -6f, -6f);
        private static vector3f max_point = new vector3f(40.5f, -4f, 6f);
        
        // перемещение left в пределах границ 
        private static vector3f SmartMoving(vector3f left, vector3f right)
        {
            vector3f res = left;

            //res.y = player_height;
            res.y = left.y + right.y;

            //if (left.x + right.x < max_point.x && left.x + right.x > min_point.x)
            res.x = left.x + right.x;

            //if (left.z + right.z < max_point.z && left.z + right.z > min_point.z)
                res.z = left.z + right.z;

            return res;
        }

        private static float player_height = -4;

        public static void MakeCrawl(float deltaTime)
        {
            player_height -= max_normal_speed * deltaTime;
        }

        public static void MakeGetUp(float deltaTime)
        {
            player_height += max_normal_speed * deltaTime;
        }

        private static void Braking(float deltaTime)
        {
            float m_speedX = speedX * (1 - brakingKeyBo * deltaTime);
            float m_speedY = speedY * (1 - brakingKeyBo * deltaTime);
            float m_speedZ = speedZ * (1 - brakingKeyBo * deltaTime);

            float m_angularX = angularX * (1 - brakingMouse * deltaTime);
            float m_angularY = angularY * (1 - brakingMouse * deltaTime);

            if (m_speedX < min_speed && m_speedX > -min_speed)
            {
                speedX = 0;
            }
            else
            {
                speedX = m_speedX;
            }

            if (m_speedY < min_speed && m_speedY > -min_speed)
            {
                speedY = 0;
            }
            else
            {
                speedY = m_speedY;
            }

            if (m_speedZ < min_speed && m_speedZ > -min_speed)
            {
                speedZ = 0;
            }
            else
            {
                speedZ = m_speedZ;
            }

            if (m_angularX < min_speed && m_angularX > -min_speed)
            {
                angularX = 0;
            }
            else
            {
                angularX = m_angularX;
            }

            if (m_angularY < min_speed && m_angularY > -min_speed)
            {
                angularY = 0;
            }
            else
            {
                angularY = m_angularY;
            }
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
