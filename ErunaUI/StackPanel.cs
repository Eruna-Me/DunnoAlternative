using SFML.Graphics;
using SFML.System;
using System.Collections.Generic;

namespace ErunaUI
{
    public class StackPanel : Control, IContainer
    {
        public List<Control> Children { get; set; }
        public bool HorizontalFlowDirection { get; set; }
        public bool InvertFlowDirection { get; set; }

        public void UpdateSizes()
        {
            int tempHeight = PosY;
            int tempWidth = PosX;
            foreach (Control child in Children)
            {
                child.PosY = tempHeight;
                child.PosX = tempWidth;

                if (HorizontalFlowDirection)
                {
                    child.TrueHeight = TrueHeight;                   
                    child.TrueWidth = child.Width;
                    tempWidth += child.TrueWidth;
                }
                else
                {
                    child.TrueHeight = child.Height;
                    child.TrueWidth = TrueWidth;
                    tempHeight += child.TrueHeight;
                }

                if (child is IContainer container)
                {
                    container.UpdateSizes();
                }
            }
        }

        public StackPanel()
        {
            Children = new List<Control>();
        }

        public override void OnDraw(RenderWindow window)
        {
            base.OnDraw(window);

            foreach (Control child in Children)
            {
                child.OnDraw(window);
            }
        }

        public override bool OnClick(Vector2i mousePos)
        {
            foreach (var child in Children)
            {
                if (child.OnClick(mousePos))
                {
                    return true;
                }
            }
            return base.OnClick(mousePos);
        }
    }
}
