using game_2.MathFolder;

namespace game_2.Brain.Lights
{
    public struct BaseLight
    {
        public vector3f Color;
        public float AmbientIntensity;
        public float DiffuseIntensity;
    }

    public struct BaseLightLocations
    {
        public int Color;
        public int AmbientIntensity;
        public int DiffuseIntensity;
    }
}
