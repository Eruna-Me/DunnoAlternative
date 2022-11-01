using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DunnoAlternative.Shared;
using DunnoAlternative.State;
using SFML.Graphics;
using SFML.System;

namespace DunnoAlternative.World
{
    internal class WorldState : IState
    {
        private readonly Tile[,] tiles;
        private readonly List<Player> players;

        public WorldState()
        {
            players = new List<Player> {
                new Player(PlayerType.human, "Humanland", Color.Blue),
                new Player(PlayerType.CPU, "Robofactory", Color.Red),
                new Player(PlayerType.CPU, "Bob5", Color.Magenta),
                new Player(PlayerType.passive, "Rebels", Color.Yellow), 
            };

            tiles = new Tile[,] {
                { new Tile("First Tile", players[0], new Vector2f(0,0)), new Tile("Second Tile", players[1], new Vector2f(64,0)), },
                { new Tile("Other Tile", players[2], new Vector2f(0,64)), new Tile("Rebel Mountain", players[3], new Vector2f(64,64)), },
            };
        }

        public void Draw(RenderWindow window)
        {
            foreach (var tile in tiles)
            {
                tile.Draw(window);
            }
        }

        public void Update()
        {
        }
    }
}
