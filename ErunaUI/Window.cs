//using ErunaCore.Input;

using SFML.Graphics;
using SFML.System;
using System.Text;

namespace ErunaUI
{
    public class Window : IWindow
    {
        public Control? Child { get; set; }
        public int Layer { get; set; }

        public void OnDraw(RenderWindow window)
        {
            Child?.OnDraw(window);
        }

        public void UpdateSizes()
        {
            if (Child is IContainer container)
            {
                container.UpdateSizes();
            }
        }

        public bool OnClick(Vector2i mousepos)
        {
            if(Child == null) return false;

            return Child.OnClick(mousepos);
        }
        
        public bool IsMouseOver(Vector2i mousePos)
        {
            if (Child == null) return false;

            return Child.IsMouseOver(mousePos);
        }
    }
}
