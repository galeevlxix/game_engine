using game_2.MathFolder;

namespace game_2.Brain.Lights
{
    public struct DirectionalLight
    {
        public vector3f Direction;
        public BaseLight BaseLight;
        public DirectionalLight(vector3f Color, float AmbientIntensity, float DiffuseIntensity, vector3f Direction)
        {
            this.BaseLight = new BaseLight(Color, AmbientIntensity, DiffuseIntensity);
            this.Direction = Direction;
        }

        public DirectionalLight(BaseLight BaseLight, vector3f Direction)
        {
            this.BaseLight = BaseLight;
            this.Direction = Direction;
        }

        public DirectionalLight()
        {
            BaseLight = new BaseLight();
            Direction = vector3f.Zero;
        }
    }

    public struct DirectionalLightLocations
    {
        public BaseLightLocations BaseLightLocations;
        public int Direction;
    }
}
