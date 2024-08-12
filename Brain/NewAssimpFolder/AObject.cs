using game_2.Brain.PictureOnScreen;
using OpenTK.Mathematics;
namespace game_2.Brain.NewAssimpFolder
{
    public class AObject
    {
        protected AScene scene;
        public PhysicalModel physic;

        public bool isSculpture;
        public bool isPhysical = false;

        public AObject(string ModelFilePath) 
        {
            isSculpture = false;
            scene = new AScene(ModelFilePath);
            physic = new PhysicalModel();
        }

        public void onRender(float deltaTime)
        {
            if (isPhysical)
            {
                physic.Acceleration(0, 0, 0, deltaTime);
                Move(physic.speedX, physic.speedY, physic.speedZ, deltaTime);
            } 
        }

        public void Draw(Shader shader) => scene.Draw(shader, physic._pipeline);
        public void Draw(Shader shader, Matrix4 view, Matrix4 pers) => scene.Draw(shader, physic._pipeline, view, pers);

        public void OnDelete() => scene.OnDelete();

        #region Transformation
        // УСТАНОВИТЬ
        public void SetScale(float scale)
        {
            physic._pipeline.SetScale(scale);
        }

        public void SetScale(float scaleX, float scaleY, float scaleZ)
        {
            physic._pipeline.SetScale(scaleX, scaleY, scaleZ);
        }

        public void SetAngle(float angleX, float angleY, float angleZ)
        {
            physic._pipeline.SetAngle(angleX, angleY, angleZ);
        }

        public void SetPosition(float PosX, float PosY, float PosZ)
        {
            physic._pipeline.SetPosition(PosX, PosY, PosZ);
        }

        // НЕМЕДЛЕННО ОБНОВИТЬ
        public void ExpandImmediately(float scaleX, float scaleY, float scaleZ)
        {
            physic._pipeline.SetScale(physic._pipeline.ScaleX + scaleX, physic._pipeline.ScaleY + scaleY, physic._pipeline.ScaleZ + scaleZ);
        }
        public void ExpandImmediately(float value)
        {
            physic._pipeline.SetScale(physic._pipeline.ScaleX + value, physic._pipeline.ScaleY + value, physic._pipeline.ScaleZ + value);
        }
        public void RotateImmediately(float angleX, float angleY, float angleZ)
        {
            physic._pipeline.SetAngle(physic._pipeline.AngleX + angleX, physic._pipeline.AngleY + angleY, physic._pipeline.AngleZ + angleZ);
        }

        public void MoveImmediately(float PosX, float PosY, float PosZ)
        {
            physic._pipeline.SetPosition(physic._pipeline.PosX + PosX, physic._pipeline.PosY + PosY, physic._pipeline.PosZ + PosZ);
        }

        // СКОРОСТЬ * ВРЕМЯ
        public void Rotate(float speedX, float speedY, float speedZ, float time)
        {
            physic._pipeline.Rotate(speedX, speedY, speedZ, time);
        }

        public void Move(float speedX, float speedY, float speedZ, float time)
        {
            physic._pipeline.Move(speedX, speedY, speedZ, time);
        }

        public void Expand(float speedX, float speedY, float speedZ, float time)
        {
            physic._pipeline.Expand(speedX, speedY, speedZ, time);
        }

        public void Expand(float speedVal, float time)
        {
            physic._pipeline.Expand(speedVal, time);
        }

        public void Reset()
        {
            physic._pipeline.Reset();
        }
        #endregion
    }
}
