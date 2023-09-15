using game_2.Brain.ObjectFolder;
using game_2.MathFolder;
using OpenTK.Mathematics;

namespace game_2.Brain.MonochromeObjectFolder
{
    public class MonochromeObject : GameObj
    {
        private vector3f Color = new vector3f(1, 1, 1);
        private vector3f Light = new vector3f(1, 1, 1);
        public MonochromeObject(vector3f Color, vector3f Light)
        {
            mesh = new MonochromeObjectMesh();
            pipeline = new Pipeline();

            this.Color = Color;
            this.Light = Light;
        }

        public override void Draw()
        {
            CentralizedShaders.MonochromeShader.setVector3("color", Color);
            CentralizedShaders.MonochromeShader.setVector3("light", Light);
            base.Draw();
        }

        public void SetColor(vector3f Color)
        {
            this.Color = Color;
        }

        public void SetLight(vector3f Light)
        {
            this.Light = Light;
        }
    }
}
