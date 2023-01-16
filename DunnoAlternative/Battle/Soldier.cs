using DunnoAlternative.Shared;
using DunnoAlternative.World;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DunnoAlternative.Battle
{
    internal class Soldier : IDrawable
    {
        public SquadType SquadType { get;  }
        public Vector2f Pos { get; set; }
        private Sprite sprite;

        public Soldier(SquadType squadType, Vector2f initialPos)
        {
            SquadType = squadType;
            Pos = initialPos;
            sprite = new Sprite(squadType.Texture);
        }

        public void Draw(RenderWindow window)
        {
            sprite.Position = Pos;
            window.Draw(sprite);
        }
    }
}
