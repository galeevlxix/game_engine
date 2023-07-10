using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game_2.MathFolder
{
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
