using Microsoft.Xna.Framework;

namespace TriangelsSystem.CommonGraphics.Helpers
{
    public static class GeometriaExtensions
    {
        public static Vector2 PerpendicularClockwise(this Vector2 vector2)
        {
            return new Vector2(vector2.Y, -vector2.X);
        }

        public static Vector2 PerpendicularCounterClockwise(this Vector2 vector2)
        {
            return new Vector2(-vector2.Y, vector2.X);
        }

        public static (Vector2 v1, Vector2 v2) GetPerpendicular(this Vector2 point1, Vector2 point2)
        {
            var df = point1 - point2;

            var center = new Vector2(
                (point1.X + point2.X) / 2,
                (point1.Y + point2.Y) / 2);

            var r1 = df.PerpendicularClockwise() + center;
            var r2 = df.PerpendicularCounterClockwise() + center;

            return (r1, r2);
        }

        // For finite lines:
        public static Vector2 FindNearestPointOnLineOnFiniteLine(this Vector2 point, Vector2 start, Vector2 end)
        {
            //Get heading
            var direction = end - start;

            var length = direction.Length();
            direction.Normalize();

            //Do projection from the point but clamp it
            var lhs = point - start;
            var dotP = Vector2.Dot(lhs, direction);
            dotP = Math.Clamp(dotP, 0f, length);
            return start + direction * dotP;
        }

        // For infinite lines:
        public static Vector2 FindNearestPointOnLineOnInfiniteLine(this Vector2 point, Vector2 start, Vector2 end)
        {
            //Get heading
            var direction = end - start;
            direction.Normalize();

            //Do projection from the point
            var lhs = point - start;
            var dotP = Vector2.Dot(lhs, direction);

            return start + direction * dotP;
        }

        //public static bool InRange(int range, int x1, int y1, int x2, int y2)
        //{
        //    var dx = x2 - x1;
        //    var dy = y2 - y1;

        //    if (Math.Abs(dx) > range || Math.Abs(dy) > range)
        //        return false;

        //    return dx * dx + dy * dy < range * range;
        //}

        public static int PowRange(this Vector2 point1, Vector2 point2)
        {
            var dx = (int)(point2.X - point1.X);
            var dy = (int)(point2.Y - point1.Y);

            return dx * dx + dy * dy;
        }


        public static Vector2? LinesIntersection(Vector2 l1p1, Vector2 l1p2, Vector2 l2p1, Vector2 l2p2)
        {
            var a1 = l1p1.Y - l1p2.Y;
            var b1 = l1p2.X - l1p1.X;
            var c1 = l1p1.X * l1p2.Y - l1p1.Y * l1p2.X;

            var a2 = l2p1.Y - l2p2.Y;
            var b2 = l2p2.X - l2p1.X;
            var c2 = l2p1.X * l2p2.Y - l2p1.Y * l2p2.X;

            var u = b1 * c2 - b2 * c1;
            var v = a2 * c1 - a1 * c2;
            var w = a1 * b2 - a2 * b1;

            if (w == 0)
                return null;

            var x = u / w;
            var y = v / w;

            return new Vector2(x, y);
        }

        public static bool InRecatangle(this Vector2 point, Rectangle rectangle)
        {
            return point.X > rectangle.X && point.X < rectangle.Right
                && point.Y > rectangle.Y && point.Y < rectangle.Bottom;
        }

        public static bool IsIntersect(Vector2 p1, int radius1, Vector2 p2, int radius2)
        {
            var dx = p2.X - p1.X;
            var dy = p2.Y - p1.Y;
            var range = radius2 + radius1;

            if (Math.Abs(dx) > range || Math.Abs(dy) > range)
                return false;

            return dx * dx + dy * dy < range * range;
        }

        public static bool IsIntersect(Vector2 p1, int radius, Vector2 p2)
        {
            return IsIntersect(p1, radius, p2, 0);
        }
    }
}
