using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DunnoAlternative.World
{
    public enum PlayerType { human, CPU, passive }

    internal class Player
    {
        public bool Alive;
        public string Name;
        public PlayerType Type { get; }
        public Color Color { get; }

        public Player(PlayerType type, string name, Color color)
        {
            Color = color;
            Alive = true;
            Type = type;
            Name = name;
        }
        
        //Heroes
        //Squads
        //Money
    }
}
