using game_2.Brain.Shadows;
using game_2.MathFolder;
using OpenTK.Mathematics;

namespace game_2.Brain.Lights.LightStructures
{
    public class Spotlight
    {
        public vector3f Direction;
        public float Cutoff1;
        public float Cutoff2;
        public PointLight PointLight;

        public ShadowMapFBO ShadowMapSpotlight;
        public Dictionary<string, Matrix4> mvpMatrixFromLight;

        public Spotlight()
        {
            Direction = new vector3f();
            Cutoff1 = 1;
            Cutoff2 = 1;
            PointLight = new PointLight();

            ShadowMapSpotlight = new ShadowMapFBO();
            mvpMatrixFromLight = new Dictionary<string, Matrix4>();
        }
    }

    public struct SpotlightLocations
    {
        public PointLightLocations PointLightLocations;
        public int Direction;
        public int Cutoff1;
        public int Cutoff2;
    }
}
