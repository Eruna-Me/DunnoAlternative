using SFML.Graphics;
using SFML.System;
using System;

namespace ErunaUI
{
    public abstract class Control
    {
        public static int DefaultMargin = 0;
        public static int DefaultBorderThickness = 0;

        public int PosX { get; set; }
        public int PosY { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int TrueWidth { get; set; }
        public int TrueHeight { get; set; }
        public int BorderThickness { get; set; } = DefaultBorderThickness;
        public Color? BorderColor { get; set; }
        public int Margin { get; set; } = DefaultMargin;
        public Vector2f Size { get; set; }
        public Control? Parent { get; }
        public Color? Background { get; set; }

        //public event Action ClickEvent;

        public virtual void OnDraw(RenderWindow window)
        {
            var rectangle = new RectangleShape
            {
                OutlineThickness = BorderThickness,
                Position = new Vector2f(PosX + Margin, PosY + Margin),
                Size = new Vector2f(TrueWidth - 2* Margin, TrueHeight -2* Margin),
            };

            if (Background != null)
            {
                rectangle.FillColor = (Color)Background;
            }
            if (BorderColor != null)
            {
                rectangle.OutlineColor = (Color)BorderColor;
            }

            if (Background != null || BorderColor != null)
            {
                window.Draw(rectangle);
            }
        }

        //public bool IsMouseOver(Point mousePos)
        //{
        //    return (mousePos.X >= PosX
        //        && mousePos.X <= PosX + TrueWidth
        //        && mousePos.Y >= PosY
        //        && mousePos.Y <= PosY + TrueHeight);
        //}

        //public virtual bool OnClick(Point mousePos)
        //{
        //    if (IsMouseOver(mousePos) && ClickEvent != null)
        //    {
        //        ClickEvent();
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}
    }
}
