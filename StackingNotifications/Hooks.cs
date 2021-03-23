using Mono.Cecil.Cil;
using MonoMod.Cil;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StackingNotifications
{
    public static class Hooks
    {
        internal static void Init()
        {
            On.Menu.Start += Menu_Start;
        }

        private static void Menu_Start(On.Menu.orig_Start orig, Menu self)
        {
            orig(self);
            self.gameObject.AddComponent<NotificationHandler>();
        }
    }
}