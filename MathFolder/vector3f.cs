using OpenTK.Mathematics;

namespace game_2.MathFolder
{
    public class vector3f
    {
        public float x;
        public float y;
        public float z;

        public vector3f()
        {
            x = 0;
            y = 0;
            z = 0;
        }

        public vector3f(float _x, float _y, float _z)
        {
            x = _x;
            y = _y;
            z = _z;
        }

        public vector3f(vector3f v)
        {
            x = v.x;
            y = v.y;
            z = v.z;
        }

        public static vector3f operator +(vector3f l, vector3f r)
        {
            return new vector3f(l.x + r.x, l.y + r.y, l.z + r.z);
        }

        public static vector3f operator -(vector3f l, vector3f r)
        {
            return new vector3f(l.x - r.x, l.y - r.y, l.z - r.z);
        }

        public static vector3f operator -(vector3f r)
        {
            return new vector3f(-r.x, -r.y, -r.z);
        }

        public static vector3f operator *(vector3f l, float f)
        {
            return new vector3f(l.x * f, l.y * f, l.z * f);
        }

        public static vector3f operator /(float l, vector3f f)
        {
            return new vector3f(f.x / l, f.y / l, f.z / l);
        }

        public static vector3f Transform(vector3f v, Matrix4 m)
        {
            float x = v.x * m[0, 0] + v.y * m[1, 0] + v.z * m[2, 0] + m[3, 0];
            float y = v.y * m[0, 1] + v.y * m[1, 1] + v.z * m[2, 1] + m[3, 1];
            float z = v.z * m[0, 2] + v.y * m[1, 2] + v.z * m[2, 2] + m[3, 2];
            return new vector3f(x, y, z);
        }

        public static vector3f Cross(vector3f b, vector3f v)
        {
            float _x = b.y * v.z - b.z * v.y;
            float _y = b.z * v.x - b.x * v.z;
            float _z = b.x * v.y - b.y * v.x;
            return new vector3f(_x, _y, _z);
        }

        public static vector3f Normalize(vector3f v)
        {
            float Length = (float)Math.Sqrt(v.x * v.x + v.y * v.y + v.z * v.z);

            v.x /= Length;
            v.y /= Length;
            v.z /= Length;

            return v;
        }

        public void Normalize()
        {
            float Length = (float)Math.Sqrt(x * x + y * y + z * z);

            x /= Length;
            y /= Length;
            z /= Length;
        }

        public void Rotate(float Angle, vector3f Axe)
        {
            float SinHalfAngle = (float)Math.Sin(math3d.ToRadian(Angle / 2));
            float CosHalfAngle = math3d.cos(math3d.ToRadian(Angle / 2));

            float Rx = Axe.x * SinHalfAngle;
            float Ry = Axe.y * SinHalfAngle;
            float Rz = Axe.z * SinHalfAngle;
            float Rw = CosHalfAngle;
            Quaternion RotationQ = new Quaternion(Rx, Ry, Rz, Rw);
            Quaternion ConjugateQ = RotationQ.Conjugate();
            Quaternion W = RotationQ * (this) * ConjugateQ;

            x = W.x;
            y = W.y;
            z = W.z;
        }

        public static vector3f Zero
        {
            get
            {
                return new vector3f { x = 0, y = 0, z = 0 };
            }
        }

        public static vector3f One
        {
            get
            {
                return new vector3f { x = 1, y = 1, z = 1 };
            }
        }

        public static vector3f Right
        {
            get
            {
                return new vector3f { x = 1, y = 0, z = 0 };
            }
        }

        public static vector3f Up
        {
            get
            {
                return new vector3f { x = 0, y = 1, z = 0 };
            }
        }

        public static vector3f Ford
        {
            get
            {
                return new vector3f { x = 0, y = 0, z = 1 };
            }
        }

        public string ToStr()
        {
            return "x: " + x + "; y: " + y + "; z: " + z;
        }
    }
}
