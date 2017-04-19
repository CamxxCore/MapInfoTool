using System.Drawing;
using System.Windows.Forms;
using MapInfoTool.Config;

namespace MapInfoTool.ScriptBase
{
    /// <summary>
    /// User config values.
    /// </summary>
    public sealed class UserConfig
    {
        public static void LoadValues()
        {
            ShowObjects = IniHelper.GetConfigSetting("General", "ShowObjects", true);
            ShowBuildings = IniHelper.GetConfigSetting("General", "ShowBuildings", true);
            ShowNearbyList = IniHelper.GetConfigSetting("General", "ShowNearbyList", true);
            DrawAllObjects = IniHelper.GetConfigSetting("General", "DrawAllObjects", true);
            ObjectSearchRadius = IniHelper.GetConfigSetting("General", "ObjectSearchRadius", 10.0f);
            BuildingSearchRadius = IniHelper.GetConfigSetting("General", "BuildingSearchRadius", 10.0f);
            MaxBuildingsOnScreen = IniHelper.GetConfigSetting("General", "MaxBuildingsOnScreen", 35);
            MaxObjectsOnScreen = IniHelper.GetConfigSetting("General", "MaxObjectsOnScreen", 30);
            ObjectColour = IniHelper.GetConfigSetting("General", "ObjectColour", Color.DarkGoldenrod);
            BuildingColour = IniHelper.GetConfigSetting("General", "BuildingColour", Color.DarkRed);
            ActivationKey = IniHelper.GetConfigSetting("KeyBinds", "Activate", Keys.Y);
        }

        /// <summary>
        /// Whether to draw information over static map objects (default= true)
        /// </summary>
        public static bool ShowObjects { get; private set; }

        /// <summary>
        /// Whether to draw information over buildings (default= true)
        /// </summary>
        public static bool ShowBuildings { get; private set; }

        /// <summary>
        /// Whether to show the list of nearby objects (default= true)
        /// </summary>
        public static bool ShowNearbyList { get; private set; }

        /// <summary>
        /// Whether to draw information about all on-screen objects or just the one in-front of the camera.
        /// </summary>
        public static bool DrawAllObjects { get; private set; }

        /// <summary>
        /// Default search radius for objects (default= 10)
        /// </summary>
        public static float ObjectSearchRadius { get; private set; }

        /// <summary>
        /// Default search radius for buildings (default=10)
        /// </summary>
        public static float BuildingSearchRadius { get; private set; }

        /// <summary>
        /// Max buildings that can be drawn on the screen at once (default=35)
        /// </summary>
        public static int MaxBuildingsOnScreen { get; private set; }

        /// <summary>
        /// Max objects that can be drawn on the screen at once (default=30)
        /// </summary>
        public static int MaxObjectsOnScreen { get; private set; }

        /// <summary>
        /// Background colour of objects.
        /// </summary>
        public static Color ObjectColour { get; private set; }

        /// <summary>
        /// Background colour of buildings.
        /// </summary>
        public static Color BuildingColour { get; private set; }

        /// <summary>
        /// Activation key.
        /// </summary>
        public static Keys ActivationKey { get; private set; }
    }
}
