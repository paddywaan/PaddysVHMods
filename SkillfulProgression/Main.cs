using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace SkillfulProgression
{
    [BepInDependency("Paddy_StackingNotifications")]
    [BepInPlugin(GUID, MODNAME, VERSION)]
    public class Main : BaseUnityPlugin
    {
        #region[Declarations]

        public const string
            MODNAME = "SkillfulProgression",
            AUTHOR = "Paddy",
            GUID = AUTHOR + "_" + MODNAME,
            VERSION = "1.0.0";
        internal readonly Assembly assembly;
        public readonly string modFolder;

        public static ManualLogSource log;

        #endregion

        public Main()
        {
            log = Logger;
            assembly = Assembly.GetExecutingAssembly();
            modFolder = Path.GetDirectoryName(assembly.Location);
        }

        public void Start()
        {
            Hooks.Init();
        }
    }
}
