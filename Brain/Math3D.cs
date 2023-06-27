using OpenTK.Mathematics;

namespace game_2.Brain
{
    public class vector2
    {
        public int x;
        public int y;

        public vector2(int x, int y)
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

        public vector3 Cross(vector3 v)
        {
            float _x = y * v.z - z * v.y;
            float _y = z * v.x - x * v.z;
            float _z = x * v.y - y * v.x;
            return new vector3(_x, _y, _z);
        }

        public vector3 Normalize()
        {
            float Length = (float)Math.Sqrt(x * x + y * y + z * z);

            x /= Length;
            y /= Length;
            z /= Length;

            return this;
        }

        public void Rotate(float Angle, vector3 Axe)
        {
            float SinHalfAngle = (float)Math.Sin(math3d.ToRadian(Angle / 2));
            float CosHalfAngle = math3d.cos(math3d.ToRadian(Angle / 2));

            float Rx = Axe.x * SinHalfAngle;
            float Ry = Axe.y * SinHalfAngle;
            float Rz = Axe.z * SinHalfAngle;
            float Rw = CosHalfAngle;
            quaternion RotationQ = new quaternion(Rx, Ry, Rz, Rw);
            quaternion ConjugateQ = RotationQ.Conjugate();
            quaternion W = RotationQ * (this) * ConjugateQ;

            x = W.x;
            y = W.y;
            z = W.z;
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

        public static vector3 Transform(vector3 v, matrix4 m)
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

    public class matrix4
    {
        public float[,] m;

        public matrix4()
        {
            m = new float[4, 4];
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    m[i, j] = 0.0f;
        }

        public float this[int i, int j]
        {
            get
            {
                return m[i, j];
            }
            set
            {
                m[i, j] = value;
            }
        }

        public void InitIdentity()
        {
            m[0, 0] = 1.0f; m[0, 1] = 0.0f; m[0, 2] = 0.0f; m[0, 3] = 0.0f;
            m[1, 0] = 0.0f; m[1, 1] = 1.0f; m[1, 2] = 0.0f; m[1, 3] = 0.0f;
            m[2, 0] = 0.0f; m[2, 1] = 0.0f; m[2, 2] = 1.0f; m[2, 3] = 0.0f;
            m[3, 0] = 0.0f; m[3, 1] = 0.0f; m[3, 2] = 0.0f; m[3, 3] = 1.0f;
        }

        public static matrix4 operator *(matrix4 Left, matrix4 Right)
        {
            matrix4 Ret = new matrix4();

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    Ret.m[i, j] =
                        Left.m[i, 0] * Right.m[0, j] +
                        Left.m[i, 1] * Right.m[1, j] +
                        Left.m[i, 2] * Right.m[2, j] +
                        Left.m[i, 3] * Right.m[3, j];
                }
            }
            return Ret;
        }
    }

    public class Pipeline
    {
        public vector3 RotateVector;
        public vector3 ScaleVector;
        public vector3 PositionVector;
        public mPersProj mPersProj;
        public mCamera camera;

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

        public void setCamera(vector3 Pos, vector3 Target, vector3 Up)
        {
            camera.Pos = Pos;
            camera.Target = Target;
            camera.Up = Up;
        }

        private Matrix4 getTransformation()
        {
            return  InitScaleTransform(ScaleVector.x, ScaleVector.y, ScaleVector.z) *
                    InitRotateTransform(RotateVector.x, RotateVector.y, RotateVector.z) *
                    InitTranslationTransform(PositionVector.x, PositionVector.y, PositionVector.z);
        }

        public Matrix4 getMVP()
        {
            var sc = InitScaleTransform(ScaleVector.x, ScaleVector.y, ScaleVector.z);
            var rot = InitRotateTransform(RotateVector.x, RotateVector.y, RotateVector.z);
            var trans = InitTranslationTransform(PositionVector.x, PositionVector.y, PositionVector.z);
            var pers = InitPersProjTransform(mPersProj.FOV, mPersProj.width, mPersProj.height, mPersProj.zNear, mPersProj.zFar);
          
            return sc * rot * trans * pers;                    
        }

