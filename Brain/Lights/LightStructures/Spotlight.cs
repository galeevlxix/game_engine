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

        public ShadowPiece shadowPiece = new ShadowPiece();

        public Spotlight()
        {
            Direction = new vector3f();
            Cutoff1 = 1;
            Cutoff2 = 1;
            PointLight = new PointLight();

            shadowPiece.ShadowMap = new ShadowMapFBO();
            shadowPiece.wvpMatricesFromLight = new Dictionary<string, Matrix4>();
        }
    }

    public struct SpotlightLocations
    {
        public PointLightLocations PointLightLocations;
        public int Direction;
        public int Cutoff1;
        public int Cutoff2;
    }

    public struct ShadowPiece
    {
        public ShadowMapFBO ShadowMap;
        public Dictionary<string, Matrix4> wvpMatricesFromLight;
    }
}
