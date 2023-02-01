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

        private readonly float baseMoveSpeed;
        private float moveSpeed;
        private float hp;
        private readonly float maxHp;

        public List<SoldierAttack> Attacks;

        private float postAttack;

        private const int HEALTH_BAR_SIZE_Y = 5;

        SoldierAttack? currentAttack;
        float preAttack;

        public Soldier(SquadType squadType, Vector2f initialPos, Player player)
        {
            SquadType = squadType;
            Pos = initialPos;
            sprite = new Sprite(squadType.Texture);
            baseMoveSpeed = SquadType.MoveSpeed.RandomFromRange();
            Size = squadType.Size;
            Attacks= new List<SoldierAttack>();

            foreach(Attack attack in SquadType.Attacks)
            {
                Attacks.Add(new SoldierAttack(attack));
            }

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

            postAttack--;
            preAttack--;

            var target = FindNearestEnemy(enemies);

            if (target == null) return;

            var delta = (target.Pos - Pos);

            if (currentAttack != null)
            {
                var attack = currentAttack.Value;

                if (delta.Length() < attack.continueRangeMax + target.Size + Size)
                {
                    if (preAttack <= 0)
                    {
                        target.Damage(attack.damage.RandomFromRange());
                        attack.Ammo--;
                        postAttack = attack.postTime.RandomFromRange();
                        moveSpeed = baseMoveSpeed * attack.moveSpeedMultPost;
                        currentAttack = null;
                    }
                }
                else
                {
                    currentAttack = null;
                }
            }
            else if (postAttack <= 0)
            {
                moveSpeed = baseMoveSpeed;

                foreach (var attack in Attacks)
                {
                    if (attack.Ammo != 0 && delta.Length() < attack.initRangeMax + target.Size + Size)
                    {
                        preAttack = attack.preparationTime.RandomFromRange();
                        currentAttack = attack;
                        moveSpeed = baseMoveSpeed * attack.moveSpeedMultPre;
                        break;
                    }
                }
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
