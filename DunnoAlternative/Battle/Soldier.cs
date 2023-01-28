using DunnoAlternative.Shared;
using DunnoAlternative.World;
using SFML.Graphics;
using SFML.System;

namespace DunnoAlternative.Battle
{
    internal class Soldier : IDrawable
    {
        public SquadType SquadType { get; }
        public Vector2f Pos { get; set; }
        public Vector2f Target { get; set; }

        public float Size { get; }
        public bool Alive { get; private set; }

        private readonly Sprite sprite;
        private readonly RectangleShape healthBarForeground;
        private readonly RectangleShape healthBarBackground;

        private readonly float moveSpeed;
        private readonly Vector2f attackSpeed;
        private readonly Vector2f damage;
        private readonly float range;
        private float hp;
        private readonly float maxHp;

        private int delay;

        private const int HEALTH_BAR_SIZE_Y = 5;

        public Soldier(SquadType squadType, Vector2f initialPos, Player player)
        {
            SquadType = squadType;
            Pos = initialPos;
            sprite = new Sprite(squadType.Texture);
            moveSpeed = SquadType.MoveSpeed.RandomFromRange();
            Size = squadType.Size;
            attackSpeed = SquadType.AttackSpeed;
            damage = SquadType.Damage;
            range = SquadType.Range.RandomFromRange();
            hp = SquadType.HP.RandomFromRange();
            maxHp = hp;

            Alive = true;

            healthBarForeground = new RectangleShape
            {
                Size = new Vector2f(sprite.Texture.Size.X, HEALTH_BAR_SIZE_Y),
                FillColor = player.Color,
            };

            healthBarBackground = new RectangleShape
            {
                Size = new Vector2f(sprite.Texture.Size.X, HEALTH_BAR_SIZE_Y),
                FillColor = Color.Black,
            };
        }

        public void Draw(RenderWindow window)
        {
            if (!Alive) return;

            sprite.Position = Pos;
            healthBarBackground.Position = Pos - new Vector2f(0, sprite.Texture.Size.Y / 2);
            healthBarForeground.Position = healthBarBackground.Position;

            window.Draw(sprite);
            window.Draw(healthBarBackground);
            window.Draw(healthBarForeground);
        }

        public void Update(List<Soldier> enemies)
        {   
            if (!Alive) return;

            delay--;

            var target = FindNearestEnemy(enemies);

            if (target == null) return;

            var delta = (target.Pos - Pos);

            if (delay <= 0 && delta.Length() < range + target.Size)
            {
                target.Damage(damage.RandomFromRange());
                
                delay = (int)attackSpeed.RandomFromRange();
            } 

            if (delta.Length() < moveSpeed)
            {
                Pos = target.Pos;
            }
            else
            {
                Pos += (target.Pos - Pos).Normalize() * moveSpeed;
            }
        }

        public void Damage(float damage)
        {
            hp -= damage;

            if(hp < 0)
            {
                Alive = false;
            }
            else
            {
                healthBarForeground.Size = new Vector2f(sprite.Texture.Size.X * (hp / maxHp), HEALTH_BAR_SIZE_Y);
            }
        }

        private Soldier? FindNearestEnemy(List<Soldier> enemies)
        {
            if (enemies.Count == 0) return null;

            return enemies.MinBy(x => (Pos - x.Pos).Length());
        }
    }
}
