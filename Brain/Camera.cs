using OpenTK.Mathematics;

namespace game_2.Brain
{
    public class Camera
    {
        private Matrix4 PersProjMat;
        private Matrix4 WorldView;

        public Camera()
        {
            //проекция перспективы
            PersProjMat = Pipeline.getPersProjMatrix(new mPersProj());
            WorldView = Matrix4.Identity;
        }

        public void setWVM(Matrix4 wvm) { WorldView = wvm; }

        public Matrix4 getMatrix { get { return PersProjMat * WorldView; } }
    }
}
