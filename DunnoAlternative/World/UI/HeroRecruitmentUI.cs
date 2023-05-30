using ErunaUI;
using ErunaUI.Text;
using SFML.Graphics;
using System;

namespace DunnoAlternative.World.UI
{
    public class HeroRecruitmentUI
    {
        private readonly Window window;
        private readonly List<Hero> recruitableHeroes;
        private readonly Font font;
        private HeroInfoUI selectedHeroInfo;
        private Grid grid;
        private readonly TextLabel recruitButton;

        private Hero? _selectedHero;
        private Hero? SelectedHero
        {
            get => _selectedHero;
            set
            {
                _selectedHero = value;
                selectedHeroInfo.ChangeHero(value);
            }
        }

        private readonly StackPanel recruitableHeroesStackPanel;


        public event Action OnClose = delegate { };
        public event Action<Hero?> OnRecruit = delegate { };

        public HeroRecruitmentUI(Window window, Font font, int windowHeight, int windowWidth, List<Hero> recruitableHeroes) 
        {
            this.window = window;
            this.recruitableHeroes = recruitableHeroes;
            this.font = font;

            grid = new()
            {
                PosX = 10,
                PosY = 10,
                TrueHeight = windowHeight,
                TrueWidth = windowWidth,
                XRows = GridRow.GenerateRows(3),
                YRows = GridRow.GenerateRows(3),
            };

            grid.YRows[0].Size = 90;
            grid.YRows[1].Size = 10;

            grid.XRows[0].Size = 50;
            grid.XRows[1].Size = 50;

            selectedHeroInfo = new HeroInfoUI(font);

            recruitButton = new TextLabel(font)
            {
                TextString = "Select a Hero",
                TextAlign = TextAlign.Center,
                TextGravity = TextGravity.Center,
                Background = Color.Blue,
                Color = Color.Black,
            };

            var closeButton = new TextLabel(font)
            {
                TextString = "Close",
                TextAlign = TextAlign.Center,
                TextGravity = TextGravity.Center,
                Background = Color.Blue,
                Color = Color.Black,
            };
            closeButton.ClickEvent += () => OnClose();

            recruitButton.ClickEvent += () => RecruitButtonClicked();
            
            grid.Background = Color.Green;
           
            recruitableHeroesStackPanel = new StackPanel
            {
                Background = Color.Magenta,
            };
            recruitableHeroesStackPanel.ClickEvent += () => ListClickEvent(null);

            grid.Children.Add(new Cell(closeButton, 1, 1));

            grid.Children.Add(new Cell(recruitableHeroesStackPanel, 1, 0));

            grid.Children.Add(new Cell(recruitButton, 0, 1));

            grid.Children.Add(new Cell(selectedHeroInfo, 0, 0));

            window.Child = grid;
            window.UpdateSizes();

            Update();
        }

        private void ListClickEvent(Hero? hero)
        {
            if (SelectedHero != null)
            {
                recruitableHeroes.Add(SelectedHero);
                recruitButton.TextString = "Select a Hero";
            }

            SelectedHero = hero;

            if (hero != null)
            {
                recruitableHeroes.Remove(hero);
                recruitButton.TextString = hero.Name + " - $" + hero.Squads.Sum(x => x.Type.Cost);
            }
        }

        public void Update()
        {
            UpdateList();
        }

        private void UpdateList()
        {
            recruitableHeroesStackPanel.Children.Clear();

            foreach (Hero hero in recruitableHeroes)
            {
                Grid squadLabel = new()
                {
                    Height = 50,
                    Background = Color.Blue,
                    BorderColor = Color.White,
                    BorderThickness = 1,
                    YRows = GridRow.GenerateRows(1),
                    XRows = GridRow.GenerateRows(1)
                };
                squadLabel.Children.Add(new Cell(new TextLabel(font) { Color = Color.White, TextAlign = TextAlign.Center, TextGravity = TextGravity.Center, TextString = hero.Name + " - $" + hero.Squads.Sum(x => x.Type.Cost) }, 0, 0)); //parse on draw??

                squadLabel.ClickEvent += () => ListClickEvent(hero);

                recruitableHeroesStackPanel.Children.Add(squadLabel);
            }

            window.UpdateSizes();
        }

        private void RecruitButtonClicked()
        {
            OnRecruit(SelectedHero);
        }
    }
}
