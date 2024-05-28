using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game_2.Brain.PictureOnScreen
{
    public class PictureObject
    {
        private PictureMesh mesh;
        public Pipeline pipeline;

        public PictureObject(string ImagePath)
        {
            mesh = new PictureMesh(ImagePath);
            pipeline = new Pipeline();

            pipeline.SetPosition(-0.4f, -0.13f, -1);
            pipeline.SetScale(0.3f);
        }

        public void Draw()
        {
            mesh.Draw(pipeline.getWorld());
        }

        public void OnDelete()
        {
            mesh.Dispose();
        }
    }
}
