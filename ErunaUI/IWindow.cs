using SFML.Graphics;
using SFML.System;

namespace ErunaUI
{
    public interface IWindow
    {
        public int Layer { get; set; }

        public void OnDraw(RenderWindow window);

        public bool OnClick(Vector2i inputManager);

        public bool IsMouseOver(Vector2i mousePos);
    }
}
