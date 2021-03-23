using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.IO;
using System.Reflection;


namespace StackingNotifications
{
    [BepInPlugin(GUID, MODNAME, VERSION)]
    public class Main : BaseUnityPlugin
    {
        #region[Declarations]

        public const string
            MODNAME = "StackingNotifications",
            AUTHOR = "Paddy",
            GUID = AUTHOR + "_" + MODNAME,
            VERSION = "1.0.0";

        public static ManualLogSource log;
        internal readonly Assembly assembly;
        public readonly string modFolder;
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