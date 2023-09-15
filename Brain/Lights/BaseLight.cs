using game_2.MathFolder;

namespace game_2.Brain.Lights
{
    public struct BaseLight
    {
        public vector3f Color;
        public float AmbientIntensity;
        public float DiffuseIntensity;

        public BaseLight(vector3f Color, float AmbientIntensity, float DiffuseIntensity)
        {
            this.Color = Color;
            this.AmbientIntensity = AmbientIntensity;
            this.DiffuseIntensity = DiffuseIntensity;
        }

        public BaseLight()
        {
            Color = vector3f.Zero;
            AmbientIntensity = 0f;
            DiffuseIntensity = 0f;
        }
    }

    public struct BaseLightLocations
    {
        public int Color;
        public int AmbientIntensity;
        public int DiffuseIntensity;
    }
}
