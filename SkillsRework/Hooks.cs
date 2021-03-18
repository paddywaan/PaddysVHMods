using Mono.Cecil.Cil;
using MonoMod.Cil;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SkillsRework
{
    public static class States
    {
        public static bool PrimaryAttack = false;
        public static bool SecondaryAttack = false;
    }
    public static class Hooks
    {
        internal static void Init()
        {
            On.Menu.Start += Menu_Start;
            IL.Humanoid.BlockAttack += Humanoid_BlockAttack;
            On.Projectile.OnHit += Projectile_OnHit;
            On.Character.RPC_Damage += Character_RPC_Damage;
            On.Attack.DoMeleeAttack += Attack_DoMeleeAttack;
            On.Humanoid.StartAttack += Humanoid_StartAttack;
        }

        private static bool Humanoid_StartAttack(On.Humanoid.orig_StartAttack orig, Humanoid self, Character target, bool secondaryAttack)
        {
            if (self.IsPlayer())
            {
                States.PrimaryAttack = !secondaryAttack;
                States.SecondaryAttack = secondaryAttack;
            }
            return orig(self, target, secondaryAttack);
        }

        private static void Attack_DoMeleeAttack(On.Attack.orig_DoMeleeAttack orig, Attack self)
        {
            orig(self);
            if (self.m_character.IsPlayer())
            {
                States.PrimaryAttack = false;
                States.SecondaryAttack = false;
                //Main.log.LogDebug($"Returning states to false.");
            }
        }

        private static void Character_RPC_Damage(On.Character.orig_RPC_Damage orig, Character self, long sender, HitData hit)
        {

            bool alerted = false;
            if(self.m_baseAI) alerted = self.m_baseAI.m_alerted;
            var time = Time.time;
            var backStabTime = self.m_backstabTime;
            orig(self, sender, hit);
            var attacker = hit.GetAttacker();
            float XP = 0;
            if (attacker is Player)
            {
                //Main.log.LogDebug($"Primary: {States.PrimaryAttack}, Secondary: {States.SecondaryAttack} with {hit.m_skill}. Attackers {(attacker as Player).GetCurrentWeapon()} is a {(attacker as Player).GetCurrentWeapon().m_shared.m_skillType}. alerted?: {alerted}");
                switch (hit.m_skill)
                {
                    case Skills.SkillType.Knives:
                        //Main.log.LogDebug($"{self.m_baseAI},{!alerted}{hit.m_backstabBonus > 1f}{time - backStabTime > 300f}, Evaluates to: {(self.m_baseAI != null && !alerted && hit.m_backstabBonus > 1f && Time.time - self.m_backstabTime > 300f)}");
                        if (self.m_baseAI != null && !alerted && hit.m_backstabBonus > 1f && time - backStabTime > 300f)
                        {
                            XP = DamageToXP(self, hit);
                            if (self.IsDead()) XP *= 2;
                            NotificationHandler.Instance.AddNotification($"Knives: +{XP:0.00}XP");
                            attacker.RaiseSkill(hit.m_skill, XP);
                        }
                        break;
                    case Skills.SkillType.Swords:
                    case Skills.SkillType.Clubs:
                        if (States.SecondaryAttack)
                        {
                            XP = DamageToXP(self, hit);
                            NotificationHandler.Instance.AddNotification($"Clubs: +{XP:0.00}XP");
                            attacker.RaiseSkill(hit.m_skill, XP);
                        }
                        break;
                    default:
                        break;
                }
                Main.log.LogDebug($"{hit.GetAttacker().GetHoverName()} performed {hit.m_skill} against {self.GetHoverName()} hitting for {hit.GetTotalDamage()} yeilding {XP}.");
            }
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
                        float XP;
                        switch (hitData.m_skill)
                        {
                            case Skills.SkillType.Bows:
                            case Skills.SkillType.Spears:
                                XP = DamageToXP(c, hitData, hitData.GetTotalDamage());
                                var distance = Vector3.Distance(hitPoint, self.m_owner.transform.localPosition); //0-100+
                                var multi = 1 + (distance / 100);
                                XP *= multi;
                                if (hitData.m_skill == Skills.SkillType.Spears) XP *= 3;
                                NotificationHandler.Instance.AddNotification($"{hitData.m_skill}: +{XP:0.00}");
                                NotificationHandler.Instance.AddNotification($"Distance: {distance:0.0}m");

                                Main.log.LogDebug($"{(self.m_owner as Player).GetPlayerName()} hit {c.m_name} at {distance:0.0}m for {hitData.GetTotalDamage():0.00}/{c.GetMaxHealth()} using a {multi:0.00} multiplier yeilding {XP:0.00}XP to {hitData.m_skill}.");
                                //(self.m_owner as Player).RaiseSkill(Skills.SkillType.Bows, num);
                                break;
                        }
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
                num /= 10; //0.5-10
                Main.log.LogDebug($"Gave {chara.m_name} {num} XP for Parry: {num}");
                NotificationHandler.Instance.AddNotification($"Shields: +{num}XP");
                chara.RaiseSkill(Skills.SkillType.Blocking, num);
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="victim"></param>
        /// <param name="hit"></param>
        /// <param name="damage">Required to pass this param because the dev's totaldamage function only returns the totalled damagetypes, exclusive of any other modifiers.</param>
        /// <returns></returns>
        private static float DamageToXP(Character victim, HitData hit, float damageOverride = 0)
        {
            Main.log.LogDebug($"override: {damageOverride} vs dmg: {hit.GetTotalDamage()}");
            var dmg = Mathf.Clamp(damageOverride==0 ? hit.GetTotalDamage() : damageOverride, 1, victim.GetMaxHealth()); //range: 5-100+
            var num = dmg / 20; //0.25-20
            if (num < 1) num = 1;
            Main.log.LogDebug($"override: {damageOverride} vs dmg: {hit.GetTotalDamage()}, MaxHP: {victim.GetMaxHealth()}, post-Clamp: {dmg}");
            return num;
        }
    }
}