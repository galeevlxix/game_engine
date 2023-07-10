using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game_2.Brain
{
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
}
