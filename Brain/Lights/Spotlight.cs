using game_2.MathFolder;

namespace game_2.Brain.Lights
{
    public struct Spotlight
    {
        public vector3f Direction;
        public float Cutoff;
        public PointLight PointLight;
    }

    public struct SpotlightLocations
    {
        public PointLightLocations PointLightLocations;
        public int Direction;
        public int Cutoff;
    }
}
