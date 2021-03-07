using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Reflection;

namespace SkillsRework
{
    [BepInPlugin(GUID, MODNAME, VERSION)]
    public class Main : BaseUnityPlugin
    {
        #region[Declarations]

        public const string
            MODNAME = "SkillsRework",
            AUTHOR = "Paddy",
            GUID = AUTHOR + "_" + MODNAME,
            VERSION = "1.0.0";

        public static ManualLogSource log;
        internal readonly Harmony harmony;
        internal readonly Assembly assembly;
        public readonly string modFolder;

        #endregion
        public static GameObject SkillUp;
        AssetBundle SkillAB;

        public Main()
        {
            log = Logger;
            harmony = new Harmony(GUID);
            assembly = Assembly.GetExecutingAssembly();
            modFolder = Path.GetDirectoryName(assembly.Location);
        }

        public void Start()
        {
            Hooks.Init();

            //String SkillPrefix = String.Format("@{0}+{1}", "SkillUp", "skillup");
            //var skillUpPath = "Assets/Scenes/SkillUp";
            //AssetBundle skillAB = LoadAssetBundleResourcesProvider(SkillPrefix, Properties.Resources.skillup);
            //skillAB.LoadAsset<GameObject>(skillUpPath);
            //Main.log.LogDebug($"{SkillPrefix + ":" + skillUpPath + ".prefab"}");
            //SkillUp = Resources.Load<GameObject>(SkillPrefix + ":" + skillUpPath + ".prefab");
            //if (SkillUp == null) Main.log.LogError($"Asset loading failed for:{SkillPrefix + ":" + skillUpPath + ".prefab"}");
            var assets = AssetBundle.LoadFromMemory(Properties.Resources.skillup);
            SkillUp = assets.LoadAsset<GameObject>("SkillUpFixed");
            if (SkillUp == null) Main.log.LogError($"Asset loading failed for:{SkillUp}");
            
            //SkillUp.gameObject.transform.parent = Menu.m_instance.transform;
        }

        public static AssetBundle LoadAssetBundleResourcesProvider(String prefix, Byte[] resourceBytes)
        {
            //Check to make sure that the byte array supplied is not null, and throw an appropriate exception if they are.
            if (resourceBytes == null) throw new ArgumentNullException(nameof(resourceBytes));
            if (String.IsNullOrEmpty(prefix) || !prefix.StartsWith("@")) throw new ArgumentException("Invalid prefix format", nameof(prefix));

            //Actually load the bundle with a Unity function.
            var bundle = AssetBundle.LoadFromMemory(resourceBytes);
            if (bundle == null) throw new NullReferenceException(String.Format("{0} did not resolve to an assetbundle.", nameof(resourceBytes)));

            // Create and register the provider.
            var provider = new AssetBundleResourcesProvider(prefix, bundle);
            //ResourcesAPI.AddProvider(provider);

            return bundle;
        }

        public static void Update()
        {

        }

    }
}
