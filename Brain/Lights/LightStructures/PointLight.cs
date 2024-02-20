using game_2.MathFolder;

namespace game_2.Brain.Lights.LightStructures
{
    public struct PointLight
    {
        public vector3f Position;
        public BaseLight BaseLight;
        public Attenuation Attenuation;
    }

    public struct PointLightLocations
    {
        public BaseLightLocations BaseLightLocations;
        public AttenuationLocations Attenuation;
        public int Position;
    }

    public struct Attenuation
    {
        public float Constant;
        public float Linear;
        public float Exp;
    }

    public struct AttenuationLocations
    {
        public int Constant;
        public int Linear;
        public int Exp;
    }
}
