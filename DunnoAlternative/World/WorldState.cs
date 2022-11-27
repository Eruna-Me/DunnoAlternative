using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using DunnoAlternative.Shared;
using DunnoAlternative.State;
using ErunaInput;
using ErunaUI;
using SFML.Graphics;
using SFML.System;

namespace DunnoAlternative.World
{
    internal class WorldState : IState
    {
        private readonly Tile[,] tiles;
        private readonly List<Player> players;
        private readonly View mainView;
        private readonly View uiView;
        private readonly WindowManager windowManager;
        private readonly InputManager inputManager;

        public WorldState(RenderWindow window)
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
            mainView = new View(new FloatRect(0, 0, window.Size.X, window.Size.Y));
            uiView = new View();

            var demoUI = new Window
            {
                Child = new Grid
                {
                    Background = Color.Red,
                    BorderColor = Color.Green,
                    BorderThickness = 3,
                    TrueHeight = 100,
                    TrueWidth = 300,
                    PosX = 200,
                    PosY = 20,
                }
            };

            inputManager = new InputManager();

            windowManager = new WindowManager(inputManager);
            windowManager.AddWindow(demoUI);
        }

        public void Draw(RenderWindow window)
        {
            window.SetView(mainView);
            foreach (var tile in tiles)
            {
                tile.Draw(window);
            }
            window.SetView(uiView);
            windowManager.OnDraw(window);
        }

        public void Update()
        {
        }
    }
}
