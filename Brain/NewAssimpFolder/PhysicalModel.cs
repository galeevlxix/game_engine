using Assimp;
using Assimp.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game_2.Brain.NewAssimpFolder
{
    public class PhysicalModel
    {
        public Pipeline _pipeline;

        public float speedX;
        public float speedY;
        public float speedZ;

        private const float G = 9.834f;

        public PhysicalModel()
        {
            speedX = 0;
            speedY = 0;
            speedZ = 0;

            _pipeline = new Pipeline();
        }

        public void Acceleration(float accelX, float accelY, float accelZ, float deltaTime)
        {
            speedX += accelX * deltaTime;
            speedY += accelY * deltaTime;
            speedZ += accelZ * deltaTime;

            speedY -= G * deltaTime;
        }

        
    }
}
