using System;
using System.Reflection;

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
    }
}
