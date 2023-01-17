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
        List<Soldier> attackers;
        //List<Soldier> badGuys;

        public BattleState(Squad[,] attackers, Squad[,] defenders)
        {
            //foreach(Squad squad in goodGuys)
            //{//

            var demoType = new SquadType
            {
                Texture = new Texture("Content/Textures/Samurai.png"),
            };

            this.attackers = new List<Soldier>
            {
                new Soldier(demoType, new Vector2f(100, 100))
            };
        }

        public void Draw(RenderWindow window)
        {
            foreach(Soldier soldier in attackers)
            {
                soldier.Draw(window);
            }
        }

        public void Update(RenderWindow window)
        {
        }
    }
}
