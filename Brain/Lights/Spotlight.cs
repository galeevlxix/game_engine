using game_2.MathFolder;

namespace game_2.Brain.Lights
{
    public struct Spotlight
    {
        public vector3f Direction;
        public float Cuttoff;
        public PointLight PointLight;
        public Spotlight(vector3f Color, float AmbientIntensity, float DiffuseIntensity, vector3f Position, vector3f Direction, float Constant, float Linear, float Exp, float Cuttoff)
        {
            this.PointLight = new PointLight(Color, AmbientIntensity, DiffuseIntensity, Position, Constant, Linear, Exp);
            this.Direction = Direction;
            this.Cuttoff = Cuttoff;
        }

        public Spotlight(PointLight PointLight, vector3f Direction, float Cuttoff)
        {
            this.PointLight = PointLight;
            this.Direction = Direction;
            this.Cuttoff = Cuttoff;
        }   
    }

    public struct SpotlightLocations
    {
        public PointLightLocations PointLightLocations;
        public int Direction;
        public int Cuttoff;
    }
}
