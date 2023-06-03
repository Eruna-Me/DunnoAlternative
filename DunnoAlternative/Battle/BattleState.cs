using System;
using System.Linq;
using System.Text;
using DunnoAlternative.Shared;
using DunnoAlternative.State;
using DunnoAlternative.World;
//using ErunaUI;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace DunnoAlternative.Battle
{
    public class BattleState : IState
    {
        private readonly WorldState world;

        readonly List<Soldier> attackers;
        readonly List<Soldier> defenders;
        readonly List<Soldier> soldiers;

        private readonly Camera camera; //TODO: dispose
        private readonly CameraControls controls; //TODO: dispose

        const float CENTER_X = 400;
        const float ARMY_DISTANCE_FROM_CENTER_X = 200;
        const float ARMY_DISTANCE_FROM_TOP = 100;
        const float INITIAL_SOLDIER_SPACING_Y = 30;
        const float INITIAL_SQUAD_SPACING_X = 100;
        const float INITIAL_HERO_SPACING_X = 100;
        const float INITIAL_HERO_SPACING_Y = 125;

        private readonly BattleTerrain battleTerrain;

        public BattleState(WorldState world, RenderWindow window, Hero[,] attackers, Hero[,] defenders, Player attacker, Player defender)
        {
            this.attackers = ArrangeSoldiers(attackers, attacker, -1);
            this.defenders = ArrangeSoldiers(defenders, defender, 1);
            soldiers = new List<Soldier>(this.attackers);
            soldiers.AddRange(this.defenders);

            camera = new Camera(window);
            controls = new CameraControls(window, camera);

            battleTerrain = new BattleTerrain(new Texture("Content/Textures/Terrain.png"), new Vector2u(128,128), new Vector2u(20,20));

            this.world = world;
        }

        private static List<Soldier> ArrangeSoldiers(Hero[,] heroes, Player player, float invert)
        {
            var soldiers = new List<Soldier>();

            for (int x = 0; x <= heroes.GetUpperBound(0); x++)
            {
                for (int y = 0; y <= heroes.GetUpperBound(1); y++)
                {
                    if (heroes[x, y] == null) continue;

                    soldiers.Add(new Soldier(
                        heroes[x, y].heroClass,
                        heroes[x, y].texture,
                        new Vector2f(CENTER_X + (ARMY_DISTANCE_FROM_CENTER_X + INITIAL_SQUAD_SPACING_X * -1 + INITIAL_HERO_SPACING_X * x) * invert,
                                     ARMY_DISTANCE_FROM_TOP + y * INITIAL_HERO_SPACING_Y + 1 * INITIAL_SOLDIER_SPACING_Y),
                        player
                        )
                    );

                    for (int m = 0; m < heroes[x, y].Squads.Count; m++)
                    {
                        for (int n = 0; n < heroes[x, y].Squads[m].Soldiers; n++)
                        {
                            soldiers.Add(new Soldier(
                                heroes[x, y].Squads[m].Type,
                                new Vector2f(CENTER_X + (ARMY_DISTANCE_FROM_CENTER_X + INITIAL_SQUAD_SPACING_X * m + INITIAL_HERO_SPACING_X * x) * invert,
                                            ARMY_DISTANCE_FROM_TOP + y * INITIAL_HERO_SPACING_Y + n * INITIAL_SOLDIER_SPACING_Y),
                                player
                                )
                            );
                        }
                    }
                }
            }

            return soldiers;
        }

        public void Draw(RenderWindow window)
        {
            window.SetView(camera.GetWorldView());

            battleTerrain.Draw(window);

            foreach (Soldier soldier in soldiers.OrderBy(x => x.Pos.Y))
            {
                soldier.Draw(window);
            }

            window.SetView(camera.GetUiView());
            //windowManager.OnDraw(window);
        }

        public void Update(RenderWindow window)
        {
            foreach (var soldier in attackers)
            {
                soldier.Update(defenders);
            }
            foreach (var soldier in defenders)
            {
                soldier.Update(attackers);
            }

            soldiers.RemoveAll(x => !x.Alive);
            defenders.RemoveAll(x => !x.Alive);
            attackers.RemoveAll(x => !x.Alive);

            foreach (var soldierA in soldiers)
            {
                foreach (var SoldierB in soldiers)
                {
                    if (soldierA == SoldierB) continue;

                    HandleCollision(soldierA, SoldierB);
                }
            }

            if (attackers.Count == 0)
            {
                world.BattleResult(false);
            }
            else if (defenders.Count == 0)
            {
                world.BattleResult(true);
            }
        }

        private static void HandleCollision(Soldier a, Soldier b)
        {
            var delta = a.Pos - b.Pos;

            if (delta.Length() < a.Size + b.Size) //if collision
            {
                var norm = delta.Normalize();

                var overlap = a.Size + b.Size - delta.Length();

                a.Pos += overlap * norm / 2;
                b.Pos -= overlap * norm / 2;
            }
        }
        
        public void Unload()
        {
            controls.UnSetupControls();
        }

        public void Load()
        {
            controls.SetupControls();
        }
    }
}
