using Microsoft.Xna.Framework.Graphics;

namespace TriangelsSystem.CommonGraphics.Helpers
{
    public static class Globals
    {
        public static bool DebugOn { get; set; } = false;
        public static bool ShowLinks { get; set; } = true;
        public static GraphicsDevice GraphicsDevice { get; private set; }

        public static void SetGraphipsDevice(GraphicsDevice graphicsDevice)
        {
            if (GraphicsDevice != null)
                return;

            GraphicsDevice = graphicsDevice;
        }
    }
}
