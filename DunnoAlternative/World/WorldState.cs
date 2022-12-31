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
using ErunaUI.Text;
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
        private int currentPlayerIndex = 0;
        private Player currentPlayer;
        private readonly Control currentPlayerIndicator;

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

            currentPlayer = players[currentPlayerIndex];

            var font = new Font("Content/Fonts/KosugiMaru-Regular.ttf");

            currentPlayerIndicator = new TextLabel(font)
            {
                Background = currentPlayer.Color,
                BorderColor = Color.Black,
                BorderThickness = 3,
                TrueHeight = 100,
                TrueWidth = 300,
                PosX = 200,
                PosY = 20,
                TextString = "End Turn",
            };

            currentPlayerIndicator.ClickEvent += EndTurn;

            var demoUI = new Window
            {
                Child = currentPlayerIndicator
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

        private void EndTurn()
        {
            currentPlayerIndex++;
            if(currentPlayerIndex >= players.Count)
            {
                currentPlayerIndex = 0;
            }

            currentPlayer = players[currentPlayerIndex];
            currentPlayerIndicator.Background = currentPlayer.Color;
            //currentplayer.turnstart

            if(currentPlayer.Type == PlayerType.passive) EndTurn();
        }

        public void Update(RenderWindow window)
        {
            inputManager.Update(window);
            windowManager.Update();
        }
    }
}
