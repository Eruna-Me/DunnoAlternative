using SFML.Graphics;
using SFML.System;
namespace DunnoAlternative.World
{
    public struct SquadType
    {
        public Texture Texture { get; set; }
        public string Name { get; set; }
        public int Soldiers { get; set; }
        public int Max { get; set; }
        public int Cost { get; set; }
        public Vector2f MoveSpeed { get; set; }
        public float Size { get; set; }
        public List<Attack> Attacks { get; set; }
        public Vector2f HP { get; set; }
    }
}
