using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game_2.MathFolder
{
    public class vertex
    {
        public vector3f m_pos;
        public vector2f m_tex;
        public vector3f m_normal;

        public vertex(vector3f pos, vector2f tex, vector3f nor)
        {
            m_pos = pos; 
            m_tex = tex; 
            m_normal = nor;
        }
    }
}
