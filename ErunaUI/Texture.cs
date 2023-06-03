using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Text;

namespace ErunaUI
{
    public class Texture : Control
    {
        public Sprite Sprite { get; set; }

        public Texture(SFML.Graphics.Texture texture)
        {
            Sprite = new Sprite(texture);
        }

        public Texture()
        {
            Sprite = new Sprite();
        }

        public override void OnDraw(RenderWindow window)
        {
            base.OnDraw(window);

            if(Sprite.Texture != null)
            {
                var scale = Math.Min((float)TrueWidth / Sprite.Texture.Size.X, (float)TrueHeight / Sprite.Texture.Size.Y);
                Sprite.Scale = new Vector2f(scale, scale);
                Sprite.Position = new Vector2f(PosX, PosY);
            }

            window.Draw(Sprite);
        }
    }
}
