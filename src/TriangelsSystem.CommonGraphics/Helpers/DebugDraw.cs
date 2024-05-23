using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TriangelsSystem.CommonGraphics.Primitive2d;

namespace TriangelsSystem.CommonGraphics.Helpers
{
    public static class DebugDraw
    {
        public static void DrawScreenBounds(this SpriteBatch spriteBatch)
        {
            var rect = SpaceHelper.GetBoundedScreen();
            spriteBatch.DrawRectangle(rect, 1, Color.Gray, false);
        }
    }
}
