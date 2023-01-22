using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using DunnoAlternative.Battle;
using DunnoAlternative.Shared;
using DunnoAlternative.State;
using ErunaInput;
using ErunaUI;
using ErunaUI.Text;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace DunnoAlternative.World
{
    internal class WorldState : IState
    {
        private readonly Tile[,] tiles;
        private readonly List<Player> players;
        private readonly WindowManager windowManager;
        private readonly InputManager inputManager;
        private int currentPlayerIndex = 0;
        private Player currentPlayer;
        private readonly Control currentPlayerIndicator;

        private ErunaUI.Window? buildWindow;
        private ErunaUI.Window? invasionWindow;
        private ErunaUI.Window? squadRecruitWindow;
        private ErunaUI.Window? battleSetupWindow;

        private BattleSetupUI? battleSetupUI;
        private readonly RenderWindow renderWindow;

        const int POPUP_WINDOW_WIDTH = 250;
        const int POPUP_BUTTON_HEIGHT = 80;

        private readonly Font font = new("Content/Fonts/KosugiMaru-Regular.ttf");

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

            controls.SetupControls();

            
            this.stateManager = stateManager;

            players = new List<Player> {
                new Player(PlayerType.human, "Humanland", Color.Blue),
                new Player(PlayerType.CPU, "Robofactory", Color.Red),
                new Player(PlayerType.CPU, "Bob5", Color.Magenta),
                new Player(PlayerType.passive, "Rebels", Color.Yellow),
            };

            tiles = new Tile[,] {
                { new Tile("First Tile", players[0], new Vector2f(0,0)), new Tile("Second Tile", players[1], new Vector2f(0,64)), },
                { new Tile("Other Tile", players[2], new Vector2f(64,0)), new Tile("Rebel Mountain", players[3], new Vector2f(64,64)), },
            };


            currentPlayer = players[currentPlayerIndex];

            currentPlayerIndicator = new TextLabel(font)
            {
                Background = currentPlayer.Color,
                BorderColor = Color.Black,
                BorderThickness = 3,
                TextString = "End Turn",
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
                Rows = GridRow.GenerateRows(1),
                Columns = GridRow.GenerateRows(2),
                TrueHeight = 100,
                TrueWidth = 800,
                PosX = 0,
                PosY = 500,
            };

            demoGrid.Children.Add(new Cell(currentPlayerIndicator, new List<int> { 1 }, new List<int> { 0 }));
            demoGrid.Children.Add(new Cell(btnRecruitSquad, new List<int> { 0 }, new List<int> { 0 }));

            btnRecruitSquad.ClickEvent += CreateSquadRecruitWindow;
            currentPlayerIndicator.ClickEvent += EndTurn;

            var demoUI = new ErunaUI.Window
            {
                Child = demoGrid
            };
            demoUI.UpdateSizes();

            windowManager = new WindowManager(inputManager);
            windowManager.AddWindow(demoUI);

            squadTypes = new List<SquadType>
            {
                new SquadType
                {
                    DefaultName = "The Last Samurais",
                    TypeName = "Samurai",
                    Texture = new Texture("Content/Textures/Samurai.png"),
                    Soldiers = 3,
                    MoveSpeed = 50 / Program.LOGIC_UPDATES_PER_SECOND,
                    Size = 16,
                    AttackSpeed = 1.5f * Program.LOGIC_UPDATES_PER_SECOND,
                    HP = 100,
                    Damage = 5,
                    Range = 50,
                }
            };
        }

        private void CreateSquadRecruitWindow()
        {
            var buttons = new List<(string, Action)>();

            foreach (var type in squadTypes)
            {
                buttons.Add((type.TypeName, () =>
                {
                    currentPlayer.UnassignedSquads.Add(new Squad(type.DefaultName, type));
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
            //currentplayer.turnstart

            if (currentPlayer.Type == PlayerType.passive) EndTurn();
        }

        public void Update(RenderWindow window)
        {
            inputManager.Update(window);
            windowManager.Update();
            battleSetupUI?.Update();

            if (inputManager.MouseButtonState[Mouse.Button.Left] == (ButtonState.Release, false))
            {
                ClosePopupWindows();

                var tileIndex = MousePosToTile(inputManager.MousePos);
                if (TileExists(tileIndex))
                {
                    if (tiles[tileIndex.X, tileIndex.Y].Owner == currentPlayer)
                    {
                        var buttons = new List<(string, Action)>
                        {
                            ("Upgrade Castle", ()=>{ }),
                            ("Test", ()=>{ }),
                            ("Dunno", ()=>{ }),
                        };

                        buildWindow = CreatePopupWindow(inputManager.MousePos, buttons);
                        windowManager.AddWindow(buildWindow);
                    }
                    else
                    {
                        if (CheckNeighbors(tileIndex))
                        {

                            var buttons = new List<(string, Action)>
                            {
                                ("InvadeOld", ()=>{ tiles[tileIndex.X, tileIndex.Y].ChangeOwner(currentPlayer); }),
                                ("InvadeAdvanced", ()=>{
                                    ClosePopupWindows();

                                    battleSetupWindow = new ErunaUI.Window();
                           
                                    battleSetupUI = new BattleSetupUI(battleSetupWindow, font, 300, 500, currentPlayer.UnassignedSquads, tiles[tileIndex.X, tileIndex.Y].Owner);
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

        private void AttackerSetupFinished(Squad[,] attackers, Player defender)
        {
            ClosePopupWindows();

            battleSetupWindow = new ErunaUI.Window();

            battleSetupUI = new BattleSetupUI(battleSetupWindow, font, 300, 500, defender.UnassignedSquads, defender, attackers);
            battleSetupUI.OnSetupFinished += DefenderSetupFinished;
            battleSetupUI.OnSetupCanceled += ClosePopupWindows;

            battleSetupWindow.UpdateSizes();

            windowManager.AddWindow(battleSetupWindow);
        }

        private void DefenderSetupFinished(Squad[,] attackers, Squad[,] defenders, Player defender)
        {
            ClosePopupWindows();

            stateManager.Push(new BattleState(renderWindow ,attackers, defenders, currentPlayer, defender));
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
    }
}
