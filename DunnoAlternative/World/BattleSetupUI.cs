using ErunaUI.Text;
using ErunaUI;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;

namespace DunnoAlternative.World
{
    public class BattleSetupUI
    {
        public readonly Window mainWindow;
        //private readonly WindowManager windowManager;

        private Grid gameGrid;
        private TextLabel okButton;
        //private SelectedCommanderSummary selectedCommanderSummary;

        private readonly Squad[,] playerFieldGrid;
        private readonly List<Squad> unassignedSquads;
        private StackPanel unassignedSquadsStackPanel;

        public int WindowHeight { private get; set; }
        public int WindowWidth { private get; set; }

        private readonly Font font;
        private Squad? selectedSquad;
        //public event Action<Squad[,]> OnSetupFinished = delegate { };

        public BattleSetupUI(Window window, Font font, int windowHeight, int windowWidth, List<Squad> squads)
        {
            //this.windowManager = windowManager;
            mainWindow = window;
            WindowWidth = windowWidth;
            WindowHeight = windowHeight;
            this.font = font;

            //windowManager.ClearWindows();

            //windowManager.AddWindow(mainWindow);

            playerFieldGrid = new Squad[2, 3];
            unassignedSquads = squads;

            InitUi();

            Update();
        }

        private void InitUi()
        {
            Grid fieldGrid = new()
            {
                PosX = 0,
                PosY = 0,
                TrueHeight = WindowHeight,
                TrueWidth = WindowWidth,
                Columns = GridRow.GenerateRows(2),
                Rows = GridRow.GenerateRows(3),
            };

            fieldGrid.Rows[0].Size = 40;
            fieldGrid.Rows[1].Size = 50;
            fieldGrid.Rows[2].Size = 10;

            //selectedCommanderSummary = new SelectedCommanderSummary(font);

            gameGrid = new Grid
            {
                //Columns = GridRow.GenerateRows(Globals.fieldWidth / 2),
                //Rows = GridRow.GenerateRows(Globals.fieldHeight),
            };

            okButton = new TextLabel(font)
            {
                TextString = "Done",
                TextAlign = TextAlign.Center,
                TextGravity = TextGravity.Center,
                Background = Color.Blue,
                Color = Color.Black,
            };

            okButton.ClickEvent += OkClicked;

            gameGrid.Background = Color.Green;

            unassignedSquadsStackPanel = new StackPanel
            {
                Background = Color.Magenta,
            };

            unassignedSquadsStackPanel.ClickEvent += () => ListClickEvent(null);

            //for (int x = 0; x < Globals.fieldWidth / 2; x++)
            //{
            //    for (int y = 0; y < Globals.fieldHeight; y++)
            //    {
            //        var commandor = new Square(font);
            //        int bsX = x;
            //        int bsY = y;
            //
            //        commandor.ClickEvent += () => FieldClickEvent(playerFieldGrid[bsX, bsY], bsX, bsY);
            //
            //        gameGrid.Children.Add(new Cell(commandor, x, y));
            //    }
            //}

            fieldGrid.Children.Add(new Cell(gameGrid, 0, 1, 1, 2));
            fieldGrid.Children.Add(new Cell(unassignedSquadsStackPanel, 1, 0, 1, 2));
            fieldGrid.Children.Add(new Cell(okButton, 1, 2));
            //fieldGrid.Children.Add(new Cell(selectedCommanderSummary, 0, 0));

            mainWindow.Child = fieldGrid;
            mainWindow.UpdateSizes();
        }

        private void FieldClickEvent(Squad squad, int x, int y)
        {
            playerFieldGrid[x, y] = selectedSquad;

            selectedSquad = squad;
        }

        private void ListClickEvent(Squad? squad)
        {
            if (selectedSquad != null)
            {
                unassignedSquads.Add(selectedSquad);
            }

            selectedSquad = squad;

            if (squad != null)
            {
                unassignedSquads.Remove(squad);
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

            foreach (Squad squad in unassignedSquads)
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
                squadLabel.Children.Add(new Cell(new TextLabel(font) { Color = Color.White, TextAlign = TextAlign.Center, TextGravity = TextGravity.Center, TextString = squad.Name }, 0, 0)); //parse on draw??

                squadLabel.ClickEvent += () => ListClickEvent(squad);

                unassignedSquadsStackPanel.Children.Add(squadLabel);
            }

            mainWindow.UpdateSizes();
        }

        private void UpdateField()
        {
            foreach (var child in gameGrid.Children)
            {
                var field = playerFieldGrid[child.Columns.First(), child.Rows.First()];
                var Grid = (Grid)child.Control;
                var Name = (TextLabel)Grid.Children.Single(x => x.Rows.Single() == 0 && x.Columns.Single() == 1).Control;
                ///var HP = (TextBlock)Grid.Children.Single(x => x.Rows.Single() == 1 && x.Columns.Single() == 1).Control;
                //var Status = (TextBlock)Grid.Children.Single(x => x.Rows.Single() == 1 && x.Columns.Single() == 0).Control;

                Grid.BorderColor = Color.Black;

                //Status.TextString = "";
                //Status.Background = Color.Transparent;

                if (field == null)
                {
                    Grid.Background = Color.Green;
                    Name.TextString = "";
                    //HP.TextString = "";
                }
                else
                {
                    Grid.Background = Color.Blue;
                    Name.TextString = field.Name;
                    //HP.TextString = field.HP + "/" + field.Role.MaxHP;
                    Grid.BorderColor = Color.Black;
                }
            }
            mainWindow.UpdateSizes();
        }

        private void OkClicked()
        {
            //OnSetupFinished(playerFieldGrid);
        }
    }
}
