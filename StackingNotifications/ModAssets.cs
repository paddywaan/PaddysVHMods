using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace StackingNotifications
{
    public class ModAssets
    {
        public AssetBundle Assets;
        public GameObject SkillUp;
        private static ModAssets instance;
        public GameObject NotificationLayer;
        public static ModAssets Instance
        {
            get
            {
                if (instance == null) instance = new ModAssets();
                return instance;
            }
        
        }

        ModAssets()
        {
            LoadAssets();
        }

        private void LoadAssets()
        {
            Assets = AssetBundle.LoadFromMemory(Properties.Resources.skillup);
            SkillUp = Assets.LoadAsset<GameObject>("SkillUpFixed");
            //var notify = SkillUp.GetComponent<SkillNotify>();
            


            if (SkillUp == null) Main.log.LogError($"Asset loading failed for:{SkillUp}");
            NotificationLayer = Assets.LoadAsset<GameObject>("NotificationLayer");
        }
    }
}
