using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DunnoAlternative.Battle;
using DunnoAlternative.State;
using ErunaInput;
using ErunaUI;
using ErunaUI.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace DunnoAlternative.World
{
    public class WorldState : IState
    {
        private readonly Tile[,] tiles;
        private readonly List<Player> players;
        private readonly WindowManager windowManager;
        private readonly InputManager inputManager;
        private int currentPlayerIndex = 0;
        private Player currentPlayer;
        private readonly Control currentPlayerIndicator;
        private readonly TextLabel moneyIndicator;

        private ErunaUI.Window? buildWindow;
        private ErunaUI.Window? invasionWindow;
        private ErunaUI.Window? squadRecruitWindow;
        private ErunaUI.Window? battleSetupWindow;

        private BattleSetupUI? battleSetupUI;
        private readonly RenderWindow renderWindow;

        const int POPUP_WINDOW_WIDTH = 250;
        const int POPUP_BUTTON_HEIGHT = 80;

        private readonly Font font = new("Content/Fonts/KosugiMaru-Regular.ttf");

        Vector2i invadedTile;

        private readonly List<SquadType> squadTypes;

        private readonly StateManager stateManager;
        private readonly Camera camera; //TODO: dispose
        private readonly CameraControls controls; //TODO: dispose

        public WorldState(RenderWindow window, StateManager stateManager)
        {
            inputManager = new InputManager();
            camera = new Camera(window);
            controls = new CameraControls(window, camera);

            renderWindow = window;

            
            this.stateManager = stateManager;

            players = new List<Player> {
                new Player(PlayerType.human, "Humanland", Color.Blue),
                new Player(PlayerType.CPU, "Robofactory", Color.Red),
                new Player(PlayerType.CPU, "Bob5", Color.Magenta),
                new Player(PlayerType.passive, "Rebels", Color.Yellow),
            };

            tiles = new Tile[,] {
                { new Tile(font, "First Tile", players[0], 300, new Vector2f(0,0)), new Tile(font, "Second Tile", players[1], 300, new Vector2f(0,Tile.Size.Y)), },
                { new Tile(font, "Other Tile", players[2], 300, new Vector2f(Tile.Size.X,0)), new Tile(font, "Rebel Mountain", players[3], 300, new Vector2f(Tile.Size.X,Tile.Size.Y)), },
            };

            currentPlayer = players[currentPlayerIndex];

            currentPlayerIndicator = new TextLabel(font)
            {
                Background = currentPlayer.Color,
                BorderColor = Color.Black,
                BorderThickness = 3,
                TextString = "End Turn",
            };

            moneyIndicator = new TextLabel(font)
            {
                Background = Color.Blue,
                BorderColor = Color.Black,
                BorderThickness = 3,
                TextString = "$0",
            };

            var btnRecruitSquad = new TextLabel(font)
            {
                Background = Color.Blue,
                BorderColor = Color.Black,
                BorderThickness = 3,
                TextString = "Recruit Squads",
            };

            var demoGrid = new Grid
            {
                Rows = GridRow.GenerateRows(2),
                Columns = GridRow.GenerateRows(2),
                TrueHeight = 100,
                TrueWidth = 800,
                PosX = 0,
                PosY = 500,
            };

            demoGrid.Children.Add(new Cell(currentPlayerIndicator, new List<int> { 1 }, new List<int> { 0 }));
            demoGrid.Children.Add(new Cell(moneyIndicator, new List<int> { 1 }, new List<int> { 1 }));
            demoGrid.Children.Add(new Cell(btnRecruitSquad, new List<int> { 0 }, new List<int> { 0,1 }));

            btnRecruitSquad.ClickEvent += CreateSquadRecruitWindow;
            currentPlayerIndicator.ClickEvent += EndTurn;

            var demoUI = new ErunaUI.Window
            {
                Child = demoGrid
            };
            demoUI.UpdateSizes();

            windowManager = new WindowManager(inputManager);
            windowManager.AddWindow(demoUI);

            squadTypes = new List<SquadType>();

            foreach(var file in new DirectoryInfo("Content/Squadtypes").GetFiles())
            {
                squadTypes.Add(JObject.Parse(File.ReadAllText(file.FullName)).ToObject<SquadType>());
            };

            TurnStart();
        }

        public void Update(RenderWindow window)
        {
            inputManager.Update(window);
            windowManager.Update();
            battleSetupUI?.Update();

            if (inputManager.MouseButtonState[Mouse.Button.Left] == (ButtonState.Release, false))
            {
                ClosePopupWindows();

                invadedTile = MousePosToTile(inputManager.MousePos);
                if (TileExists(invadedTile))
                {
                    if (tiles[invadedTile.X, invadedTile.Y].Owner == currentPlayer)
                    {
                        var buttons = new List<(string, Action)>
                        {
                            ("Upgrade Castle", ()=>{ }),
                        };

                        buildWindow = CreatePopupWindow(inputManager.MousePos, buttons);
                        windowManager.AddWindow(buildWindow);
                    }
                    else
                    {
                        if (CheckNeighbors(invadedTile))
                        {

                            var buttons = new List<(string, Action)>
                            {
                                ("Invade", ()=>{
                                    ClosePopupWindows();

                                    battleSetupWindow = new ErunaUI.Window();

                                    battleSetupUI = new BattleSetupUI(battleSetupWindow, font, 300, 500, currentPlayer.Heroes, tiles[invadedTile.X, invadedTile.Y].Owner);
                                    battleSetupUI.OnAttackerFinished += AttackerSetupFinished;
                                    battleSetupUI.OnSetupCanceled += ClosePopupWindows;

                                    battleSetupWindow.UpdateSizes();

                                    windowManager.AddWindow(battleSetupWindow);
                                }),
                            };

                            invasionWindow = CreatePopupWindow(inputManager.MousePos, buttons);
                            windowManager.AddWindow(invasionWindow);
                        }
                    }
                }
            }
        }

        public void BattleResult(bool attackerWon)
        {
            if (attackerWon)
            {
                var oldOwner = tiles[invadedTile.X, invadedTile.Y].Owner;

                tiles[invadedTile.X, invadedTile.Y].ChangeOwner(currentPlayer);

                CheckPlayerAlive(oldOwner);
            }

            stateManager.Pop();
        }

        const int BASE_INCOME = 500;

        private void TurnStart()
        {
            currentPlayer.Money += BASE_INCOME;

            foreach(var tile in tiles)
            {
                if(tile.Owner == currentPlayer)
                {
                    currentPlayer.Money += tile.Income;
                }
            }

            moneyIndicator.TextString = "$" + currentPlayer.Money;
        }

        private void CheckPlayerAlive(Player player)
        {
            foreach (var tile in tiles)
            {
                if (tile.Owner == player) break;
            }

            player.Alive = false;
        }

        private void CreateSquadRecruitWindow()
        {
            var buttons = new List<(string, Action)>();

            foreach (var type in squadTypes)
            {
                var hero = new Hero(new List<Squad> { new Squad(type), new Squad(type) });
                int cost = hero.Squads.Sum(x => x.Type.Cost );

                buttons.Add((hero.Name + " - $" + cost, () =>
                {
                    if (currentPlayer.Money >= cost)
                    {
                        currentPlayer.Money -= cost;
                        moneyIndicator.TextString = "$" + currentPlayer.Money;
                        currentPlayer.Heroes.Add( hero);
                        ClosePopupWindows();
                        CreateSquadRecruitWindow();
                    }
                    else
                    {
                        //todo not enough gold popup?
                    }
                }
                ));
            }

            squadRecruitWindow = CreatePopupWindow(new Vector2i(100, 100), buttons);
            windowManager.AddWindow(squadRecruitWindow);
        }

        public void Draw(RenderWindow window)
        {
            window.SetView(camera.GetWorldView());
            foreach (var tile in tiles)
            {
                tile.Draw(window);
            }
            
            window.SetView(camera.GetUiView());
            windowManager.OnDraw(window);
        }

        private void EndTurn()
        {
            ClosePopupWindows();

            currentPlayerIndex++;
            if (currentPlayerIndex >= players.Count)
            {
                currentPlayerIndex = 0;
            }

            currentPlayer = players[currentPlayerIndex];
            currentPlayerIndicator.Background = currentPlayer.Color;
            TurnStart();

            if (currentPlayer.Type == PlayerType.passive || currentPlayer.Alive == false) EndTurn();
        }

        private void ClosePopupWindows()
        {
            CloseWindow(buildWindow);
            CloseWindow(invasionWindow);
            CloseWindow(squadRecruitWindow);
            CloseWindow(battleSetupWindow);

            if (battleSetupUI != null)
            {
                battleSetupUI.OnSetupCanceled -= ClosePopupWindows;
                battleSetupUI.OnAttackerFinished -= AttackerSetupFinished;
                battleSetupUI.OnSetupFinished -= DefenderSetupFinished;
            }
        }

        private void AttackerSetupFinished(Hero[,] attackers, Player defender)
        {
            ClosePopupWindows();

            battleSetupWindow = new ErunaUI.Window();

            battleSetupUI = new BattleSetupUI(battleSetupWindow, font, 300, 500, defender.Heroes, defender, attackers);
            battleSetupUI.OnSetupFinished += DefenderSetupFinished;
            battleSetupUI.OnSetupCanceled += ClosePopupWindows;

            battleSetupWindow.UpdateSizes();

            windowManager.AddWindow(battleSetupWindow);
        }

        private void DefenderSetupFinished(Hero[,] attackers, Hero[,] defenders, Player defender)
        {
            ClosePopupWindows();

            stateManager.Push(new BattleState(this, renderWindow ,attackers, defenders, currentPlayer, defender));
        }

        private void CloseWindow(ErunaUI.Window? window)
        {
            if (window != null)
            {
                windowManager.RemoveWindow(window);
            }
        }

        private Vector2i MousePosToTile(Vector2i mousepos)
        {
            var world = camera.ScreenToWorld(mousepos);
            Vector2i index = new((int)(world.X / Tile.Size.X), (int)(world.Y / Tile.Size.Y));

            return index;
        }

        private bool TileExists(Vector2i index)
        {
            return index.X >= 0 && index.Y >= 0 && index.X <= tiles.GetUpperBound(0) && index.Y <= tiles.GetUpperBound(1);
        }

        private bool CheckNeighbors(Vector2i index)
        {
            List<Vector2i> directions = new() {
                new Vector2i(0, 1),
                new Vector2i(0, -1),
                new Vector2i(1,0),
                new Vector2i(-1, 0),
            };

            foreach (var direction in directions)
            {
                var checkPos = direction + index;

                if (!TileExists(checkPos)) continue;

                if (tiles[checkPos.X, checkPos.Y].Owner == currentPlayer) return true;
            }

            return false;
        }

        private ErunaUI.Window CreatePopupWindow(Vector2i position, List<(string, Action)> buttons)
        {
            ClosePopupWindows();

            var stackPanel = new StackPanel
            {
                PosX = position.X,
                PosY = position.Y,
                TrueWidth = POPUP_WINDOW_WIDTH,
                TrueHeight = POPUP_BUTTON_HEIGHT * buttons.Count,
                Background = Color.Blue
            };

            foreach (var (text, action) in buttons)
            {
                var button = new TextLabel(font)
                {
                    Color = Color.White,
                    TextString = text,
                    TextAlign = TextAlign.Center,
                    TextGravity = TextGravity.Center,
                    Height = POPUP_BUTTON_HEIGHT,
                    Background = Color.Blue,
                    Margin = 2
                };
                button.ClickEvent += action;
                stackPanel.Children.Add(button);
            }

            var window = new ErunaUI.Window
            {
                Child = stackPanel,
            };

            window.UpdateSizes();

            return window;
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
