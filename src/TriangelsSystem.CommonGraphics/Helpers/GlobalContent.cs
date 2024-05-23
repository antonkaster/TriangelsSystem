using Microsoft.Xna.Framework;
using TriangelsSystem.CommonGraphics.Primitive2d;

namespace TriangelsSystem.CommonGraphics.Helpers
{
    public static class GlobalContent
    {
        public static Primitive2dCircle Body { get; private set; }
        public static Primitive2dCircle BodySelected { get; private set; }
        public static Primitive2dLines FriendLine { get; private set; }
        public static Primitive2dLines TargetLine { get; private set; }
        public static Primitive2dLines FriendLineSelected { get; private set; }

        private static Color _bodyColor = Color.Purple;
        private static Color _bodyColorSelected = Color.Yellow;
        private static Color _friendLineColor = Color.LightGray;
        private static Color _friendLineColorSelected = Color.Yellow;
        private static Color _targetLineColor = Color.GreenYellow;

        public static void Init()
        {
            var radius = 20;
            Body = new Primitive2dCircle(radius, 1, _bodyColor, _bodyColor);
            BodySelected = new Primitive2dCircle(radius, 1, _bodyColorSelected, _bodyColorSelected);
            FriendLine = new Primitive2dLines(1, _friendLineColor);
            TargetLine = new Primitive2dLines(1, _targetLineColor);
            FriendLineSelected = new Primitive2dLines(2, _friendLineColorSelected);
        }
    }
}
