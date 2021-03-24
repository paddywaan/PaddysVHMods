using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemDropDecayConfig
{
    public static class Config
    {
        private static ConfigFile genSettings;
        public static ConfigEntry<double> DecayTimer;

        static Config()
        {
            genSettings = new ConfigFile(Path.Combine(BepInEx.Paths.ConfigPath, Main.GUID + ".cfg"), true);
            DecayTimer = genSettings.Bind("SETTINGS", "Decay Timer", 3600d, "Time in seconds before an item decays. Only affects drops outside of base (crafting station/fire) and player radius.");
        }
    }
}