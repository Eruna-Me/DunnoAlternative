using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DunnoAlternative.World
{
    public class Squad
    {
        public SquadType Type { get; set; }
        public int Soldiers { get; set; }

        public Squad(SquadType type)
        {
            Type = type;
            Soldiers = type.Soldiers;
        }
    }
}
