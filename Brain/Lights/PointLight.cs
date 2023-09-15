using game_2.MathFolder;

namespace game_2.Brain.Lights
{
    public struct PointLight 
    {
        public vector3f Position;
        public BaseLight BaseLight;
        public Attenuation Attenuation;

        public PointLight(vector3f Color, float AmbientIntensity, float DiffuseIntensity, vector3f Position, float Constant, float Linear, float Exp)
        {
            this.BaseLight = new BaseLight(Color, AmbientIntensity, DiffuseIntensity);
            this.Position = Position;
            this.Attenuation = new Attenuation(Constant, Linear, Exp);
        }

        public PointLight (BaseLight BaseLight, vector3f Position, Attenuation Attenuation)
        {
            this.BaseLight = BaseLight;
            this.Position = Position;
            this.Attenuation = Attenuation;
        }
    }

    public struct Attenuation
    {
        public float Constant;
        public float Linear;
        public float Exp;

        public Attenuation(float Constant, float Linear, float Exp)
        {
            this.Constant = Constant;
            this.Linear = Linear;
            this.Exp = Exp;
        }
    }

    public struct AttenuationLocations
    {
        public int Constant;
        public int Linear;
        public int Exp;
    }

    public struct PointLightLocations
    {
        public BaseLightLocations BaseLightLocations;
        public AttenuationLocations Attenuation;
        public int Position;
    }
}
