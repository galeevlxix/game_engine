using OpenTK.Mathematics;

namespace game_2.Brain
{
    public class vector2
    {
        public float x;
        public float y;

        public vector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public vector2()
        {
            x = 0;
            y = 0;
        }
    }

    public class vector3
    {
        public float x;
        public float y;
        public float z;

        public vector3()
        {
            x = 0;
            y = 0;
            z = 0;
        }

        public vector3(float _x, float _y, float _z)
        {
            x = _x;
            y = _y;
            z = _z;
        }

        public static vector3 operator +(vector3 l, vector3 r)
        {
            return new vector3(l.x + r.x, l.y + r.y, l.z + r.z);
        }

        public static vector3 operator -(vector3 l, vector3 r)
        {
            return new vector3(l.x - r.x, l.y - r.y, l.z - r.z);
        }

        public static vector3 operator -(vector3 r)
        {
            return new vector3(- r.x, - r.y, - r.z);
        }

        public static vector3 operator *(vector3 l, float f)
        {
            return new vector3(l.x * f, l.y * f, l.z * f);
        }

        public static vector3 operator /(float l, vector3 f)
        {
            return new vector3(f.x / l, f.y / l, f.z / l);
        }

        public static vector3 Transform(vector3 v, Matrix4 m)
        {
            float x = v.x * m[0, 0] + v.y * m[1, 0] + v.z * m[2, 0] + m[3, 0];
            float y = v.y * m[0, 1] + v.y * m[1, 1] + v.z * m[2, 1] + m[3, 1];
            float z = v.z * m[0, 2] + v.y * m[1, 2] + v.z * m[2, 2] + m[3, 2];
            return new vector3(x, y, z);
        }

        public static vector3 Zero()
        {
            return new vector3 { x = 0, y = 0, z = 0 };
        }
    }

    public class Pipeline
    {
        public vector3 RotateVector;
        public vector3 ScaleVector;
        public vector3 PositionVector;
        public mPersProj mPersProj;

        public Pipeline()
        {
            RotateVector = new vector3 { x = 0, y = 0, z = 0 };
            ScaleVector = new vector3 { x = 1, y = 1, z = 1 };
            PositionVector = new vector3 { x = 0, y = 0, z = 0 };
            mPersProj = new mPersProj();
        }

        public void Rotate(float angleX, float angleY, float angleZ)
        {
            RotateVector.x = angleX;
            RotateVector.y = angleY;
            RotateVector.z = angleZ;
        }

        public void Scale(float ScaleX, float ScaleY, float ScaleZ)
        {
            ScaleVector.x = ScaleX;
            ScaleVector.y = ScaleY;
            ScaleVector.z = ScaleZ;
        }

        public void Position(float PosX, float PosY, float PosZ)
        {
            PositionVector.x = PosX;
            PositionVector.y = PosY;
            PositionVector.z = PosZ;
        }

        public void PersProj(float FOV, float width, float height, float zNear, float zFar)
        {
            mPersProj.FOV = FOV;
            mPersProj.width = width;
            mPersProj.height = height;
            mPersProj.zNear = zNear;
            mPersProj.zFar = zFar;
        }

        public Matrix4 getMVP()
        {
            return  InitScaleTransform(ScaleVector.x, ScaleVector.y, ScaleVector.z) *
                    InitRotateTransform(RotateVector.x, RotateVector.y, RotateVector.z) *
                    InitTranslationTransform(PositionVector.x, PositionVector.y, PositionVector.z) *
                    InitPersProjTransform(mPersProj.FOV, mPersProj.width, mPersProj.height, mPersProj.zNear, mPersProj.zFar);
        }

        private static Matrix4 InitRotateTransform(float RotateX, float RotateY, float RotateZ)
        {
            Matrix4 rx = new Matrix4(), ry = new Matrix4(), rz = new Matrix4();
            float x = math3d.ToRadian(RotateX);
            float y = math3d.ToRadian(RotateY);
            float z = math3d.ToRadian(RotateZ);

            rx[0, 0] = 1.0f;              rx[0, 1] = 0.0f;              rx[0, 2] = 0.0f;              rx[0, 3] = 0.0f;
            rx[1, 0] = 0.0f;              rx[1, 1] = math3d.cos(x);     rx[1, 2] = -math3d.sin(x);    rx[1, 3] = 0.0f;
            rx[2, 0] = 0.0f;              rx[2, 1] = math3d.sin(x);     rx[2, 2] = math3d.cos(x);     rx[2, 3] = 0.0f;
            rx[3, 0] = 0.0f;              rx[3, 1] = 0.0f;              rx[3, 2] = 0.0f;              rx[3, 3] = 1.0f;

            ry[0, 0] = math3d.cos(y);     ry[0, 1] = 0.0f;              ry[0, 2] = -math3d.sin(y);    ry[0, 3] = 0.0f;
            ry[1, 0] = 0.0f;              ry[1, 1] = 1.0f;              ry[1, 2] = 0.0f;              ry[1, 3] = 0.0f;
            ry[2, 0] = math3d.sin(y);     ry[2, 1] = 0.0f;              ry[2, 2] = math3d.cos(y);     ry[2, 3] = 0.0f;
            ry[3, 0] = 0.0f;              ry[3, 1] = 0.0f;              ry[3, 2] = 0.0f;              ry[3, 3] = 1.0f;

            rz[0, 0] = math3d.cos(z);     rz[0, 1] = -math3d.sin(z);    rz[0, 2] = 0.0f;              rz[0, 3] = 0.0f;
            rz[1, 0] = math3d.sin(z);     rz[1, 1] = math3d.cos(z);     rz[1, 2] = 0.0f;              rz[1, 3] = 0.0f;
            rz[2, 0] = 0.0f;              rz[2, 1] = 0.0f;              rz[2, 2] = 1.0f;              rz[2, 3] = 0.0f;
            rz[3, 0] = 0.0f;              rz[3, 1] = 0.0f;              rz[3, 2] = 0.0f;              rz[3, 3] = 1.0f;

            return rz * ry * rx;
        }

