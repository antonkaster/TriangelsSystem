using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TriangelsSystem.CommonGraphics.Helpers;

namespace TriangelsSystem.CommonGraphics.Primitive2d;

public class Primitive2dLines
{
    private readonly GraphicsDevice _graphicsDevice;

    private Texture2D? _lineTexture = null;

    private Color _color = Color.Magenta;

    private int _lineWidth = 1;

    public Primitive2dLines(int lineWidth, Color lineColor)
    {
        _graphicsDevice = Globals.GraphicsDevice ?? throw new ArgumentNullException(nameof(Globals.GraphicsDevice));
        _lineWidth = lineWidth;
        _color = lineColor;
        _lineTexture = CreateLineTexture(_graphicsDevice, lineWidth, lineColor);
    }

    public void DrawLine(SpriteBatch spriteBatch, Vector2 startPos, Vector2 endPos)
    {
        float length;
        //MathEx.Distance(ref startPos, ref endPos, out length);
        Vector2.Distance(ref startPos, ref endPos, out length);

        Vector2 direct;
        // MathEx.Direction(ref startPos, ref endPos, out direct);
        Vector2.Subtract(ref endPos, ref startPos, out direct);
        direct.Normalize();

        float rotation = (float)Math.Atan2(direct.Y, direct.X);
        //MathEx.AngleFromDirection(ref direct, out rotation);

        var rect = new Rectangle((int)startPos.X, (int)startPos.Y, (int)length, _lineWidth);

        spriteBatch.Draw(_lineTexture, rect, null, Color.White, rotation, Vector2.Zero, SpriteEffects.None, 0);
    }

    public void DrawRectangle(SpriteBatch spriteBatch, Rectangle rectangle, bool fill)
    {
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
            if (rectangle.Width < _lineWidth)
                _lineWidth = 1; //rectangle.Width; //хз как лучше, выглядит одинакого и фпс тот же
            if (rectangle.Height < _lineWidth)
                _lineWidth = 1; // rectangle.Height;

            tempRect.Height = _lineWidth;
            spriteBatch.Draw(_lineTexture, tempRect, Color.White);

            tempRect.Height += rectangle.Height - _lineWidth;
            spriteBatch.Draw(_lineTexture, tempRect, Color.White);

            tempRect = rectangle;

            tempRect.Width = _lineWidth;
            spriteBatch.Draw(_lineTexture, tempRect, Color.White);

            tempRect.Width += rectangle.Width - _lineWidth;
            spriteBatch.Draw(_lineTexture, tempRect, Color.White);
        }
        else
        {
            tempRect.Height = _lineWidth;
            spriteBatch.Draw(_lineTexture, tempRect, Color.White);

            tempRect.Y += rectangle.Height - _lineWidth;
            spriteBatch.Draw(_lineTexture, tempRect, Color.White);

            tempRect = rectangle;

            tempRect.Width = _lineWidth;
            spriteBatch.Draw(_lineTexture, tempRect, Color.White);

            tempRect.X += rectangle.Width - _lineWidth;
            spriteBatch.Draw(_lineTexture, tempRect, Color.White);
        }
    }

    private static Texture2D CreateLineTexture(GraphicsDevice device, int width, Color color)
    {
        var lineTexture = new Texture2D(device, width, width);
        var colors = new Color[width * width];

        for (int i = 0; i < colors.Length; i++)
            colors[i] = color;

        lineTexture.SetData(colors);
        return lineTexture;
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

    //         public void DrawTriangle( SpriteBatch spriteBatch, Vector2 point1, Vector2 point2, Vector2 point3 )
    //         {
    //             DrawLine(spriteBatch, point1, point2);
    //             DrawLine(spriteBatch, point2, point3);
    //             DrawLine(spriteBatch, point3, point1);
    //         }
}
