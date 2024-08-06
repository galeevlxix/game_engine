using game_2.Brain.InfoPanelFolder;
using game_2.Brain.Selecting;
using game_2.Brain.SkyBoxFolder;
using game_2.MathFolder;

namespace game_2.Brain
{
    public static class PersProjMat
    {
        private static float FOV = 70;
        private static float width = 1920;
        private static float height = 1080;
        private static float zNear = 0.1f;
        private static float zFar = 50;
        public static matrix4f PersProjMatrix = new matrix4f();

        public static float GetFOV
        {
            get
            {
                return FOV;
            }
        }

        public static float GetWidth
        {
            get
            {
                return width;
            }
        }

        public static float GetHeight
        {
            get
            {
                return height;
            }
        }

        public static float GetZNear
        {
            get 
            { 
                return zNear;
            }
        }

        public static float GetZFar
        {
            get
            {
                return zFar;
            }
        }

        public static void Init(float _FOV, float _width, float _height, float _zNear, float _zFar)
        {
            FOV = _FOV;
            width = _width;
            height = _height;
            zNear = _zNear;
            zFar = _zFar;
            PersProjMatrix.InitPersProjTransform(FOV, width, height, zNear, zFar);
            InitSymbolPersProjMatrix();
            InitSelectionPersProjMatrix();
            InitSkyBoxPersProjMat();
        }

        public static void Init()
        {
            PersProjMatrix.InitPersProjTransform(FOV, width, height, zNear, zFar);
            InitSymbolPersProjMatrix();
            InitSelectionPersProjMatrix();
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
            InitSymbolPersProjMatrix();
        }

        private static void InitSymbolPersProjMatrix() => 
            ScreenStaticPersProjMat.PersProjMatrix.InitPersProjTransform(
                ScreenStaticPersProjMat.FOV, 
                width, 
                height,
                ScreenStaticPersProjMat.zNear, 
                ScreenStaticPersProjMat.zFar);

        private static void InitSelectionPersProjMatrix() =>
            SelectingPersProjMat.PersProjMatrix.InitPersProjTransform(
                SelectingPersProjMat.FOV,
                SelectingPersProjMat.width,
                SelectingPersProjMat.height,
                zNear, zFar);

        private static void InitSkyBoxPersProjMat() =>
            SkyBoxPersProjMat.PersProjMatrix.InitPersProjTransform(
                FOV,
                width,
                height,
                zNear,
                SkyBoxPersProjMat.zFar);
    }
}
