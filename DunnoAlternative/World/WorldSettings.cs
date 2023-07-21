using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DunnoAlternative.World
{
    public struct WorldSettings
    {
        public int height; // 3 - 12 ?
        public int width;

        public float percentWater;
        public float percentPassive;

        public bool cpuAdvancedStarts; // allow generating of cpu opponents with 3-5 ? connected territories instead if 1 and some additional heroes 
        public bool cpuHugeStarts; // same except with more territories?
        public bool cpuSpreadOutStarts; // allow generating of cpu opponents with 3-5 disconnected territories instead if 1 and some additional heroes 

        List<PlayerSettings> humanPlayers;

        //seed

        //public WorldShape worldShape -> Water at borders of map, land on borders and water on the middle, weird spaghetti, whatever
    }
}
