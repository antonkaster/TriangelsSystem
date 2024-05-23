using Microsoft.Xna.Framework;
using TriangelsSystem.CommonGraphics.Constants;

namespace TriangelsSystem.CommonGraphics.Helpers;

public static class SpaceHelper
{
    private static void Swap<T>(ref T v1, ref T v2) { T v3 = v1; v1 = v2; v2 = v3; }

    private static Point _size = new();
    public static Point Size
    {
        get => _size;
        set
        {
            if (value == _size) return;
            _size = value;
        }
    }

    public static Vector2 Point1 => new Vector2(ScreenConsts.ScreenBound, ScreenConsts.ScreenBound);
    public static Vector2 Point2 => new Vector2(Size.X - ScreenConsts.ScreenBound, ScreenConsts.ScreenBound);
    public static Vector2 Point3 => new Vector2(Size.X - ScreenConsts.ScreenBound, Size.Y - ScreenConsts.ScreenBound);
    public static Vector2 Point4 => new Vector2(ScreenConsts.ScreenBound, Size.Y - ScreenConsts.ScreenBound);

    public static Rectangle GetBoundedScreen()
    {
        return new Rectangle(ScreenConsts.ScreenBound, ScreenConsts.ScreenBound, Size.X - ScreenConsts.ScreenBound * 2, Size.Y - ScreenConsts.ScreenBound * 2);
    }

    public static Point GetRandomScreenCoords()
    {
        return new Point(
            RandomHelper.Next(0, Size.X),
            RandomHelper.Next(0, Size.Y)
            );
    }

    public static Point GetRandomScreenCoords(int bound)
    {
        return new Point(
            RandomHelper.Next(bound, Size.X - bound),
            RandomHelper.Next(bound, Size.Y - bound)
            );
    }

    public static Vector2 CalculateNextPosition(Vector2 start, Vector2 target, int speed)
    {
        if (speed == 0)
            return start;

        int x0 = (int)start.X;
        int y0 = (int)start.Y;
        int x1 = (int)target.X;
        int y1 = (int)target.Y;

        var deErr = 10;

        if (Math.Abs(x0 - x1) <= deErr && Math.Abs(y0 - y1) <= deErr)
            return start;

        var swaped = false;

        if (Math.Abs(x1 - x0) < Math.Abs(y1 - y0))
        {
            Swap(ref x0, ref y0);
            Swap(ref x1, ref y1);
            swaped = true;
        }

        int dx = Math.Abs(x1 - x0);
        int dy = Math.Abs(y1 - y0);

        int error = dx / 2; // Здесь используется оптимизация с умножением на dx, чтобы избавиться от лишних дробей

        int xstep = (x0 < x1) ? 1 : -1;
        int ystep = (y0 < y1) ? 1 : -1; // Выбираем направление роста координаты y

        int y = y0;
        int x = x0;

        for (x = x0; Math.Abs(x1 - x) > 0; x += xstep)
        {
            if (speed <= 0)
                break;

            error -= dy;
            if (error < 0)
            {
                y += ystep;
                error += dx;
            }
            speed--;
        }

        return swaped ? new Vector2(y, x) : new Vector2(x, y);
    }

    public static (Vector2 p1, Vector2 p2)? GetTargetBorderPoints(Vector2 firstPos, Vector2 secondPos, Vector2 perpPoint1, Vector2 perpPoint2)
    {
        var center = GetBoundedScreen().Center.ToVector2();

        var i1 = GeometriaExtensions.LinesIntersection(perpPoint1, perpPoint2, Point1, Point2);
        var i2 = GeometriaExtensions.LinesIntersection(perpPoint1, perpPoint2, Point2, Point3);
        var i3 = GeometriaExtensions.LinesIntersection(perpPoint1, perpPoint2, Point3, Point4);
        var i4 = GeometriaExtensions.LinesIntersection(perpPoint1, perpPoint2, Point4, Point1);

        var pList = new List<Vector2>();

        if (i1.HasValue)
            pList.Add(i1.Value);
        if (i2.HasValue)
            pList.Add(i2.Value);
        if (i3.HasValue)
            pList.Add(i3.Value);
        if (i4.HasValue)
            pList.Add(i4.Value);

        if (pList.Count < 2)
            return null;

        if (pList.Count > 2)
        {
            pList = pList
                .Select(p => new { Point = p, Range = p.PowRange(center) })
                .OrderBy(p => p.Range)
                .Take(2)
                .Select(p => p.Point)
                .ToList();
        }

        var c1 = pList[0];
        var c2 = pList[1];
        return (pList[0], pList[1]);
    }
}
