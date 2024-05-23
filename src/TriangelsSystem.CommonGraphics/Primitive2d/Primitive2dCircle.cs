using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TriangelsSystem.CommonGraphics.Helpers;

namespace TriangelsSystem.CommonGraphics.Primitive2d;

public class Primitive2dCircle
{
    private readonly GraphicsDevice _graphicsDevice;

    private int _lineWidth = 1;
    private int _radius = -1;
    private Texture2D? _circleTexture = null;

    private Color _color = Color.Magenta;
    private Color? _fillColor;

    private float CurrentCircleRadius
    {
        get { return _circleTexture.Width / 2f; }
    }

    public Primitive2dCircle(int radius, int lineWidth, Color color, Color? fillColor)
    {
        _graphicsDevice = Globals.GraphicsDevice ?? throw new ArgumentNullException(nameof(Globals.GraphicsDevice));
        _color = color;
        _fillColor = fillColor;
        _lineWidth = lineWidth;
        _circleTexture = CreateCircleTexture(_graphicsDevice, radius);
    }

    public void DrawCircle(SpriteBatch spriteBatch, Vector2 center, int radius)
    {
        float scale = radius / CurrentCircleRadius;

        if (scale > 1.5f)
        {
            _circleTexture = CreateCircleTexture(_graphicsDevice, (int)(CurrentCircleRadius * 2));
            scale = radius / CurrentCircleRadius;
        }
        else if (scale < 0.5f)
        {
            _circleTexture = CreateCircleTexture(_graphicsDevice, (int)(CurrentCircleRadius / 2));
            scale = radius / CurrentCircleRadius;
        }
        else
        {
            _circleTexture = CreateCircleTexture(_graphicsDevice, (int)CurrentCircleRadius);
        }

        var origin = new Vector2(radius / scale, radius / scale);

        spriteBatch.Draw(_circleTexture, center, null, Color.White, 0, origin, scale, SpriteEffects.None, 0);
    }

    private Texture2D CreateCircleTexture(GraphicsDevice device, int radius)
    {
        if (_circleTexture != null && _radius == radius)
            return _circleTexture;

        _circleTexture?.Dispose();

        if (_lineWidth > radius)
            _lineWidth = radius;

        _radius = radius;
        _circleTexture = new Texture2D(device, (radius << 1) + 1, (radius << 1) + 1);

        var arrayWrap = new ArrayWrap<Color>(_circleTexture.Width, _circleTexture.Height);

        #region Алгоритм_Брезенхема

        int x0 = radius;
        int y0 = radius;

        int x = 0;
        int y = radius;
        int delta = 2 - radius << 1;
        while (y >= 0)
        {
            for (int i = 0; i < _lineWidth; i++)
            {
                arrayWrap[x0 + x - i, y0 + y - i] = _color;
                arrayWrap[x0 + x - i, y0 - y + i] = _color;
                arrayWrap[x0 - x + i, y0 + y - i] = _color;
                arrayWrap[x0 - x + i, y0 - y + i] = _color;
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

        if (_fillColor != null)
        {
            for (int dx = 0; dx <= radius; dx++)
            {
                for (int dy = 0; dy <= radius; dy++)
                {
                    if (arrayWrap[x0 + dx, y0 + dy] == _color)
                        break;
                    arrayWrap[x0 + dx, y0 + dy] = _fillColor.Value;
                    arrayWrap[x0 + dx, y0 - dy] = _fillColor.Value;
                    arrayWrap[x0 - dx, y0 + dy] = _fillColor.Value;
                    arrayWrap[x0 - dx, y0 - dy] = _fillColor.Value;

                }
            }
        }

        _circleTexture.SetData(arrayWrap.Source);

        return _circleTexture;
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
