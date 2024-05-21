using game_2.Brain.MonochromeObjectFolder;
using game_2.Brain.Shadows;
using game_2.MathFolder;
using OpenTK.Mathematics;

namespace game_2.Brain.Lights.LightStructures
{
    public class PointLight
    {
        private vector3f Pos;
        private BaseLight BaseLight;

        public Attenuation Attenuation;
        public MonochromeObject Lamp;

        public ShadowCubeMapFBO shadowCubeMap;

        public PointLight()
        {
            Pos = new vector3f();
            BaseLight.Color = new vector3f(0f, 0f, 0f);
            BaseLight.Intensity = 0f;
            Attenuation = new Attenuation();
            Lamp = new MonochromeObject(new vector3f(1, 1, 1), new vector3f(1, 1, 1));

            shadowCubeMap = new ShadowCubeMapFBO();
        }

        public void SetColor(vector3f color)
        {
            BaseLight.Color = color;
            Lamp.SetColor(color);
        }

        public void SetColor(float R, float G, float B)
        {
            BaseLight.Color = new vector3f(R, G, B);
            Lamp.SetColor(BaseLight.Color);
        }

        public void SetIntensity(float intensity)
        {
            BaseLight.Intensity = intensity;
            intensity = intensity > 1 ? 1 : intensity;
            vector3f intens = new vector3f(intensity, intensity, intensity);
            Lamp.SetLight(intens);
        }

        public void SetPosition(vector3f pos)
        {
            Pos = pos;
            Lamp.pipeline.SetPosition(Pos);
        }

        public void SetPosition(float X, float Y, float Z)
        {
            Pos.x = X;
            Pos.y = Y; 
            Pos.z = Z;
            Lamp.pipeline.SetPosition(Pos);
        }

        public void Move(vector3f pos)
        {
            Pos += pos;
            Lamp.pipeline.SetPosition(Pos);
        }

        public void Move(float X, float Y, float Z)
        {
            Pos.x += X;
            Pos.y += Y;
            Pos.z += Z;
            Lamp.pipeline.SetPosition(Pos);
        }

        public vector3f Color
        {
            get => BaseLight.Color;
        }

        public float Intensity
        {
            get => BaseLight.Intensity;
        }

        public vector3f Position
        {
            get => Pos;
        }

        public void SetLampScale(float scale)
        {
            Lamp.pipeline.SetScale(scale);
        }
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
