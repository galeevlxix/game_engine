using game_2.MathFolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game_2.Brain.InfoPanelFolder
{
    public static class ScreenStaticPersProjMat
    {
        public static float FOV = 50;
        public static float zFar = 2f;
        public static float zNear = 1f;
        public static matrix4f PersProjMatrix = new matrix4f();
    }
}
