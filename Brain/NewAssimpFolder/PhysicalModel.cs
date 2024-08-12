using game_2.MathFolder;

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

            //отскок по Y
            if (_pipeline.PosY - speedY * deltaTime <= -4.51f && speedY < 0)
            {
                if (math3d.abs(speedY * 0.8f) < 1f)    
                // если скорость слишком мала, отскока не будет, объект останавливается
                {
                    speedY = 0;
                }
                else 
                //объект теряет скорость при отскоке
                {
                    speedY = -speedY * 0.8f;
                } 
            }

        }

        
    }
}
