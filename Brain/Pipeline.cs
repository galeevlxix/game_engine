using game_2.MathFolder;

namespace game_2.Brain
{
    public class Pipeline
    {
        private vector3f RotateVector;
        private vector3f ScaleVector;
        private vector3f PositionVector;
        public mPersProj mPersProj;
        private matrix4f Transformation;
        private matrix4f MVP_Transformation;
        private m_camera CameraInfo;

        public Pipeline()
        {
            RotateVector = vector3f.Zero;
            ScaleVector = vector3f.One;
            PositionVector = vector3f.Zero;
            mPersProj = new mPersProj();
            CameraInfo = new m_camera();
        }

        public void Rotate(float angleX, float angleY, float angleZ)
        {
            RotateVector.x = angleX;
            RotateVector.y = angleY;
            RotateVector.z = angleZ;
        }

        public void Scale(float ScaleX, float ScaleY, float ScaleZ)
        {
            ScaleVector.x = ScaleX;
            ScaleVector.y = ScaleY;
            ScaleVector.z = ScaleZ;
        }

        public void Scale(float Scale)
        {
            ScaleVector.x = Scale;
            ScaleVector.y = Scale;
            ScaleVector.z = Scale;
        }

        public void Position(float PosX, float PosY, float PosZ)
        {
            PositionVector.x = PosX;
            PositionVector.y = PosY;
            PositionVector.z = PosZ;
        }

        public void PersProj(float FOV, float width, float height, float zNear, float zFar)
        {
            mPersProj.FOV = FOV;
            mPersProj.width = width;
            mPersProj.height = height;
            mPersProj.zNear = zNear;
            mPersProj.zFar = zFar;
        }

        public void SetCamera(vector3f Pos, vector3f Target, vector3f Up)
        {
            CameraInfo.Pos = Pos;
            CameraInfo.Target = Target;
            CameraInfo.Up = Up;
        }

        public void Reset()
        {
            Position(0, 0, 0);
            Rotate(0, 0, 0);
            Scale(1, 1, 1);
        }        

        public void ChangeWindowSize(float Width, float Height)
        {
            mPersProj.width = Width;
            mPersProj.height = Height;
        }

        public matrix4f getMVP()
        {
            matrix4f scaleTrans = new matrix4f();
            matrix4f rotateTrans = new matrix4f();
            matrix4f translationTrans = new matrix4f();
            matrix4f PersProjTrans = new matrix4f();
            matrix4f CameraTranslation = new matrix4f();
            matrix4f CameraRotate = new matrix4f();

            scaleTrans.InitScaleTransform(ScaleVector.x, ScaleVector.y, ScaleVector.z);
            rotateTrans.Rotate(RotateVector.x, RotateVector.y, RotateVector.z);
            translationTrans.InitTranslationTransform(PositionVector.x, PositionVector.y, PositionVector.z);
            PersProjTrans.InitPersProjTransform(mPersProj.FOV, mPersProj.width, mPersProj.height, mPersProj.zNear, mPersProj.zFar);

            CameraTranslation.InitTranslationTransform(-CameraInfo.Pos.x, -CameraInfo.Pos.y, -CameraInfo.Pos.z);
            CameraRotate.InitCameraTransform(CameraInfo.Target, CameraInfo.Up);                       

            Transformation = scaleTrans * rotateTrans * translationTrans * CameraTranslation * CameraRotate * PersProjTrans;
            return Transformation;
        }
    }

    struct m_camera
    {
        public vector3f Pos;
        public vector3f Target;
        public vector3f Up;
    }
}
