using DunnoAlternative.Shared;
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

        public HeroClass heroClass { get; set; }
        public Texture texture { get; set; }

        public Hero(HeroClass heroClass, Texture texture, List<Squad> squads) 
        {
            Squads = squads;

            var names = File.ReadAllLines("Content/Namelists/Test.txt");

            this.texture = texture;
            this.heroClass = heroClass;

            Name = names[Global.random.Next(names.Length)];
        }
    }
}
