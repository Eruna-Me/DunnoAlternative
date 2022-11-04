using SFML.Graphics;

namespace ErunaUI
{
    public interface IWindow
    {
        public int Layer { get; set; }

        public void OnDraw(RenderWindow window);

        //public bool OnClick(Point inputManager);

        //public bool IsMouseOver(Point mousePos);
    }
}
