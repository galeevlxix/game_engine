using game_2.Brain;
using OpenTK.Mathematics;

namespace game_2.MathFolder
{
    public class matrix4f
    {
        private float[,] m;

        public matrix4f() 
        {
            m = new float[4, 4];
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

        public static matrix4f operator *(matrix4f Left, matrix4f Right)
        {
            matrix4f Ret = new matrix4f();

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

        public void RotateX(float RotX)
        {
            float x = math3d.ToRadian(RotX);
            var sinx = math3d.sin(x);
            var cosx = math3d.cos(x);

            m[0, 0] = 1.0f; m[0, 1] = 0.0f; m[0, 2] = 0.0f; m[0, 3] = 0.0f;
            m[1, 0] = 0.0f; m[1, 1] = cosx; m[1, 2] = -sinx;m[1, 3] = 0.0f;
            m[2, 0] = 0.0f; m[2, 1] = sinx; m[2, 2] = cosx; m[2, 3] = 0.0f;
            m[3, 0] = 0.0f; m[3, 1] = 0.0f; m[3, 2] = 0.0f; m[3, 3] = 1.0f;
        }

        public void RotateY(float RotY)
        {
            float y = math3d.ToRadian(RotY);
            var siny = math3d.sin(y);
            var cosy = math3d.cos(y);

            m[0, 0] = cosy; m[0, 1] = 0.0f; m[0, 2] = -siny; m[0, 3] = 0.0f;
            m[1, 0] = 0.0f; m[1, 1] = 1.0f; m[1, 2] = 0.0f; m[1, 3] = 0.0f;
            m[2, 0] = siny; m[2, 1] = 0.0f; m[2, 2] = cosy; m[2, 3] = 0.0f;
            m[3, 0] = 0.0f; m[3, 1] = 0.0f; m[3, 2] = 0.0f; m[3, 3] = 1.0f;
        }

        public void RotateZ(float RotZ)
        {
            float z = math3d.ToRadian(RotZ);
            var sinz = math3d.sin(z);
            var cosz = math3d.cos(z);

            m[0, 0] = cosz; m[0, 1] = -sinz; m[0, 2] = 0.0f; m[0, 3] = 0.0f;
            m[1, 0] = sinz; m[1, 1] = cosz; m[1, 2] = 0.0f; m[1, 3] = 0.0f;
            m[2, 0] = 0.0f; m[2, 1] = 0.0f; m[2, 2] = 1.0f; m[2, 3] = 0.0f;
            m[3, 0] = 0.0f; m[3, 1] = 0.0f; m[3, 2] = 0.0f; m[3, 3] = 1.0f;
        }

        public void Rotate(float x, float y, float z)
        {
            matrix4f m_x, m_y, m_z;
            m_x = new matrix4f();
            m_y = new matrix4f();
            m_z = new matrix4f();

            m_x.RotateX(x);
            m_y.RotateY(y);
            m_z.RotateZ(z);

            m = (m_z * m_y * m_x).m;
        }

        public void InitTranslationTransform(float x, float y, float z)
        {
            m[0, 0] = 1.0f; m[0, 1] = 0.0f; m[0, 2] = 0.0f; m[0, 3] = x;
            m[1, 0] = 0.0f; m[1, 1] = 1.0f; m[1, 2] = 0.0f; m[1, 3] = y;
            m[2, 0] = 0.0f; m[2, 1] = 0.0f; m[2, 2] = 1.0f; m[2, 3] = z;
            m[3, 0] = 0.0f;    m[3, 1] = 0.0f;    m[3, 2] = 0.0f;    m[3, 3] = 1.0f;
            this.Trans();
        }

        public void InitScaleTransform(float x, float y, float z)
        {
            m[0, 0] = x; m[0, 1] = 0.0f; m[0, 2] = 0.0f; m[0, 3] = 0.0f;
            m[1, 0] = 0.0f; m[1, 1] = y; m[1, 2] = 0.0f; m[1, 3] = 0.0f;
            m[2, 0] = 0.0f; m[2, 1] = 0.0f; m[2, 2] = z; m[2, 3] = 0.0f;
            m[3, 0] = 0.0f; m[3, 1] = 0.0f; m[3, 2] = 0.0f; m[3, 3] = 1.0f;
        }

        public void InitCameraTransform(vector3f Target, vector3f Up)
        {
            vector3f N = Target;
            N = vector3f.Normalize(N);
            vector3f U = Up;
            U = vector3f.Normalize(U);
            U = vector3f.Cross(U, N);
            vector3f V = vector3f.Cross(N, U);

            m[0, 0] = U.x; m[0, 1] = U.y; m[0, 2] = U.z; m[0, 3] = 0.0f;
            m[1, 0] = V.x; m[1, 1] = V.y; m[1, 2] = V.z; m[1, 3] = 0.0f;
            m[2, 0] = N.x; m[2, 1] = N.y; m[2, 2] = N.z; m[2, 3] = 0.0f;
            m[3, 0] = 0.0f; m[3, 1] = 0.0f; m[3, 2] = 0.0f; m[3, 3] = 1.0f;

            this.Trans();
        }

        public void InitPersProjTransform(float FOV, float Width, float Height, float zNear, float zFar)
        {
            float ar = Width / Height;
            float zRange = zNear - zFar;
            float tanHalfFOV = math3d.tan(math3d.ToRadian(FOV) * 0.5f);

            m[0, 0] = 1.0f / (tanHalfFOV * ar); m[0, 1] = 0.0f; m[0, 2] = 0.0f; m[0, 3] = 0;
            m[1, 0] = 0.0f; m[1, 1] = 1.0f / tanHalfFOV; m[1, 2] = 0.0f; m[1, 3] = 0;
            m[2, 0] = 0.0f; m[2, 1] = 0.0f; m[2, 2] = (zNear + zFar) / zRange; m[2, 3] = 2.0f * zFar * zNear / zRange;
            m[3, 0] = 0.0f; m[3, 1] = 0.0f; m[3, 2] = -1.0f; m[3, 3] = 0;
            this.Trans();
        }

        public void InitPersProjTransform(mPersProj mPersProj)
        {
            float ar = mPersProj.width / mPersProj.height;
            float zRange = mPersProj.zNear - mPersProj.zFar;
            float tanHalfFOV = math3d.tan(math3d.ToRadian(mPersProj.FOV) * 0.5f);

            m[0, 0] = 1.0f / (tanHalfFOV * ar); m[0, 1] = 0.0f; m[0, 2] = 0.0f; m[0, 3] = 0;
            m[1, 0] = 0.0f; m[1, 1] = 1.0f / tanHalfFOV; m[1, 2] = 0.0f; m[1, 3] = 0;
            m[2, 0] = 0.0f; m[2, 1] = 0.0f; m[2, 2] = (mPersProj.zNear + mPersProj.zFar) / zRange; m[2, 3] = 2.0f * mPersProj.zFar * mPersProj.zNear / zRange;
            m[3, 0] = 0.0f; m[3, 1] = 0.0f; m[3, 2] = -1.0f; m[3, 3] = 0;
            this.Trans();
        }

        public void Trans()
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++) 
                {
                    if (j > i)
                    {
                        var temp = m[i, j];
                        m[i, j] = m[j , i]; 
                        m[j, i] = temp;
                    }
                }
            }
        }
        
        public static Matrix4 ToFloatArray(matrix4f m)
        {
            float[] f = new float[16];

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    f[i * 4 + j] = m[i, j];
                }
            }

            return new Matrix4(
                f[0], 
                f[1],
                f[2],
                f[3],
                f[4],
                f[5],
                f[6],
                f[7],
                f[8],
                f[9],
                f[10],
                f[11],
                f[12],
                f[13],
                f[14],
                f[15]
                );
        }
    }
}
