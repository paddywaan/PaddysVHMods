using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillfulProgression
{
    public static class Config
    {
        private readonly static ConfigFile genSettings;

        static Config()
        {
            genSettings = new ConfigFile(Path.Combine(BepInEx.Paths.ConfigPath, Main.MODNAME + ".cfg"), true);
        }
    }
}
