using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DunnoAlternative.World
{
    public struct HeroClass
    {
        public float MoveSpeed { get; set; }
        public float Size { get; set; }
        public List<Attack> Attacks { get; set; }
        public float HP { get; set; }
        public int Cost { get; set; }
        public string Name { get; set; }
    }
}
