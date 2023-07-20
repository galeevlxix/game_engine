using game_2.MathFolder;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game_2.Brain
{
    public static class mPersProj
    {
        private static float FOV = 50;
        private static float width = 1920;
        private static float height = 1080;
        private static float zNear = 0.001f;
        private static float zFar = 100;
        public static matrix4f PersProjMatrix = new matrix4f();

        public static void Init(float _FOV, float _width, float _height, float _zNear, float _zFar)
        {
            FOV = _FOV;
            width = _width;
            height = _height;
            zNear = _zNear;
            zFar = _zFar;
            PersProjMatrix.InitPersProjTransform(FOV, width, height, zNear, zFar);
        }

        public static void InitDefault()
        {
            PersProjMatrix.InitPersProjTransform(FOV, width, height, zNear, zFar);
        }

        public static void ChangeFOV(float _fov)
        {
            FOV = _fov;
            PersProjMatrix.InitPersProjTransform(FOV, width, height, zNear, zFar);
        }

        public static void ChangeWindowSize(float _width, float _height)
        {
            width = _width; height = _height;
            PersProjMatrix.InitPersProjTransform(FOV, width, height, zNear, zFar);
        }
    }
}
