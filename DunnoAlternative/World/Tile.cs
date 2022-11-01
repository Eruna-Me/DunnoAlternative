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
    internal class Tile : IDrawable
    {
        private static readonly Vector2f tileSize = new(64.0f,64.0f);
        public Player Owner { get; set; }
        public string Name { get; set; }

        private readonly RectangleShape shape;
        //terrain type
        //buildings
        //Water/other special impassible tiles?

        public Tile(string name, Player owner, Vector2f position)
        {
            Owner = owner;
            Name = name;
            shape = new RectangleShape(tileSize)
            {
                Position = position,
                FillColor = owner.Color,
            };
        }

        public void Draw(RenderWindow window)
        {
            window.Draw(shape);
        }
    }
}
