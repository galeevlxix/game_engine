using game_2.Brain.PictureOnScreen;
using game_2.MathFolder;

namespace game_2.Brain.NewAssimpFolder
{
    public class SculptureObject : AObject
    {
        protected PictureObject pictureObject;
        public string description;

        public vector3f StartingAngle;
        public vector3f StartingPosition;
        public float StartingScale;

        public SculptureObject(string ModelFilePath) : base(ModelFilePath)
        {
            isSculpture = true;
            description = "";
            StartingAngle = new vector3f();
            StartingPosition = new vector3f();
            StartingScale = 1;
        }

        public void SetPicture(string path) => 
            pictureObject = new PictureObject(path);

        public void DrawPicture() => 
            pictureObject.Draw();
    }
}