        private static Matrix4 InitTranslationTransform(float x, float y, float z)
        {
            Matrix4 m = new Matrix4();
            m[0, 0] = 1.0f; m[0, 1] = 0.0f; m[0, 2] = 0.0f; m[0, 3] = 0.0f;
            m[1, 0] = 0.0f; m[1, 1] = 1.0f; m[1, 2] = 0.0f; m[1, 3] = 0.0f;
            m[2, 0] = 0.0f; m[2, 1] = 0.0f; m[2, 2] = 1.0f; m[2, 3] = 0.0f;
            m[3, 0] = x;    m[3, 1] = y;    m[3, 2] = z;    m[3, 3] = 1.0f;
            return m;
        }

        private static Matrix4 InitScaleTransform(float x, float y, float z)
        {
            Matrix4 m = new Matrix4();
            m[0, 0] = x;    m[0, 1] = 0.0f; m[0, 2] = 0.0f; m[0, 3] = 0.0f;
            m[1, 0] = 0.0f; m[1, 1] = y;    m[1, 2] = 0.0f; m[1, 3] = 0.0f;
            m[2, 0] = 0.0f; m[2, 1] = 0.0f; m[2, 2] = z;    m[2, 3] = 0.0f;
            m[3, 0] = 0.0f; m[3, 1] = 0.0f; m[3, 2] = 0.0f; m[3, 3] = 1.0f;
            return m;
        }

        private static Matrix4 InitPersProjTransform(float FOV, float Width, float Height, float zNear, float zFar)
        {
            Matrix4 m = new Matrix4();
            float ar = Width / Height;
            float zRange = zNear - zFar;
            float tanHalfFOV = math3d.tan(math3d.ToRadian(FOV) * 0.5f);

            m[0, 0] = 1.0f / (tanHalfFOV * ar);     m[0, 1] = 0.0f;                 m[0, 2] = 0.0f;                         m[0, 3] = 0;
            m[1, 0] = 0.0f;                         m[1, 1] = 1.0f / tanHalfFOV;    m[1, 2] = 0.0f;                         m[1, 3] = 0;
            m[2, 0] = 0.0f;                         m[2, 1] = 0.0f;                 m[2, 2] = (zNear + zFar) / zRange;      m[2, 3] = -1.0f;
            m[3, 0] = 0.0f;                         m[3, 1] = 0.0f;                 m[3, 2] = 2.0f * zFar * zNear / zRange; m[3, 3] = 0;
            return m;
        }
    }

    public class mPersProj
    {
        public float FOV;
        public float width;
        public float height;
        public float zNear;
        public float zFar;

        public mPersProj(float FOV, float width, float heigth, float zNear, float zFar)
        {
            this.FOV = FOV;
            this.width = width;
            this.height = heigth;
            this.zNear = zNear; 
            this.zFar = zFar;
        }

        public mPersProj()
        {
            FOV = 50;
            width = 1920;
            height = 1080;
            zNear = 0.001f;
            zFar = 100;
        }
    }

    public static class math3d
    {
        public static float ToRadian(float x)
        {
            return x * (float)Math.PI / 180.0f;
        }
        public static float ToDegree(float x)
        {
            return x / (float)Math.PI * 180.0f;
        }
        public static float sin(float x)
        {
            return (float)Math.Sin((float)x);
        }
        public static float cos(float x)
        {
            return (float)Math.Cos((float)x);
        }
        public static float tan(float x)
        {
            return (float)Math.Tan((float)x);
        }
        public static float asin(float x)
        {
            return (float)Math.Asin((float)x);
        }
        public static float acos(float x)
        {
            return (float)Math.Acos((float)x);
        }
        public static float atan(float x)
        {
            return (float)Math.Atan((float)x);
        }
        public static float abs(float x)
        {
            return (float)Math.Abs((float)x);
        }
        public static float sqrt(float x)
        {
            return (float)Math.Sqrt((float)x);
        }
        public static float pow(float x, float y)
        {
            return (float)Math.Pow((float)x, (float)y);
        }
    }
}
