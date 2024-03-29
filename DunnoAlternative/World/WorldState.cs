﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using DunnoAlternative.Battle;
using DunnoAlternative.Shared;
using DunnoAlternative.State;
using DunnoAlternative.World.UI;
using ErunaInput;
using ErunaUI;
using ErunaUI.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using static SFML.Window.Mouse;

namespace DunnoAlternative.World
{
    public class WorldState : IState
    {
        private readonly Tile[,] tiles;
        private readonly List<Player> players;
        private readonly WindowManager windowManager;
        private int currentPlayerIndex = 0;
        private Player currentPlayer;
        private readonly Control currentPlayerIndicator;
        private readonly TextLabel moneyIndicator;

        private ErunaUI.Window? buildWindow;
        private ErunaUI.Window? invasionWindow;
        private ErunaUI.Window? heroRecruitWindow;
        private ErunaUI.Window? battleSetupWindow;

        private BattleSetupUI? battleSetupUI;
        private HeroRecruitmentUI? heroRecruitmentUI;
        private readonly RenderWindow renderWindow;

        const int POPUP_WINDOW_WIDTH = 250;
        const int POPUP_BUTTON_HEIGHT = 80;

        private readonly Font font = new("Content/Fonts/KosugiMaru-Regular.ttf");

        Vector2i invadedTile;

        private readonly List<SquadType> squadTypes;
        private readonly List<HeroClass> heroClasses;

        private readonly StateManager stateManager;
        private readonly Camera camera; //TODO: dispose
        private readonly CameraControls controls; //TODO: dispose

        const int BASE_INCOME = 500;
        List<Hero> recruitableHeroes;

        const int GENERIC_HEROES_RECRUITABLE_EACH_TURN = 3;

        Player attacker;

        public WorldState(RenderWindow window, StateManager stateManager, List<Player> players, Tile[,] tiles)
        {
            camera = new Camera(window);
            controls = new CameraControls(window, camera);

            renderWindow = window;

            this.stateManager = stateManager;

            this.players = players;

            this.tiles = tiles;

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
                YRows = GridRow.GenerateRows(2),
                XRows = GridRow.GenerateRows(2),
                TrueHeight = 100,
                TrueWidth = 800,
                PosX = 0,
                PosY = 500,
            };

            demoGrid.Children.Add(new Cell(currentPlayerIndicator, new List<int> { 1 }, new List<int> { 0 }));
            demoGrid.Children.Add(new Cell(moneyIndicator, new List<int> { 1 }, new List<int> { 1 }));
            demoGrid.Children.Add(new Cell(btnRecruitSquad, new List<int> { 0 }, new List<int> { 0, 1 }));

            btnRecruitSquad.ClickEvent += CreateSquadRecruitWindow;

            currentPlayerIndicator.ClickEvent += () => 
            {
                if (currentPlayer.Type == PlayerType.human && !(windowManager.Windows.Contains(battleSetupWindow) && attacker == currentPlayer))
                {
                    EndTurn();
                }
            };

            var demoUI = new ErunaUI.Window
            {
                Child = demoGrid
            };
            demoUI.UpdateSizes();

            windowManager = new WindowManager(stateManager.inputManager);
            windowManager.AddWindow(demoUI);

            squadTypes = new List<SquadType>();

            foreach (var file in new DirectoryInfo("Content/SquadTypes").GetFiles())
            {
                squadTypes.Add(JObject.Parse(File.ReadAllText(file.FullName)).ToObject<SquadType>());
            };

            heroClasses = new List<HeroClass>();

            foreach (var file in new DirectoryInfo("Content/HeroClasses").GetFiles())
            {
                heroClasses.Add(JObject.Parse(File.ReadAllText(file.FullName)).ToObject<HeroClass>());
            };

            TurnStart();
        }

        public void Update(RenderWindow window)
        {
            windowManager.Update();
            battleSetupUI?.Update();

            if (currentPlayer.Type == PlayerType.CPU && !windowManager.Windows.Contains(battleSetupWindow))
            {
                EndTurn();
            }

            if (stateManager.inputManager.MouseButtonState[Mouse.Button.Left] == (ButtonState.Release, false) && !(windowManager.Windows.Contains(battleSetupWindow) && attacker == currentPlayer))
            {
                ClosePopupWindows();

                invadedTile = MousePosToTile(stateManager.inputManager.MousePos); 

                if (TileExists(invadedTile))
                {
                    if (tiles[invadedTile.X, invadedTile.Y].Owner == currentPlayer)
                    {
                        var buttons = new List<(string, Action)>
                        {
                            ("Upgrade Castle", ()=>{ }),
                        };

                        buildWindow = CreatePopupWindow(stateManager.inputManager.MousePos, buttons);
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

                                    battleSetupUI = new BattleSetupUI(battleSetupWindow, font, 400, 600, currentPlayer.Heroes.Where(x => x.Deployed == false).ToList(), tiles[invadedTile.X, invadedTile.Y].Owner);
                                    battleSetupUI.OnAttackerFinished += AttackerSetupFinished;
                                    battleSetupUI.OnSetupCanceled += ClosePopupWindows;

                                    battleSetupWindow.UpdateSizes();

                                    windowManager.AddWindow(battleSetupWindow);
                                }),
                            };

                            invasionWindow = CreatePopupWindow(stateManager.inputManager.MousePos, buttons);
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

                tiles[invadedTile.X, invadedTile.Y].ChangeOwner(attacker);

                CheckPlayerAlive(oldOwner);
            }

