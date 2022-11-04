using SFML.Graphics;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ErunaUI
{
    public class Cell
    {
        public List<int> Rows { get; set; }
        public List<int> Columns { get; set; }
        public Control Control { get; set; }

        public Cell(Control control, List<int> columns, List<int> rows)
        {
            Control = control;
            Columns = columns;// ?? new List<int> { 0 };
            Rows = rows;// ?? new List<int> { 0 };
        }

        /// <param name="width">Number of columns</param>
        /// <param name="height">Number of rows</param>
        public Cell(Control control, int row, int column, int width = 1, int height = 1) : 
            this(
                control,
                Enumerable.Range(row, width).ToList(),
                Enumerable.Range(column, height).ToList()
                )
        {
            
        }
    }

    public class GridRow
    {
        public int TrueSize
        {
            get; set;
        }

        public int Size { get; set; }
        public bool IsRelative { get; set; }
        public int Position { get; set; }

        public GridRow(int size = 1, bool isRelative = true)
        {
            Size = size;
            IsRelative = isRelative;
        }

        public static List<GridRow> GenerateRows(int amount) => Enumerable.Range(0, amount).Select(n => new GridRow()).ToList();
    }

    public class Grid : Control, IContainer
    {
        public List<GridRow> Rows { get; set; }
        public List<GridRow> Columns { get; set; }
        public List<Cell> Children { get; set; }

        public Grid()
        {
            Rows = new List<GridRow>();
            Columns = new List<GridRow>();
            Children = new List<Cell>();
        }

        public override void OnDraw(RenderWindow window)
        {
            base.OnDraw(window);

            foreach (Control child in Children.Select(x => x.Control))
            {
                child.OnDraw(window);
            }
        }

        public void UpdateSizes()
        {
            UpdateRowSizes(Rows, TrueHeight, PosY);
            UpdateRowSizes(Columns, TrueWidth, PosX);

            foreach (Cell child in Children)
            {
                int tempHeight = 0;
                foreach (int row in child.Rows)
                {
                    tempHeight += Rows[row].TrueSize;
                }
                child.Control.TrueHeight = tempHeight;
                child.Control.PosY = Rows[child.Rows.First()].Position;

                int tempWidth = 0;

                foreach (int column in child.Columns)
                {
                    tempWidth += Columns[column].TrueSize;
                }
                child.Control.TrueWidth = tempWidth;
                child.Control.PosX = Columns[child.Columns.First()].Position;

                if (child.Control is IContainer container)
                {
                    container.UpdateSizes();
                }
            }
        }

        public void UpdateRowSizes(List<GridRow> rows, int size, int position)
        {
            int relativeRowSizesTotal = RelativeRowsTotalRelativeSize(rows);
            int tempHeight = relativeRowSizesTotal;
            int tempTrueHeight = position;

            float nonAbsoluteSizeTotal = size - rows.Where(x => !x.IsRelative).Sum(x => x.Size);

            foreach (GridRow row in rows)
            {
                if (row.IsRelative)
                {
                    row.Position = tempTrueHeight;
                    row.TrueSize = (int)(row.Size / (float)relativeRowSizesTotal * nonAbsoluteSizeTotal);
                    tempHeight -= row.Size;
                    tempTrueHeight += row.TrueSize;
                }
                else
                {
                    row.Position = tempTrueHeight;
                    row.TrueSize = row.Size;
                    tempTrueHeight += row.Size;
                }
            }
        }

        public int RelativeRowsTotalRelativeSize(List<GridRow> rows)
        {
            return (rows.Where(x => x.IsRelative).Sum(x => x.Size));
        }

        //public override bool OnClick(Point mousePos)
        //{
        //    foreach(var child in Children)
        //    {
        //        if (child.Control.OnClick(mousePos))
        //        {
        //            return true;
        //        }
        //    }
        //    return base.OnClick(mousePos);
        //}
    }
}
