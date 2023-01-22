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

        private readonly Squad[,] playerFieldGrid;
        private readonly List<Squad> unassignedSquads;
        private readonly StackPanel unassignedSquadsStackPanel;

        public int WindowHeight { private get; set; }
        public int WindowWidth { private get; set; }

        private readonly Font font;
        private Squad? selectedSquad;

        const int MAX_FORMATIONS = 3;
        const int SQUADS_PER_FORMATION = 2;


        public event Action<Squad[,], Player> OnAttackerFinished = delegate { };
        public event Action<Squad[,], Squad[,], Player> OnSetupFinished = delegate { };
        public event Action OnSetupCanceled = delegate { };

        public BattleSetupUI(Window window, Font font, int windowHeight, int windowWidth, List<Squad> squads, Player defender, Squad[,]? attackers = null)
        {
            mainWindow = window;
            WindowWidth = windowWidth;
            WindowHeight = windowHeight;
            this.font = font;

            playerFieldGrid = new Squad[SQUADS_PER_FORMATION, MAX_FORMATIONS];
            unassignedSquads = new List<Squad>(squads);

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
                Columns = GridRow.GenerateRows(SQUADS_PER_FORMATION),
                Rows = GridRow.GenerateRows(MAX_FORMATIONS),
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

            for (int x = 0; x < SQUADS_PER_FORMATION; x++)
            {
                for (int y = 0; y < MAX_FORMATIONS; y++)
                {
                    var commandor = new TextLabel(font)
                    {
                        Color = Color.White,
                        Background = Color.Green
                    };

                    if (selectedSquad != null)
                    {
                        commandor.TextString = selectedSquad.Name;
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

        private void OkClicked(Squad[,]? attackers, Player defender)
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
