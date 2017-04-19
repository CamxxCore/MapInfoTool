using System.Drawing;
using GTA.Math;
using GTA.Native;

namespace MapInfoTool.Helpers
{
    public static class GameHelper
    {
        /// <summary>
        /// Fade out screen (Since this function was removed with newer versions of Scripthookvdotnet)
        /// </summary>
        /// <param name="duration">The duration of the fade effect.</param>
        public static void FadeScreenOut(int duration)
        {
            Function.Call(Hash.DO_SCREEN_FADE_OUT, duration);
        }

        /// <summary>
        /// Fade in screen (Since this function was removed with newer versions of Scripthookvdotnet)
        /// </summary>
        /// <param name="duration">The duration of the fade effect.</param>
        public static void FadeScreenIn(int duration)
        {
            Function.Call(Hash.DO_SCREEN_FADE_IN, duration);
        }

        /// <summary>
        /// Draws a box relative to the given center position.
        /// </summary>
        /// <param name="center">The center point for the box.</param>
        /// <param name="min">The minimum offset.</param>
        /// <param name="max">The maximum offset.</param>
        /// <param name="color">Color of the box.</param>
        public static void DrawBox(Vector3 center, Vector3 min, Vector3 max, Color color)
        {
            Function.Call(Hash.DRAW_LINE, center.X + max.X, center.Y + max.Y, center.Z + max.Z, center.X + max.X, center.Y + max.Y, center.Z + min.Z, color.R, color.G, color.B, color.A);

            Function.Call(Hash.DRAW_LINE, center.X + max.X, center.Y - max.Y, center.Z + max.Z, center.X + max.X, center.Y - max.Y, center.Z + min.Z, color.R, color.G, color.B, color.A);

            Function.Call(Hash.DRAW_LINE, center.X - max.X, center.Y + max.Y, center.Z + max.Z, center.X - max.X, center.Y + max.Y, center.Z + min.Z, color.R, color.G, color.B, color.A);

            Function.Call(Hash.DRAW_LINE, center.X - max.X, center.Y - max.Y, center.Z + max.Z, center.X - max.X, center.Y - max.Y, center.Z + min.Z, color.R, color.G, color.B, color.A);
            
            Function.Call(Hash.DRAW_LINE, center.X - max.X, center.Y + max.Y, center.Z + max.Z, center.X - max.X, center.Y - max.Y, center.Z + max.Z, color.R, color.G, color.B, color.A);

            Function.Call(Hash.DRAW_LINE, center.X - max.X, center.Y + max.Y, center.Z + max.Z, center.X + max.X, center.Y + max.Y, center.Z + max.Z, color.R, color.G, color.B, color.A);

            Function.Call(Hash.DRAW_LINE, center.X + max.X, center.Y + max.Y, center.Z + max.Z, center.X + max.X, center.Y - max.Y, center.Z + max.Z, color.R, color.G, color.B, color.A);

            Function.Call(Hash.DRAW_LINE, center.X + max.X, center.Y - max.Y, center.Z + max.Z, center.X - max.X, center.Y - max.Y, center.Z + max.Z, color.R, color.G, color.B, color.A);
            
            Function.Call(Hash.DRAW_LINE, center.X - max.X, center.Y + max.Y, center.Z + min.Z, center.X - max.X, center.Y - max.Y, center.Z + min.Z, color.R, color.G, color.B, color.A);

            Function.Call(Hash.DRAW_LINE, center.X - max.X, center.Y + max.Y, center.Z + min.Z, center.X + max.X, center.Y + max.Y, center.Z + min.Z, color.R, color.G, color.B, color.A);

            Function.Call(Hash.DRAW_LINE, center.X + max.X, center.Y + max.Y, center.Z + min.Z, center.X + max.X, center.Y - max.Y, center.Z + min.Z, color.R, color.G, color.B, color.A);

            Function.Call(Hash.DRAW_LINE, center.X + max.X, center.Y - max.Y, center.Z + min.Z, center.X - max.X, center.Y - max.Y, center.Z + min.Z, color.R, color.G, color.B, color.A);
        }
    }
}
