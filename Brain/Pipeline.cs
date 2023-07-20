using game_2.MathFolder;

namespace game_2.Brain
{
    public class Pipeline
    {
        private vector3f RotateVector;
        private vector3f ScaleVector;
        private vector3f PositionVector;

        public Pipeline()
        {
            RotateVector = vector3f.Zero;
            ScaleVector = vector3f.One;
            PositionVector = vector3f.Zero;
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

        public void Reset()
        {
            Position(0, 0, 0);
            Rotate(0, 0, 0);
            Scale(1, 1, 1);
        }        

        public matrix4f getMVP()
        {
            matrix4f scaleTrans = new matrix4f();
            matrix4f rotateTrans = new matrix4f();
            matrix4f translationTrans = new matrix4f();

            scaleTrans.InitScaleTransform(ScaleVector.x, ScaleVector.y, ScaleVector.z);
            rotateTrans.Rotate(RotateVector.x, RotateVector.y, RotateVector.z);
            translationTrans.InitTranslationTransform(PositionVector.x, PositionVector.y, PositionVector.z);

            return scaleTrans * rotateTrans * translationTrans;
        }
    }
}
