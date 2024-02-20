using game_2.MathFolder;

namespace game_2.Brain.Lights.LightStructures
{
    public struct Spotlight
    {
        public vector3f Direction;
        public float Cutoff1;
        public float Cutoff2;
        public PointLight PointLight;
    }

    public struct SpotlightLocations
    {
        public PointLightLocations PointLightLocations;
        public int Direction;
        public int Cutoff1;
        public int Cutoff2;
    }
}
