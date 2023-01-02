using DunnoAlternative.Shared;
using ErunaInput;
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
        public static readonly Vector2f Size = new(64.0f,64.0f);
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
            shape = new RectangleShape(Size)
            {
                Position = position,
            };

            ChangeOwner(owner);
        }

        public void ChangeOwner(Player newOwner)
        {
            Owner = newOwner;
            shape.FillColor = newOwner.Color;
        }

        public void Draw(RenderWindow window)
        {
            window.Draw(shape);
        }
        public bool IsMouseOver(Vector2i mousePos)
        {
            return mousePos.X >= shape.Position.X
                && mousePos.X <= shape.Position.X + Size.Y
                && mousePos.Y >= shape.Position.Y
                && mousePos.Y <= shape.Position.Y + Size.Y;
        }
    }
}
