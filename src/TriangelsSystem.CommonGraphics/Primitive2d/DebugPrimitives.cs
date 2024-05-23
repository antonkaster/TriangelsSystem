using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TriangelsSystem.CommonGraphics.Helpers;

namespace TriangelsSystem.CommonGraphics.Primitive2d
{
    public static class DebugPrimitives
    {
        public static Dictionary<string, Texture2D> _textures = new();

        public static void DrawLine(this SpriteBatch spriteBatch, Vector2 startPos, Vector2 endPos, int lineWidth, Color color)
        {
            var lineTexture = CreateLineTexture(lineWidth, color);

            float length;
            //MathEx.Distance(ref startPos, ref endPos, out length);
            Vector2.Distance(ref startPos, ref endPos, out length);

            Vector2 direct;
            // MathEx.Direction(ref startPos, ref endPos, out direct);
            Vector2.Subtract(ref endPos, ref startPos, out direct);
            direct.Normalize();

            float rotation = (float)Math.Atan2(direct.Y, direct.X);
            //MathEx.AngleFromDirection(ref direct, out rotation);

            var rect = new Rectangle((int)startPos.X, (int)startPos.Y, (int)length, lineWidth);

            spriteBatch.Draw(lineTexture, rect, null, Color.White, rotation, Vector2.Zero, SpriteEffects.None, 0);
        }

        public static void DrawRectangle(this SpriteBatch spriteBatch, Rectangle rectangle, int lineWidth, Color color, bool fill)
        {
            var lineTexture = CreateLineTexture(lineWidth, color);

            if (rectangle.Width < 0)
            {
                rectangle.X += rectangle.Width;
                rectangle.Width = -rectangle.Width;
            }

            if (rectangle.Height < 0)
            {
                rectangle.Y += rectangle.Height;
                rectangle.Height = -rectangle.Height;
            }

            Rectangle tempRect = rectangle;

            if (fill)
            {
                //если тощина линии больше рисуемого прямоугольника будут корявости, пофиксим  
                if (rectangle.Width < lineWidth)
                    lineWidth = 1; //rectangle.Width; //хз как лучше, выглядит одинакого и фпс тот же
                if (rectangle.Height < lineWidth)
                    lineWidth = 1; // rectangle.Height;

                tempRect.Height = lineWidth;
                spriteBatch.Draw(lineTexture, tempRect, Color.White);

                tempRect.Height += rectangle.Height - lineWidth;
                spriteBatch.Draw(lineTexture, tempRect, Color.White);

                tempRect = rectangle;

                tempRect.Width = lineWidth;
                spriteBatch.Draw(lineTexture, tempRect, Color.White);

                tempRect.Width += rectangle.Width - lineWidth;
                spriteBatch.Draw(lineTexture, tempRect, Color.White);
            }
            else
            {
                tempRect.Height = lineWidth;
                spriteBatch.Draw(lineTexture, tempRect, Color.White);

                tempRect.Y += rectangle.Height - lineWidth;
                spriteBatch.Draw(lineTexture, tempRect, Color.White);

                tempRect = rectangle;

                tempRect.Width = lineWidth;
                spriteBatch.Draw(lineTexture, tempRect, Color.White);

                tempRect.X += rectangle.Width - lineWidth;
                spriteBatch.Draw(lineTexture, tempRect, Color.White);
            }
        }

        private static Texture2D CreateLineTexture(int width, Color color)
        {
            var cacheKey = $"{width}.{color}";
            if (_textures.TryGetValue(cacheKey, out var texture))
                return texture;

            var lineTexture = new Texture2D(Globals.GraphicsDevice, width, width);
            var colors = new Color[width * width];

            for (int i = 0; i < colors.Length; i++)
                colors[i] = color;

            lineTexture.SetData(colors);

            _textures[cacheKey] = lineTexture;

            return lineTexture;
        }

        public static void DrawCircle(this SpriteBatch spriteBatch, Vector2 center, int radius, int lineWidth, Color color, Color? fillColor)
        {
            var circleTexture = CreateCircleTexture(radius, lineWidth, color, fillColor);

            var origin = new Vector2(radius, radius);

            spriteBatch.Draw(circleTexture, center, null, Color.White, 0, origin, 1, SpriteEffects.None, 0);
        }

        private static Texture2D CreateCircleTexture(int radius, int lineWidth, Color color, Color? fillColor)
        {
            var cacheKey = $"{radius}.{lineWidth}.{color}.{fillColor}";
            if (_textures.TryGetValue(cacheKey, out var texture))
                return texture;

            if (lineWidth > radius)
                lineWidth = radius;

            var circleTexture = new Texture2D(Globals.GraphicsDevice, (radius << 1) + 1, (radius << 1) + 1);

            var arrayWrap = new ArrayWrap<Color>(circleTexture.Width, circleTexture.Height);

            #region Алгоритм_Брезенхема

            int x0 = radius;
            int y0 = radius;

            int x = 0;
            int y = radius;
            int delta = 2 - radius << 1;
            while (y >= 0)
            {
                for (int i = 0; i < lineWidth; i++)
                {
                    arrayWrap[x0 + x - i, y0 + y - i] = color;
                    arrayWrap[x0 + x - i, y0 - y + i] = color;
                    arrayWrap[x0 - x + i, y0 + y - i] = color;
                    arrayWrap[x0 - x + i, y0 - y + i] = color;
                }

                int error = (delta + y << 1) - 1;
                if (delta < 0 && error <= 0)
                {
                    ++x;
                    delta += (x << 1) + 1;
                    continue;
                }

                error = (delta - x << 1) - 1;
                if (delta > 0 && error > 0)
                {
                    --y;
                    delta += 1 - y << 1;
                    continue;
                }

                ++x;
                delta += x - y << 1;
                --y;
            }

            #endregion

            if (fillColor != null)
            {
                for (int dx = 0; dx <= radius; dx++)
                {
                    for (int dy = 0; dy <= radius; dy++)
                    {
                        if (arrayWrap[x0 + dx, y0 + dy] == color)
                            break;
                        arrayWrap[x0 + dx, y0 + dy] = fillColor.Value;
                        arrayWrap[x0 + dx, y0 - dy] = fillColor.Value;
                        arrayWrap[x0 - dx, y0 + dy] = fillColor.Value;
                        arrayWrap[x0 - dx, y0 - dy] = fillColor.Value;

                    }
                }
            }

            circleTexture.SetData(arrayWrap.Source);

            _textures[cacheKey] = circleTexture;

            return circleTexture;
        }

        #region Nested type: ArrayWrap

        private class ArrayWrap<T>
        {
            private readonly T[] _source;
            private readonly int _x;
            private readonly int _y;

            public ArrayWrap(int x, int y)
            {
                _x = x;
                _y = y;
                _source = new T[x * y];
            }

            public T[] Source
            {
                get { return _source; }
            }

            public T this[int i]
            {
                get { return _source[i]; }
                set { _source[i] = value; }
            }

            public T this[int x, int y]
            {
                get
                {
                    if (x > _x || y > _y) throw new IndexOutOfRangeException();
                    return _source[y * _x + x];
                }
                set
                {
                    if (x > _x || y > _y) throw new IndexOutOfRangeException();
                    _source[y * _x + x] = value;
                }
            }
        }

        #endregion

    }
}
