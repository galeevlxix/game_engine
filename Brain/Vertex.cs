using game_2.MathFolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game_2.Brain
{
    public struct Vertex
    {
        public vector3f _pos;
        public vector2f _tex;
        public vector3f _normal;
        public vector3f _tangent;

        public static int Size
        {
            get
            {
                return System.Runtime.InteropServices.Marshal.SizeOf (typeof (Vertex));
            }
        }

        public Vertex()
        {
            _pos = new vector3f();
            _tex = new vector2f();
            _normal = new vector3f();
            _tangent = new vector3f();
        }

        public Vertex(vector3f pos, vector2f tex, vector3f normal, vector3f tangent)
        {
            _pos = pos;
            _tex = tex;
            _normal = normal;
            _tangent = tangent;
        }
    }
}
