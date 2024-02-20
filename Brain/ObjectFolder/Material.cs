using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game_2.Brain.ObjectFolder
{
    public class Material
    {
        public float SpecularPower { get; private set; }

        public void SetSpecularPower(float specularPower) => SpecularPower = specularPower;
    }
}
