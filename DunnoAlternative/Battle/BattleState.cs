using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DunnoAlternative.State;
using DunnoAlternative.World;
using SFML.Graphics;
using SFML.System;

namespace DunnoAlternative.Battle
{
    public class BattleState : IState
    {
        readonly List<Soldier> attackers;
        readonly List<Soldier> defenders;

        const float CENTER_X = 400;
        const float ARMY_DISTANCE_FROM_CENTER_X = 200;
        const float ARMY_DISTANCE_FROM_TOP = 100;
        const float INITIAL_SOLDIER_SPACING_Y = 30;
        const float INITIAL_SQUAD_SPACING_X = 100;
        const float INITIAL_SQUAD_SPACING_Y = 125;

        public BattleState(Squad[,] attackers, Squad[,] defenders)
        {
            this.attackers = ArrangeSoldiers(attackers, -1);
            this.defenders = ArrangeSoldiers(defenders, 1);
        }

        private static List<Soldier> ArrangeSoldiers(Squad[,] squads, float invert)
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
                                        ARMY_DISTANCE_FROM_TOP + y * INITIAL_SQUAD_SPACING_Y + n * INITIAL_SOLDIER_SPACING_Y)
                            ));
                    }
                }
            }

            return soldiers;
        }

        public void Draw(RenderWindow window)
        {
            foreach(Soldier soldier in attackers)
            {
                soldier.Draw(window);
            }

            foreach (Soldier soldier in defenders)
            {
                soldier.Draw(window);
            }
        }

        public void Update(RenderWindow window)
        {
        }
    }
}
