using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TriangelsSystem.CommonGraphics.Text;

public class PrimitiveText
{
    private SpriteFont _font;

    public PrimitiveText()
    {
    }

    public void Load(SpriteFont spriteFont)
    {
        _font = spriteFont;
    }

    public void DrawText(SpriteBatch spriteBatch, string text, Vector2 pos, float scale, Color color)
    {
        spriteBatch.DrawString(_font, text, pos, color, 0, new Vector2(), scale, SpriteEffects.None, 0);
    }
}
