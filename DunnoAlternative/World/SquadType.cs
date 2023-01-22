using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DunnoAlternative.World
{
    public struct SquadType
    {
        public Texture Texture { get; set; }
        public string TypeName { get; set; }
        public string DefaultName { get; set; }
        public int Soldiers { get; set; }
        public float MoveSpeed { get; set; }
        public float Size { get; set; }
        public  float AttackSpeed { get; set; }
        public  float Damage { get; set; }
        public  float Range { get; set; }
        public float HP { get; set; }
    }
}
