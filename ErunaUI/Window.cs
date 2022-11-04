//using ErunaCore.Input;

using SFML.Graphics;

namespace ErunaUI
{
    public class Window : IWindow
    {
        public Control Child { get; set; }
        public int Layer { get; set; }

        public void OnDraw(RenderWindow window)
        {
            Child.OnDraw(window);
        }

        public void UpdateSizes()
        {
            if (Child is IContainer container)
            {
                container.UpdateSizes();
            }
        }

        //public bool OnClick(Point mousepos)
        //{
        //    return Child.OnClick(mousepos);
        //}
        //
        //public bool IsMouseOver(Point mousePos)
        //{
        //    return Child.IsMouseOver(mousePos);
        //}
    }
}
