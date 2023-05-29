using ErunaUI.Text;
using ErunaUI;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using System.Reflection.Metadata;

namespace DunnoAlternative.World
{
    public class BattleSetupUI
    {
        public readonly Window mainWindow;

        private readonly Grid gameGrid;
        private readonly TextLabel okButton;
        //private SelectedCommanderSummary selectedCommanderSummary;

        private readonly Hero[,] playerFieldGrid;
        private readonly List<Hero> unassignedHeroes;
        private readonly StackPanel unassignedSquadsStackPanel;

        public int WindowHeight { private get; set; }
        public int WindowWidth { private get; set; }

        private readonly Font font;
        private Hero? selectedHero;

        const int MAX_WIDTH = 3;
        const int MAX_DEPTH = 1;


        public event Action<Hero[,], Player> OnAttackerFinished = delegate { };
        public event Action<Hero[,], Hero[,], Player> OnSetupFinished = delegate { };
        public event Action OnSetupCanceled = delegate { };

        public BattleSetupUI(Window window, Font font, int windowHeight, int windowWidth, List<Hero> heros, Player defender, Hero[,]? attackers = null)
        {      
            mainWindow = window;
            WindowWidth = windowWidth;
            WindowHeight = windowHeight;
            this.font = font;

            playerFieldGrid = new Hero[MAX_DEPTH, MAX_WIDTH];
            unassignedHeroes = new List<Hero>(heros);

            Grid fieldGrid = new()
            {
                PosX = 0,
                PosY = 0,
                TrueHeight = WindowHeight,
                TrueWidth = WindowWidth,
                Columns = GridRow.GenerateRows(3),
                Rows = GridRow.GenerateRows(3),
            };

            fieldGrid.Rows[0].Size = 40;
            fieldGrid.Rows[1].Size = 50;
            fieldGrid.Rows[2].Size = 10;

            fieldGrid.Columns[0].Size = 50;
            fieldGrid.Columns[1].Size = 25;
            fieldGrid.Columns[2].Size = 25;

            //selectedCommanderSummary = new SelectedCommanderSummary(font);

            gameGrid = new Grid
            {
                Columns = GridRow.GenerateRows(MAX_DEPTH),
                Rows = GridRow.GenerateRows(MAX_WIDTH),
            };

            okButton = new TextLabel(font)
            {
                TextString = "Done",
                TextAlign = TextAlign.Center,
                TextGravity = TextGravity.Center,
                Background = Color.Blue,
                Color = Color.Black,
            };
            var cancelButton = new TextLabel(font)
            {
                TextString = "Cancel",
                TextAlign = TextAlign.Center,
                TextGravity = TextGravity.Center,
                Background = Color.Blue,
                Color = Color.Black,
            };

            cancelButton.ClickEvent += CancelClicked;
            
            okButton.ClickEvent += () => OkClicked(attackers, defender);

            gameGrid.Background = Color.Green;

            unassignedSquadsStackPanel = new StackPanel
            {
                Background = Color.Magenta,
            };

            unassignedSquadsStackPanel.ClickEvent += () => ListClickEvent(null);

            for (int x = 0; x < MAX_DEPTH; x++)
            {
                for (int y = 0; y < MAX_WIDTH; y++)
                {
                    var commandor = new TextLabel(font)
                    {
                        Color = Color.White,
                        Background = Color.Green
                    };

                    if (selectedHero != null)
                    {
                        commandor.TextString = selectedHero.Name;
                    }
                    else
                    {
                        commandor.TextString = string.Empty;
                    }

                    int bsX = x;
                    int bsY = y;

                    commandor.ClickEvent += () => FieldClickEvent(playerFieldGrid[bsX, bsY], bsX, bsY);

                    gameGrid.Children.Add(new Cell(commandor, x, y));
                }
            }

            fieldGrid.Children.Add(new Cell(gameGrid, 0, 1, 1, 2));
            fieldGrid.Children.Add(new Cell(unassignedSquadsStackPanel, 1, 0, 2, 2));
            
            if(attackers == null)
            {
                fieldGrid.Children.Add(new Cell(okButton, 1, 2));
                fieldGrid.Children.Add(new Cell(cancelButton, 2, 2));
            }
            else
            {
                fieldGrid.Children.Add(new Cell(okButton, 1, 2, 2, 1));
            }
            //fieldGrid.Children.Add(new Cell(selectedCommanderSummary, 0, 0));

            mainWindow.Child = fieldGrid;
            mainWindow.UpdateSizes();

            Update();
        }

        private void FieldClickEvent(Hero hero, int x, int y)
        {
            playerFieldGrid[x, y] = selectedHero;

            selectedHero = hero;
        }

        private void ListClickEvent(Hero? squad)
        {
            if (selectedHero != null)
            {
                unassignedHeroes.Add(selectedHero);
            }

            selectedHero = squad;

            if (squad != null)
            {
                unassignedHeroes.Remove(squad);
            }
        }

        public void Update()
        {
            UpdateList();
            UpdateField();
            //selectedCommanderSummary.Update(selectedSquad);
        }

        private void UpdateList()
        {
            unassignedSquadsStackPanel.Children.Clear();

            foreach (Hero hero in unassignedHeroes)
            {
                Grid squadLabel = new()
                {
                    Height = 50,
                    Background = Color.Blue,
                    BorderColor = Color.White,
                    BorderThickness = 1,
                    Rows = GridRow.GenerateRows(1),
                    Columns = GridRow.GenerateRows(1)
                };
                squadLabel.Children.Add(new Cell(new TextLabel(font) { Color = Color.White, TextAlign = TextAlign.Center, TextGravity = TextGravity.Center, TextString = hero.Name }, 0, 0)); //parse on draw??

                squadLabel.ClickEvent += () => ListClickEvent(hero);

                unassignedSquadsStackPanel.Children.Add(squadLabel);
            }

            mainWindow.UpdateSizes();
        }

        private void UpdateField()
        {
            foreach (var child in gameGrid.Children)
            {
                var field = playerFieldGrid[child.Columns.First(), child.Rows.First()];
                var Item = (TextLabel)child.Control;

                Item.BorderColor = Color.Black;

                if (field == null)
                {
                    Item.Background = Color.Green;
                    Item.TextString = "";
                }
                else
                {
                    Item.Background = Color.Blue;
                    Item.TextString = field.Name;
                    Item.BorderColor = Color.Black;
                }
            }
            mainWindow.UpdateSizes();
        }

        private void OkClicked(Hero[,]? attackers, Player defender)
        {
            if(attackers == null)
            {
                OnAttackerFinished(playerFieldGrid, defender);
            }
            else
            {
                OnSetupFinished(attackers, playerFieldGrid, defender);
            }
        }

        private void CancelClicked()
        {
            OnSetupCanceled();
        }
    }
}
