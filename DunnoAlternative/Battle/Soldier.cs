using DunnoAlternative.Shared;
using DunnoAlternative.World;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DunnoAlternative.Battle
{
    internal class Soldier : IDrawable
    {
        public SquadType SquadType { get; }
        public Vector2f Pos { get; set; }
        public Vector2f Target { get; set; }

        public float Size { get; set; }
        private Sprite sprite;

        private float moveSpeed;

        public Soldier(SquadType squadType, Vector2f initialPos)
        {
            SquadType = squadType;
            Pos = initialPos;
            sprite = new Sprite(squadType.Texture);
            moveSpeed = SquadType.MoveSpeed;
            Size = squadType.Size;
        }

        public void Draw(RenderWindow window)
        {
            sprite.Position = Pos;
            window.Draw(sprite);
        }

        public void Update(List<Soldier> enemies)
        {
            var target = FindNearestEnemy(enemies);

            if (target == null) return;

            var delta = (target.Pos - Pos);

            if (delta.Length() < moveSpeed)
            {
                Pos = target.Pos;
            }
            else
            {
                Pos += (target.Pos - Pos).Normalize() * moveSpeed;
            }
        }

        private Soldier? FindNearestEnemy(List<Soldier> enemies)
        {
            if (enemies.Count == 0) return null;

            return enemies.MinBy(x => (Pos - x.Pos).Length());
        }
    }
}
