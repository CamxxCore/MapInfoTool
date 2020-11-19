using System;
using System.Reflection;
using GTA;
using GTA.Math;
using GTA.Native;

namespace MapInfoTool
{
    public static class Utility
    {
        /// <summary>
        /// Gets the current assembly name.
        /// </summary>
        /// <returns></returns>
        public static string GetAssemblyName()
        {
            var assemblyName = Assembly.GetExecutingAssembly().GetName();

            return assemblyName.Name;
        }

        /// <summary>
        /// Concatenates an array of strings with each member on a new line.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string[] GetLines(this string s)
        {
            return s.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
        }

        public static int HashKey(this string text)
        {
            return Game.GenerateHash(text);
        }

        public static Vector2 WorldToScreen(Vector3 pos)
        {
            var x2dp = new OutputArgument();
            var y2dp = new OutputArgument();
            Function.Call<bool>(Hash._WORLD3D_TO_SCREEN2D, pos.X, pos.Y, pos.Z, x2dp, y2dp);
            return new Vector2(x2dp.GetResult<float>(), y2dp.GetResult<float>());
        }
    }
}
