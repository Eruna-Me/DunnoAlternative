using SFML.Graphics;
using SFML.System;

namespace ErunaUI.Text
{
    public class TextLabel : TextBase
    {
        public TextLabel(Font spriteFont) : base(spriteFont)
        {
        }

        public override void OnDraw(RenderWindow window)
        {
            base.OnDraw(window);

            if (string.IsNullOrEmpty(TextString))
                return;

            var texty = new SFML.Graphics.Text(TextString, spriteFont)
            {
                Position = new Vector2f(PosX, PosY)
            };

            window.Draw(texty);
            //spriteBatch.DrawStringAligned(spriteFont, TextString, new Vector2(PosX, PosY), Color, TextAlign, TextGravity, new Vector2(TrueWidth, TrueHeight), 0);
        }
    }
}
