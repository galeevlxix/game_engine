using game_2.Brain.Lights.LightStructures;
using game_2.MathFolder;
using OpenTK.Mathematics;

namespace game_2.Brain
{
    public class Pipeline
    {
        public vector3f RotateVector { get; private set; }
        public vector3f ScaleVector { get; private set; }
        public vector3f PositionVector { get; private set; }

        // Получить значения

        public float PosX
        {
            get
            {
                return PositionVector.x;
            }
        }

        public float PosY
        {
            get
            {
                return PositionVector.y;
            }
        }

        public float PosZ
        {
            get
            {
                return PositionVector.z;
            }
        }

        public float AngleX
        {
            get
            {
                return RotateVector.x;
            }
        }

        public float AngleY
        {
            get
            {
                return RotateVector.y;
            }
        }

        public float AngleZ
        {
            get
            {
                return RotateVector.z;
            }
        }

        public float ScaleX
        {
            get
            {
                return ScaleVector.x;
            }
        }

        public float ScaleY
        {
            get
            {
                return ScaleVector.y;
            }
        }

        public float ScaleZ
        {
            get
            {
                return ScaleVector.z;
            }
        }

        public Pipeline()
        {
            RotateVector = vector3f.Zero;
            ScaleVector = vector3f.One;
            PositionVector = vector3f.Zero;
        }

        // Установить значение

        public void SetAngle(float angleX, float angleY, float angleZ)
        {
            RotateVector.x = angleX;
            RotateVector.y = angleY;
            RotateVector.z = angleZ;
        }

        public void SetScale(float ScaleX, float ScaleY, float ScaleZ)
        {
            ScaleVector.x = ScaleX;
            ScaleVector.y = ScaleY;
            ScaleVector.z = ScaleZ;
        }

        public void SetScale(float Scale)
        {
            ScaleVector.x = Scale;
            ScaleVector.y = Scale;
            ScaleVector.z = Scale;
        }

        public void SetPosition(float PosX, float PosY, float PosZ)
        {
            PositionVector.x = PosX;
            PositionVector.y = PosY;
            PositionVector.z = PosZ;
        }

        public void SetPosition(vector3f position)
        {
            PositionVector.x = position.x;
            PositionVector.y = position.y;
            PositionVector.z = position.z;
        }

        public void SetPositionX(float PosX)
        {
            PositionVector.x = PosX;
        }

        public void SetPositionY(float PosY)
        {
            PositionVector.y = PosY;
        }

        public void SetPositionZ(float PosZ)
        {
            PositionVector.z = PosZ;
        }

        // Вращать

        public void Rotate(float speedX, float speedY, float speedZ, float time)
        {
            RotateVector.x += speedX * time;
            RotateVector.y += speedY * time;
            RotateVector.z += speedZ * time;
        }

        // Передвижение

        public void Move(float speedX, float speedY, float speedZ, float time)
        {
            PositionVector.x += speedX * time;
            PositionVector.y += speedY * time;
            PositionVector.z += speedZ * time;
        }

        public void MoveX(float speedX, float time)
        {
            PositionVector.x += speedX * time;
        }

        public void MoveY(float speedY, float time)
        {
            PositionVector.y += speedY * time;
        }

        public void MoveZ(float speedZ, float time)
        {
            PositionVector.z += speedZ * time;
        }

        // Увеличение

        public void Expand(float speedX, float speedY, float speedZ, float time)
        {
            ScaleVector.x += speedX * time;
            ScaleVector.y += speedY * time;
            ScaleVector.z += speedZ * time;
        }

        public void Expand(float speed, float time)
        {
            ScaleVector.x += speed * time;
            ScaleVector.y += speed * time;
            ScaleVector.z += speed * time;
        }

        public void Reset()
        {
            SetPosition(0, 0, 0);
            SetAngle(0, 0, 0);
            SetScale(1, 1, 1);
        }

        public Matrix4 getWorld()
        {
            matrix4f scaleTrans = new matrix4f();
            matrix4f rotateTrans = new matrix4f();
            matrix4f translationTrans = new matrix4f();

            scaleTrans.InitScaleTransform(ScaleVector.x, ScaleVector.y, ScaleVector.z);
            rotateTrans.Rotate(RotateVector.x, RotateVector.y, RotateVector.z);
            translationTrans.InitTranslationTransform(PositionVector.x, PositionVector.y, PositionVector.z);

            return (scaleTrans * rotateTrans * translationTrans).ToOpenTK();
        }

        public Matrix4 getWVP()
        {
            matrix4f scaleTrans = new matrix4f();
            matrix4f rotateTrans = new matrix4f();
            matrix4f translationTrans = new matrix4f();
            scaleTrans.InitScaleTransform(ScaleVector.x, ScaleVector.y, ScaleVector.z);
            rotateTrans.Rotate(RotateVector.x, RotateVector.y, RotateVector.z);
            translationTrans.InitTranslationTransform(PositionVector.x, PositionVector.y, PositionVector.z);

            matrix4f pers = mPersProj.PersProjMatrix;
            matrix4f c_pos = Camera.CameraTranslation;
            matrix4f c_rot = Camera.CameraRotation;

            return (scaleTrans * rotateTrans * translationTrans * c_pos * c_rot * pers).ToOpenTK();
        }
    }
}
