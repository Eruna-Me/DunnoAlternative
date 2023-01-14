using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DunnoAlternative.World
{
    public class Squad
    {
        public string Name { get; set; }
        public SquadType Type { get; set; }

        public Squad(string name, SquadType type)
        {
            Name = name;
            Type = type;
        }
    }
}
