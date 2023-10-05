using game_2.MathFolder;

namespace game_2.Brain.Lights
{
    public struct DirectionalLight
    {
        public vector3f Direction;
        public BaseLight BaseLight;
    }

    public struct DirectionalLightLocations
    {
        public BaseLightLocations BaseLight;
        public int Direction;
    }
}