        public Matrix4 getMVP_with_camera()
        {
            Matrix4 CameraTranslationTrans, CameraRotateTrans, PersProjTrans;

            CameraTranslationTrans = InitTranslationTransform(-camera.Pos.x, -camera.Pos.y, -camera.Pos.z);
            CameraRotateTrans = InitCameraTransform(camera.Target, camera.Up);
            PersProjTrans = InitPersProjTransform(mPersProj.FOV, mPersProj.width, mPersProj.height, mPersProj.zNear, mPersProj.zFar);

            return PersProjTrans * CameraRotateTrans * CameraTranslationTrans * getTransformation();
        }

        public static Matrix4 RotateX(float a)
        {
            Matrix4 rx = new Matrix4();
            float x = math3d.ToRadian(a);

            rx[0, 0] = 1.0f; rx[0, 1] = 0.0f; rx[0, 2] = 0.0f; rx[0, 3] = 0.0f;
            rx[1, 0] = 0.0f; rx[1, 1] = math3d.cos(x); rx[1, 2] = -math3d.sin(x); rx[1, 3] = 0.0f;
            rx[2, 0] = 0.0f; rx[2, 1] = math3d.sin(x); rx[2, 2] = math3d.cos(x); rx[2, 3] = 0.0f;
            rx[3, 0] = 0.0f; rx[3, 1] = 0.0f; rx[3, 2] = 0.0f; rx[3, 3] = 1.0f;

            return rx;
        }

        public static Matrix4 RotateY(float a)
        {
            Matrix4 ry = new Matrix4();
            float y = math3d.ToRadian(a);

            ry[0, 0] = math3d.cos(y); ry[0, 1] = 0.0f; ry[0, 2] = -math3d.sin(y); ry[0, 3] = 0.0f;
            ry[1, 0] = 0.0f; ry[1, 1] = 1.0f; ry[1, 2] = 0.0f; ry[1, 3] = 0.0f;
            ry[2, 0] = math3d.sin(y); ry[2, 1] = 0.0f; ry[2, 2] = math3d.cos(y); ry[2, 3] = 0.0f;
            ry[3, 0] = 0.0f; ry[3, 1] = 0.0f; ry[3, 2] = 0.0f; ry[3, 3] = 1.0f;

            return ry;
        }

        public static Matrix4 RotateZ(float a)
        {
            Matrix4 rz = new Matrix4();
            float z = math3d.ToRadian(a);

            rz[0, 0] = math3d.cos(z); rz[0, 1] = -math3d.sin(z); rz[0, 2] = 0.0f; rz[0, 3] = 0.0f;
            rz[1, 0] = math3d.sin(z); rz[1, 1] = math3d.cos(z); rz[1, 2] = 0.0f; rz[1, 3] = 0.0f;
            rz[2, 0] = 0.0f; rz[2, 1] = 0.0f; rz[2, 2] = 1.0f; rz[2, 3] = 0.0f;
            rz[3, 0] = 0.0f; rz[3, 1] = 0.0f; rz[3, 2] = 0.0f; rz[3, 3] = 1.0f;

            return rz;
        }

        public static Matrix4 InitRotateTransform(float RotateX, float RotateY, float RotateZ)
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

        public static Matrix4 InitTranslationTransform(float x, float y, float z)
        {
            Matrix4 m = new Matrix4();
            m[0, 0] = 1.0f; m[0, 1] = 0.0f; m[0, 2] = 0.0f; m[0, 3] = 0.0f;
            m[1, 0] = 0.0f; m[1, 1] = 1.0f; m[1, 2] = 0.0f; m[1, 3] = 0.0f;
            m[2, 0] = 0.0f; m[2, 1] = 0.0f; m[2, 2] = 1.0f; m[2, 3] = 0.0f;
            m[3, 0] = x; m[3, 1] = y; m[3, 2] = z; m[3, 3] = 1.0f;
            return m;
        }

        public static Matrix4 InitScaleTransform(float x, float y, float z)
        {
            Matrix4 m = new Matrix4();
            m[0, 0] = x;    m[0, 1] = 0.0f; m[0, 2] = 0.0f; m[0, 3] = 0.0f;
            m[1, 0] = 0.0f; m[1, 1] = y;    m[1, 2] = 0.0f; m[1, 3] = 0.0f;
            m[2, 0] = 0.0f; m[2, 1] = 0.0f; m[2, 2] = z;    m[2, 3] = 0.0f;
            m[3, 0] = 0.0f; m[3, 1] = 0.0f; m[3, 2] = 0.0f; m[3, 3] = 1.0f;
            return m;
        }

