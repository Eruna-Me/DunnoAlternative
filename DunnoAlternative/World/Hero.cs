using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DunnoAlternative.World
{
    public class Hero
    {
        public List<Squad> Squads { get; }
        public string Name { get; set; }

        //public Texture Texture { get; set; }
        //public float MoveSpeed { get; set; }
        //public float Size { get; set; }
        //public List<Attack> Attacks { get; set; }
        //public float HP { get; set; }

        //public Hero(string name, Texture texture, float moveSpeed, float size, List<Attack> attacks, float HP, List<Squad> squads) 
        public Hero(string name, List<Squad> squads) 
        {
            Squads = squads;
            Name = name;
            //Texture = texture;
            //MoveSpeed = moveSpeed;
           // Size = size;
            //Attacks = attacks;
            //this.HP = HP;
        }
    }
}
