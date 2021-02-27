using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx.Configuration;

namespace PaddysMods
{
    public static class Config
    {
        private static ConfigFile genSettings;
        internal static ConfigEntry<bool> WorkBenchRadiusEnabled;
        internal static ConfigEntry<float> WorkbenchRadius;
        internal static ConfigEntry<float> CartMassMultiplier;
        internal static ConfigEntry<bool> PublicPlayerPosition;
        internal static ConfigEntry<bool> TrashFilter;
        internal static ConfigEntry<bool> AutoShout;
        internal static ConfigEntry<string> TrashString;
        internal static List<string> FilterList;

        static Config()
        {
            genSettings = new ConfigFile(Path.Combine(BepInEx.Paths.ConfigPath, "PaddysMods.cfg"), true);
            WorkBenchRadiusEnabled = genSettings.Bind("Crafting", "Workbench radius enabled", true, "If you prefer another authors mod which modifies the workbench radius, disable this setting to allow cross compatibility.");
            WorkbenchRadius = genSettings.Bind("Crafting", "Workbench radius", 40f, "The radius around the workbenches in which operation is permitted.");
            CartMassMultiplier = genSettings.Bind("QoL", "Cart Mass Multiplier", 0.125f, "The multiplier which scales the mass value of the cart and impacts upon its handling. < 1.00 for less mass, >1.00 for more mass.");
            PublicPlayerPosition = genSettings.Bind("QoL", "Public Player Position", true, "Sets the public map position for the player when joining the game.");
            AutoShout = genSettings.Bind("QoL", "AutoShout", true, "Automatically converts localised chat messages to server-wide shouts.");
            TrashFilter = genSettings.Bind("QoL", "TrashFilter", false, "Setting to true will filter item pickups, excluding the configured trash items.");
            TrashString = genSettings.Bind("QoL", "TrashString", "GreydwarfEye;Resin;GreylingTrophy;TrophyBoar;TrophyDeer",
                "A semi-colon delimited list of pickup names which should be excluded from pickup. Set BepInEx.cfg `LogLevels = All`, or include `Debug` to output pickup item names to console.");
            FormatTrashStrings();
        }
        private static void FormatTrashStrings()
        {
            FilterList = TrashString.Value.Split(';').ToList();
            FilterList = FilterList.Select(x => x + "(Clone)").ToList();
            //foreach (var s in FilterList)
            //{
            //    s = s + "(Clone)";
            //}
        }
    }
}