        public static Matrix4 InitPersProjTransform(float FOV, float Width, float Height, float zNear, float zFar)
        {
            Matrix4 m = new Matrix4();
            float ar = Width / Height;
            float zRange = zNear - zFar;
            float tanHalfFOV = math3d.tan(math3d.ToRadian(FOV) * 0.5f);

            m[0, 0] = 1.0f / (tanHalfFOV * ar);     m[0, 1] = 0.0f;                 m[0, 2] = 0.0f;                         m[0, 3] = 0;
            m[1, 0] = 0.0f;                         m[1, 1] = 1.0f / tanHalfFOV;    m[1, 2] = 0.0f;                         m[1, 3] = 0;
            m[2, 0] = 0.0f;                         m[2, 1] = 0.0f;                 m[2, 2] = -(-zNear - zFar) / zRange;     m[2, 3] = -1.0f;
            m[3, 0] = 0.0f;                         m[3, 1] = 0.0f;                 m[3, 2] = 2.0f * zFar * zNear / zRange; m[3, 3] = 0;
            return m;
        }

        private static Matrix4 InitCameraTransform(vector3 Target, vector3 Up)
        {
            Matrix4 m = new Matrix4();
            vector3 N = Target;
            N.Normalize();
            vector3 U = Up;
            U.Normalize();
            U = U.Cross(N);
            vector3 V = N.Cross(U);

            m[0, 0] = U.x; m[0, 1] = U.y; m[0, 2] = U.z; m[0, 3] = 0.0f;
            m[1, 0] = V.x; m[1, 1] = V.y; m[1, 2] = V.z; m[1, 3] = 0.0f;
            m[2, 0] = N.x; m[2, 1] = N.y; m[2, 2] = N.z; m[2, 3] = 0.0f;
            m[3, 0] = 0.0f; m[3, 1] = 0.0f; m[3, 2] = 0.0f; m[3, 3] = 1.0f;
            return m;
        }
    }

    public struct mCamera
    {
        public vector3 Pos;
        public vector3 Target;
        public vector3 Up;
    }

    public struct mPersProj
    {
        public float FOV;
        public float width;
        public float height;
        public float zNear;
        public float zFar;
    }

    public class math3d
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
        public static vector3 MultiplyDirection(Matrix4 m, vector3 v)
        {
            return new vector3(
                m[0, 0] * v.x + m[0, 1] * v.y + m[0, 2] * v.z,
                m[1, 0] * v.x + m[1, 1] * v.y + m[1, 2] * v.z,
                m[2, 0] * v.x + m[2, 1] * v.y + m[2, 2] * v.z);
        }
    }

    public class quaternion //вращение на определенный угол вокруг произвольной оси
    {
        public float x, y, z, w;

        public quaternion(float _x, float _y, float _z, float _w)
        {
            x = _x;
            y = _y;
            z = _z;
            w = _w;
        }

        public void Normalize()
        {
            float Length = math3d.sqrt(x * x + y * y + z * z + w * w);

            x /= Length;
            y /= Length;
            z /= Length;
            w /= Length;
        }

        public quaternion Conjugate()
        {
            return new quaternion(-x, -y, -z, w);
        }

        public static quaternion operator *(quaternion l, quaternion r)
        {
            float w = (l.w * r.w) - (l.x * r.x) - (l.y * r.y) - (l.z * r.z);
            float x = (l.x * r.w) + (l.w * r.x) + (l.y * r.z) - (l.z * r.y);
            float y = (l.y * r.w) + (l.w * r.y) + (l.z * r.x) - (l.x * r.z);
            float z = (l.z * r.w) + (l.w * r.z) + (l.x * r.y) - (l.y * r.x);

            return new quaternion(x, y, z, w);
        }

        public static quaternion operator *(quaternion q, vector3 v)
        {
            float w = -(q.x * v.x) - (q.y * v.y) - (q.z * v.z);
            float x = (q.w * v.x) + (q.y * v.z) - (q.z * v.y);
            float y = (q.w * v.y) + (q.z * v.x) - (q.x * v.z);
            float z = (q.w * v.z) + (q.x * v.y) - (q.y * v.x);

            return new quaternion(x, y, z, w);
        }
    }    
}
