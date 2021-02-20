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
        internal static ConfigEntry<bool> TrashPickup;

        static Config()
        {
            genSettings = new ConfigFile(Path.Combine(BepInEx.Paths.ConfigPath, "PaddysMods.cfg"), true);
            WorkBenchRadiusEnabled = genSettings.Bind("Crafting", "Workbench radius enabled", true, "If you prefer another authors mod which modifies the workbench radius, disable this setting to allow cross compatability.");
            WorkbenchRadius = genSettings.Bind("Crafting", "Workbench radius", 40f, "The radius around the workbenches in which operation is permitted.");
            CartMassMultiplier = genSettings.Bind("QoL", "Cart Mass Multiplier", 0.125f, "The multiplier which scales the mass value of the cart and impacts upon its handling. < 1.00 for less mass, >1.00 for more mass.");
            PublicPlayerPosition = genSettings.Bind("QoL", "Public Player Position", true, "Sets the public map position for the player when joining the game.");
            TrashPickup = genSettings.Bind("QoL", "TrashPickup", true, "Setting to false disables pickups of common trash (boar/deer trophy, amber, greydwarf eye). Might add custom string if high demand.");
        }
    }
}
