﻿using Mono.Cecil.Cil;
using MonoMod.Cil;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SkillsRework
{
    public static class Hooks
    {
        internal static void Init()
        {
            IL.Humanoid.BlockAttack += Humanoid_BlockAttack;
            On.Projectile.OnHit += Projectile_OnHit;
            On.Menu.Start += Menu_Start;

            //On.Game.Start += Game_Start;
            //On.MenuScene.Awake += MenuScene_Awake;
        }

        private static void Menu_Start(On.Menu.orig_Start orig, Menu self)
        {
            self.gameObject.AddComponent<NotificationHandler>();
            orig(self);
        }

        //private static void Menu_Start(On.Menu.orig_Start orig, Menu self)
        //{
        //    orig(self);
        //   // self.gameObject.AddComponent<NotificationHandler>();
        //    //var go = GameObject.Instantiate(ModAssets.SkillUp, self.m_root.transform);
        //    //go.transform.SetParent(self.transform);
        //    //var notify = go.AddComponent<SkillNotify>();
        //    //notify.Start();
        //    //notify.gameObject.SetActive(true);
            
        //    //go.SetActive(true);
        //    //Main.log.LogDebug($"{self.m_root.transform}");



        //    //var testGameObject = new GameObject();
        //    //var transform = testGameObject.AddComponent<UnityEngine.RectTransform>();
        //    //var image = testGameObject.AddComponent<UnityEngine.UI.Image>();
        //    //transform.pivot = new Vector2(0.5f, 0.5f);
        //    //transform.sizeDelta = new Vector2(1000, 1000);
        //    //transform.SetParent(self.m_root.transform);
        //    //Main.log.LogDebug($"{self.m_root.transform}");
        //}

        private static void Projectile_OnHit(On.Projectile.orig_OnHit orig, Projectile self, Collider collider, Vector3 hitPoint, bool water)
        {
            orig(self, collider, hitPoint, water);
            if (!water)
            {
                HitData hitData = new HitData
                {
                    m_hitCollider = collider,
                    m_damage = self.m_damage,
                    m_pushForce = self.m_attackForce,
                    m_backstabBonus = self.m_backstabBonus,
                    m_point = hitPoint,
                    m_dir = self.transform.forward,
                    m_statusEffect = self.m_statusEffect,
                    m_dodgeable = self.m_dodgeable,
                    m_blockable = self.m_blockable,
                    m_skill = self.m_skill
                };
                hitData.SetAttacker(self.m_owner);

                if (self.m_owner != null && self.m_owner.IsPlayer())
                {
                    var c = collider.GetComponent<Character>();
                    if (c != null && c.IsMonsterFaction())
                    {
                        var dmg = Mathf.Clamp(hitData.GetTotalDamage(), 1, c.GetMaxHealth()); //range: 5-100+
                        var num = dmg/20; //0.25-20
                        if (num < 1) num = 1;
                        var distance = Vector3.Distance(hitPoint, self.m_owner.transform.localPosition); //0-100+
                        var multi = 1 + (distance / 100);
                        num *= multi;
                        NotificationHandler.Instance.AddNotification($"+XP: {num:0.00}");
                        NotificationHandler.Instance.AddNotification($"Distance: {distance:0.0}m");

                        Main.log.LogDebug($"{(self.m_owner as Player).GetPlayerName()} hit {c.m_name} at {distance:0.0}m for {dmg}/{c.GetMaxHealth()} using a {multi:0.00} multiplier yeilding {num}XP to bows.");
                        //(self.m_owner as Player).RaiseSkill(Skills.SkillType.Bows, num);
                    }
                }
                   
            }
        }

        private static void Humanoid_BlockAttack(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(
                MoveType.After,
                zz => zz.MatchLdarg(2),
                zz => zz.MatchLdfld<Character>("m_staggerWhenBlocked"),
                zz => zz.MatchBrfalse(out _)
                );
            c.Index += 5;
            c.Emit(OpCodes.Ldarg_0);
            c.Emit(OpCodes.Ldloc, 5);
            c.EmitDelegate<Action<Character, float>>((chara, num) =>
            {
                num /= 10;
                Main.log.LogDebug($"Gave {chara.m_name} {num} XP for Parry: {num}");
                chara.RaiseSkill(Skills.SkillType.Blocking, num);
            });
        }
    }
}