using System;
using System.Linq;
using System.Text;
using DunnoAlternative.Shared;
using DunnoAlternative.State;
using DunnoAlternative.World;
using ErunaUI;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace DunnoAlternative.Battle
{
    public class BattleState : IState
    {
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
        const float INITIAL_SQUAD_SPACING_Y = 125;

        public BattleState(RenderWindow window, Squad[,] attackers, Squad[,] defenders, Player attacker, Player defender)
        {
            this.attackers = ArrangeSoldiers(attackers, attacker, -1);
            this.defenders = ArrangeSoldiers(defenders, defender, 1);
            soldiers = new List<Soldier>(this.attackers);
            soldiers.AddRange(this.defenders);

            camera = new Camera(window);
            controls = new CameraControls(window, camera);

            controls.SetupControls();
        }

        private static List<Soldier> ArrangeSoldiers(Squad[,] squads, Player player, float invert)
        {
            var soldiers = new List<Soldier>();

            for (int x = 0; x <= squads.GetUpperBound(0); x++)
            {
                for (int y = 0; y <= squads.GetUpperBound(1); y++)
                {
                    if (squads[x, y] == null) continue;

                    for (int n = 0; n < squads[x, y].Soldiers; n++)
                    {
                        soldiers.Add(new Soldier(
                            squads[x, y].Type,
                            new Vector2f(CENTER_X + (ARMY_DISTANCE_FROM_CENTER_X + INITIAL_SQUAD_SPACING_X * x) * invert,
                                        ARMY_DISTANCE_FROM_TOP + y * INITIAL_SQUAD_SPACING_Y + n * INITIAL_SOLDIER_SPACING_Y),
                            player
                            )
                        );
                    }
                }
            }

            return soldiers;
        }

        public void Draw(RenderWindow window)
        {
            window.Clear(new Color(0,128,0));

            window.SetView(camera.GetWorldView());

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
    }
}
