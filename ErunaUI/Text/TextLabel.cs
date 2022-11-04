//using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework;
//using ErunaCore.Extensions.Graphics;

//namespace ErunaUI.Text
//{
//    public class TextLabel : TextBase
//    {
//        public TextLabel(SpriteFont spriteFont) : base(spriteFont)
//        {
//        }

//        public override void OnDraw(SpriteBatch spriteBatch)
//        {
//            base.OnDraw(spriteBatch);

//            if (string.IsNullOrEmpty(TextString))
//                return;

//            spriteBatch.DrawStringAligned(spriteFont, TextString, new Vector2(PosX, PosY), Color, TextAlign, TextGravity, new Vector2(TrueWidth, TrueHeight), 0);
//        }
//    }
//}
