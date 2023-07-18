using DunnoAlternative.Battle.Particles;
using DunnoAlternative.Shared;
using DunnoAlternative.World;
using Newtonsoft.Json.Linq;
using SFML.Graphics;
using SFML.System;

namespace DunnoAlternative.Battle
{
    internal class Soldier : IDrawable
    {
        private readonly ParticleSystem particleSystem;

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

        ParticleProp? bloodParticle;

        public Soldier(ParticleSystem particleSystem, SquadType squadType, Vector2f initialPos, Player player)
        {
            this.particleSystem = particleSystem;

            Pos = initialPos;
            sprite = new Sprite(squadType.Texture);
            baseMoveSpeed = squadType.MoveSpeed.RandomFromRange();
            Size = squadType.Size;
            Attacks= new List<SoldierAttack>();

            foreach(Attack attack in squadType.Attacks)
            {
                Attacks.Add(new SoldierAttack(attack));
            }

            hp = squadType.HP.RandomFromRange();
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

            if (squadType.BloodParticle != string.Empty)
            {
                bloodParticle = JObject.Parse(File.ReadAllText(squadType.BloodParticle)).ToObject<ParticleProp>();
            }
        }

        public Soldier(ParticleSystem particleSystem, HeroClass heroClass, Texture texture, Vector2f initialPos, Player player)
        {
            this.particleSystem = particleSystem;

            Pos = initialPos;
            sprite = new Sprite(texture);
            baseMoveSpeed = heroClass.MoveSpeed;
            Size = heroClass.Size;
            Attacks = new List<SoldierAttack>();

            foreach (Attack attack in heroClass.Attacks)
            {
                Attacks.Add(new SoldierAttack(attack));
            }

            hp = heroClass.HP;
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

            if (heroClass.BloodParticle != string.Empty)
            {
                bloodParticle = JObject.Parse(File.ReadAllText(heroClass.BloodParticle)).ToObject<ParticleProp>();
            }
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
                if (delta.Length() < currentAttack.continueRangeMax + target.Size + Size)
                {
                    if (preAttack <= 0)
                    {
                        target.Damage(currentAttack.damage.RandomFromRange());
                        currentAttack.Ammo--;
                        postAttack = currentAttack.postTime.RandomFromRange();


                        int skirmish;
                        if (currentAttack.skirmishMaxRange != 0 && 
                            delta.Length() < currentAttack.skirmishMaxRange &&
                            delta.Length() > currentAttack.skirmishMinRange &&
                            delta.Length() < currentAttack.initRangeMax)
                        {
                            skirmish = -1;
                        }
                        else
                        {
                            skirmish = 1;
                        }

                        moveSpeed = baseMoveSpeed * currentAttack.moveSpeedMultPost * skirmish;
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
                        if(delta.Length() < attack.skirmishNoAttackRange &&
                           delta.Length() > attack.skirmishMinRange)
                        {
                            moveSpeed *= -1;
                            break;
                        }

                        preAttack = attack.preparationTime.RandomFromRange();
                        currentAttack = attack;

                        int skirmish;
                        if (attack.skirmishMaxRange != 0 &&
                            delta.Length() < attack.skirmishMaxRange &&
                            delta.Length() > attack.skirmishMinRange &&
                            delta.Length() < attack.initRangeMax)
                        {
                            skirmish = -1;
                        }
                        else
                        {
                            skirmish = 1;
                        }

                        moveSpeed = baseMoveSpeed * attack.moveSpeedMultPre * skirmish;
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

            if (bloodParticle != null)
            {
                bloodParticle.Position = Pos + new Vector2f(sprite.Texture.Size.X * 0.5f, sprite.Texture.Size.Y * 0.5f);
                particleSystem.Emit(bloodParticle, (int)(damage / maxHp * 100));
            }

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