            stateManager.Pop();
        }


        private void TurnStart()
        {
            ClosePopupWindows();

            currentPlayer.Money += BASE_INCOME;

            foreach (var tile in tiles)
            {
                if (tile.Owner == currentPlayer)
                {
                    currentPlayer.Money += tile.Income;
                }
            }

            foreach( var hero in currentPlayer.Heroes)
            {
                hero.Deployed = false;
            }

            moneyIndicator.TextString = "$" + currentPlayer.Money;

            recruitableHeroes = new List<Hero>();

            for (int i = 0; i < GENERIC_HEROES_RECRUITABLE_EACH_TURN; i++)
            {
                recruitableHeroes.Add(
                    new Hero(
                        heroClasses.GetRandom(),
                        squadTypes.GetRandom().Texture,
                        new List<Squad> {
                            new Squad(squadTypes.GetRandom()),
                            new Squad(squadTypes.GetRandom())
                        }));
            }
        }

        private void CheckPlayerAlive(Player player)
        {
            foreach (var tile in tiles)
            {
                if (tile.Owner == player) break;
            }

            if (player.Type == PlayerType.human) return; //TODO remove this line

            player.Alive = false;
        }

        private void CreateSquadRecruitWindow()
        {
            if (windowManager.Windows.Contains(battleSetupWindow) && attacker == currentPlayer) return;

            ClosePopupWindows();

            heroRecruitWindow = new ErunaUI.Window();

            heroRecruitmentUI = new HeroRecruitmentUI(heroRecruitWindow, font, 400, 600, recruitableHeroes.ToList());
            heroRecruitmentUI.OnRecruit += Recruit;
            heroRecruitmentUI.OnClose += ClosePopupWindows;

            heroRecruitWindow.UpdateSizes();

            windowManager.AddWindow(heroRecruitWindow);
        }

        private void Recruit(Hero? hero)
        {
            ClosePopupWindows();

            if (hero != null)
            {
                int cost = hero.Squads.Sum(x => x.Type.Cost);

                if (currentPlayer.Money >= cost)
                {
                    currentPlayer.Money -= cost;
                    moneyIndicator.TextString = "$" + currentPlayer.Money;
                    currentPlayer.Heroes.Add(hero);
                    recruitableHeroes.Remove(hero);
                }
                else
                {
                    //todo not enough gold popup?
                }

            }

            CreateSquadRecruitWindow();
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

            if (currentPlayer.Type == PlayerType.CPU) CPUTurn();
        }

        private void CPUTurn()
        {
            for (int n = 0; n < 10; n++)
            {
                Recruit(recruitableHeroes.GetRandom());
            }

            List<Vector2i> possibleTargets = new();

            for (int x = 0; x < tiles.GetLength(0); x++)
            {
                for (int y = 0; y < tiles.GetLength(1); y++)
                {
                    var target = new Vector2i(x, y);

                    if (tiles[x, y].Owner != currentPlayer && CheckNeighbors(target))
                    {
                        possibleTargets.Add(target);
                    }
                }
            }

            if (possibleTargets.Count == 0) return; //Probably shouldn't happen

            invadedTile = possibleTargets.GetRandom();

            AttackerSetupFinished(CPUDeploy(currentPlayer), tiles[invadedTile.X, invadedTile.Y].Owner);
        }

        private Hero[,] CPUDeploy(Player player)
        {
            float targetDeploy = player.Heroes.Count / 2.0f;
            float fields = BattleSetupUI.MAX_DEPTH * BattleSetupUI.MAX_WIDTH;
            float deployChance = targetDeploy / fields;

            Hero[,] returnValue = new Hero[BattleSetupUI.MAX_DEPTH,BattleSetupUI.MAX_WIDTH];

            for(int x = 0; x < BattleSetupUI.MAX_DEPTH; x++)
            {
                for(int y = 0; y < BattleSetupUI.MAX_WIDTH; y++)
                {
                    var availableHeroes = player.Heroes.Where((x) => x.Deployed == false).ToList();

                    if (availableHeroes.Count == 0) break;

                    var nextHero = availableHeroes.GetRandom();

                    if(Global.random.NextSingle() <= deployChance)
                    {
                        returnValue[x, y] = nextHero;
                        nextHero.Deployed = true;
                    }
                }
            }

            return returnValue;
        }

        private void ClosePopupWindows()
        {
            CloseWindow(buildWindow);
            CloseWindow(invasionWindow);
            CloseWindow(heroRecruitWindow);
            CloseWindow(battleSetupWindow);

            if (battleSetupUI != null)
            {
                battleSetupUI.OnSetupCanceled -= ClosePopupWindows;
                battleSetupUI.OnAttackerFinished -= AttackerSetupFinished;
                battleSetupUI.OnSetupFinished -= DefenderSetupFinished;
            }

            if (heroRecruitmentUI != null)
            {
                heroRecruitmentUI.OnClose -= ClosePopupWindows;
                heroRecruitmentUI.OnRecruit -= Recruit;
            }
        }

        private void AttackerSetupFinished(Hero[,] attackers, Player defender)
        {
            attacker = currentPlayer;

            ClosePopupWindows();

            var empty = true;

            foreach (var attacker in attackers)
            {
                if (attacker != null) empty = false;
            }

            if (empty) return;

            if (defender.Type == PlayerType.human)
            {
                battleSetupWindow = new ErunaUI.Window();

                battleSetupUI = new BattleSetupUI(battleSetupWindow, font, 400, 600, defender.Heroes.Where(x => x.Deployed == false).ToList(), defender, attackers);
                battleSetupUI.OnSetupFinished += DefenderSetupFinished;
                battleSetupUI.OnSetupCanceled += ClosePopupWindows;

                battleSetupWindow.UpdateSizes();

                windowManager.AddWindow(battleSetupWindow);
            }
            else
            {
                DefenderSetupFinished(attackers, CPUDeploy(defender), defender);
            }
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
