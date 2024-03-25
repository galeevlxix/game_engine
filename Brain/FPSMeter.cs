using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game_2.Brain
{
    public static class FPSMeter
    {
        private static double fps = 0;
        private static List<double> fps_list = new List<double>();

        public static int Int_FPS
        {
            get
            {
                return (int)(int)fps;
            }
        }

        public static float Float_FPS
        {
            get
            {
                return (float)fps;
            }
        }

        public static double Double_FPS
        {
            get
            {
                return fps;
            }
        }

        public static void Update(double deltaTime)
        {
            fps_list.Add(1 / deltaTime);
            if (fps_list.Count % 100 == 0)
            {
                fps = fps_list.Average();
                fps_list.Clear();
            }
        }
    }
}
