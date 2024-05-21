using game_2.MathFolder;
using OpenTK.Graphics.OpenGL;

namespace game_2.Brain.Shadows
{
    public class CubeMapTarget
    {
        public readonly TextureTarget CubemapFace;
        public readonly vector3f Target;
        public readonly vector3f Up;

        private CubeMapTarget(TextureTarget CubemapFace, vector3f target, vector3f up)
        {
            this.CubemapFace = CubemapFace;
            this.Target = target;
            this.Up = up;
        }

        public static readonly CubeMapTarget[] cubeMapTargets = new CubeMapTarget[]
        {
            new CubeMapTarget(TextureTarget.TextureCubeMapPositiveX, new vector3f(1, 0, 0), new vector3f(0, 1, 0)),
            new CubeMapTarget(TextureTarget.TextureCubeMapNegativeX, new vector3f(-1, 0, 0), new vector3f(0, 1, 0)),
            new CubeMapTarget(TextureTarget.TextureCubeMapPositiveY, new vector3f(0, 1, 0), new vector3f(0, 0, -1)),
            new CubeMapTarget(TextureTarget.TextureCubeMapNegativeY, new vector3f(0, -1, 0), new vector3f(0, 0, 1)),
            new CubeMapTarget(TextureTarget.TextureCubeMapPositiveZ, new vector3f(0, 0, 1), new vector3f(0, 1, 0)),
            new CubeMapTarget(TextureTarget.TextureCubeMapNegativeZ, new vector3f(0, 0, -1), new vector3f(0, 1, 0)),
        };
    }
}