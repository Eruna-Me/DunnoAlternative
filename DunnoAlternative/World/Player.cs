using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DunnoAlternative.World
{
    public enum PlayerType { human, CPU, passive }

    public class Player
    {
        public bool Alive;
        public string Name;
        public PlayerType Type { get; }
        public Color Color { get; }
        public List<Squad> UnassignedSquads { get; set; }
        public int Money { get; set; }

        public Player(PlayerType type, string name, Color color)
        {
            Color = color;
            Alive = true;
            Type = type;
            Name = name;
            UnassignedSquads = new List<Squad>();
        }
        
        //Heroes
    }
}
