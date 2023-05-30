using ErunaUI;
using ErunaUI.Text;
using SFML.Graphics;

namespace DunnoAlternative.World.UI
{
    internal class HeroInfoUI : Grid
    {
        Font font;

        Grid heroInfoGrid;
        Grid troopInfoGrid;
        Cell troopInfoGridCell;

        TextLabel heroNameLabel;
        //TextLabel heroClassLabel;
        //hero texture

        public HeroInfoUI(Font font) : base()
        {
            this.font = font;

            XRows = GridRow.GenerateRows(1);
            YRows = GridRow.GenerateRows(2);

            heroInfoGrid = new Grid();
            heroInfoGrid.XRows = GridRow.GenerateRows(2);
            heroInfoGrid.YRows = GridRow.GenerateRows(2);


            heroInfoGrid.XRows[0].Size = 1;
            heroInfoGrid.XRows[1].Size = 2;

            heroNameLabel = new TextLabel(font)
            {
                TextString = "",
                TextAlign = TextAlign.Left,
                TextGravity = TextGravity.Center,
                Color = Color.Black,
            };

            heroInfoGrid.Children.Add(new Cell(heroNameLabel, 1, 0));

            troopInfoGrid = new Grid();
            troopInfoGridCell = new Cell(troopInfoGrid, 0, 1);
            troopInfoGridCell.Control = troopInfoGrid;

            Children.Add(new Cell(heroInfoGrid, 0, 0));
            Children.Add(troopInfoGridCell);

            Background = Color.Magenta;
        }

        public void ChangeHero(Hero? hero)
        {
            if (hero == null)
            {
                heroNameLabel.TextString = string.Empty;
                troopInfoGrid = new Grid();
                troopInfoGridCell.Control = troopInfoGrid;
                return;
            }

            heroNameLabel.TextString = hero.Name;

            troopInfoGrid = new Grid
            {
                YRows = GridRow.GenerateRows(hero.Squads.Count),
                XRows = GridRow.GenerateRows(1)
            };

            troopInfoGridCell.Control = troopInfoGrid;

            for (int i = 0; i < hero.Squads.Count; i++)
            {
                troopInfoGrid.Children.Add(new Cell(new TroopInfoRow(font, hero.Squads[i].Type), 0, i));
            }

            UpdateSizes();
        }
    }

    internal class TroopInfoRow : Grid
    {
        public TroopInfoRow(Font font, SquadType squadType) : base()
        {
            XRows = GridRow.GenerateRows(2);
            YRows = GridRow.GenerateRows(1);

            var label = new TextLabel(font)
            {
                TextString = squadType.Name + ": " + squadType.Soldiers,
                TextAlign = TextAlign.Left,
                TextGravity = TextGravity.Center,
                Color = Color.Black,
            };

            XRows[0].Size = 1;
            XRows[1].Size = 3;

            var texture = new ErunaUI.Texture(squadType.Texture);

            Children.Add(new Cell(texture, 0, 0));
            Children.Add(new Cell(label, 1, 0));
        }
    }
}
