using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
        public Vector2f MoveSpeed { get; set; }
        public float Size { get; set; }
        public  Vector2f AttackSpeed { get; set; }
        public  Vector2f Damage { get; set; }
        public  Vector2f Range { get; set; }
        public Vector2f HP { get; set; }
    }
}
