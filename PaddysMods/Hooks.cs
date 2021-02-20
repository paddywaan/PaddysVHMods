using Mono.Cecil.Cil;
using MonoMod.Cil;
using UnityEngine;

namespace PaddysMods
{
    public static class Hooks
    {

        internal static void Init()
        {
            if (Config.WorkBenchRadiusEnabled.Value)
            {
                IL.CraftingStation.HaveBuildStationInRange += CraftingStation_HaveBuildStationInRange;
                On.CircleProjector.CreateSegments += CircleProjector_CreateSegments;
            }
            On.Vagon.UpdateMass += Vagon_UpdateMass;
            On.InventoryGui.Update += InventoryGui_Update;
            On.Humanoid.Pickup += Humanoid_Pickup;
            On.Minimap.Start += Minimap_Start;
            if (Config.AutoShout.Value)
            {
                On.Chat.InputText += Chat_InputText;
            }
        }

        private static void Chat_InputText(On.Chat.orig_InputText orig, Chat self)
        {
            var text = self.m_input.text;
            if (text.StartsWith("\\")) orig(self);
            self.SendText(Talker.Type.Shout, text);
        }

        private static void Minimap_Start(On.Minimap.orig_Start orig, Minimap self)
        {
            orig(self);
            self.m_publicPosition.isOn = Config.PublicPlayerPosition.Value;
            ZNet.instance.SetPublicReferencePosition(self.m_publicPosition.isOn);
        }

        private static bool Humanoid_Pickup(On.Humanoid.orig_Pickup orig, Humanoid self, GameObject go)
        {
            if (Config.TrashPickup.Value) return orig(self, go);
            //Main.log.Log(BepInEx.Logging.LogLevel.Debug, $"{go.name}");
            ItemDrop component = go.GetComponent<ItemDrop>();
            if (component == null)
            {
                return false;
            }
            switch(go.name)
            {
                case "GreydwarfEye(Clone)": return false;
                case "Resin(Clone)": return false;
                case "GreylingTrophy(Clone)": return false;
                case "TophyBoar(Clone)": return false;
                case "TrophyDeer(Clone)": return false;
                default: /*Main.log.LogDebug($"Pickup Exclusions: {self.m_name} picked up {go.name}");*/ break;
            }
            return orig(self, go);
        }

        public static void CircleProjector_CreateSegments(On.CircleProjector.orig_CreateSegments orig, CircleProjector self)
        {
            self.m_radius = Config.WorkbenchRadius.Value;
            orig(self);
        }

        public static void CraftingStation_HaveBuildStationInRange(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(
                MoveType.Before,
                zz => zz.MatchLdloc(1),
                zz => zz.MatchLdfld<CraftingStation>("m_rangeBuild"),
                zz => zz.MatchStloc(2)
                );
            c.Next.OpCode = OpCodes.Nop;
            c.Next.Next.OpCode = OpCodes.Ldc_R4;
            c.Next.Next.Operand = Config.WorkbenchRadius.Value;
        }



        private static void InventoryGui_Update(On.InventoryGui.orig_Update orig, InventoryGui self)
        {
            orig(self);
            int alphaBegin = (int)KeyCode.Alpha0;
            int keypadBegin = (int)KeyCode.Keypad0;
            for(int i = 0; i<10; i++)
            {
                if(Input.GetKeyDown((KeyCode)alphaBegin+i) || Input.GetKeyDown((KeyCode)keypadBegin+i))
                {
                    self.m_splitSlider.value = i;
                    self.OnSplitSliderChanged(i);
                }
            }
        }

        public static void Chat_GetShoutWorldTexts(On.Chat.orig_GetShoutWorldTexts orig, Chat self, System.Collections.Generic.List<Chat.WorldTextInstance> texts)
        {
            foreach (Chat.WorldTextInstance worldTextInstance in self.m_worldTexts)
            {
                if (worldTextInstance.m_type == Talker.Type.Shout || worldTextInstance.m_type == Talker.Type.Normal)
                {
                    texts.Add(worldTextInstance);
                }
            }
        }
        public static void Vagon_UpdateMass(On.Vagon.orig_UpdateMass orig, Vagon self)
        {
            var x = self.m_itemWeightMassFactor;
            self.m_itemWeightMassFactor = Config.CartMassMultiplier.Value;
            orig(self);
        }
    }
}
