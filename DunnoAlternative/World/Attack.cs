using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DunnoAlternative.World
{
    public struct Attack
    {
        public float moveSpeedMultPre;
        public float moveSpeedMultPost;

        public Vector2f preparationTime;

        public Vector2f postTime;
        public Vector2f damage;

        public Vector2f initRangeMax;
        public Vector2f continueRangeMax;

        public int Ammo; //negative for infinite ammo

        //min range
        public bool skirmish;
    }
}
