using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Text;

namespace ErunaUI
{
    public class Texture : Control
    {
        private readonly Sprite sprite;

        public Texture(SFML.Graphics.Texture texture)
        {
            sprite = new Sprite(texture);
        }

        public override void OnDraw(RenderWindow window)
        {
            base.OnDraw(window);

            sprite.Position = new Vector2f(PosX, PosY);

            window.Draw(sprite);
        }
    }
}
