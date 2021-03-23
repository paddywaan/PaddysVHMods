using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using UnityEngine;
using StackingNotifications;

namespace SkillfulProgression
{
    public static class States
    {
        public static bool PrimaryAttack = false;
        public static bool SecondaryAttack = false;
        public static int ChainCount = 0;
    }
    public static class Hooks
    {
        private const string highlightColour = "#155c28";
        internal static void Init()
        {
            IL.Humanoid.BlockAttack += Humanoid_BlockAttack;
            On.Projectile.OnHit += Projectile_OnHit;
            On.Character.RPC_Damage += Character_RPC_Damage;
            On.Attack.DoMeleeAttack += Attack_DoMeleeAttack;
            On.Humanoid.StartAttack += Humanoid_StartAttack;
            On.Attack.Start += Attack_Start;
            On.Character.Awake += CharacterOnAwake;
        }

        private static void CharacterOnAwake(On.Character.orig_Awake orig, Character self)
        {
            orig(self);
            self.gameObject.AddComponent<SkillTracker>();
        }

        private static bool Attack_Start(On.Attack.orig_Start orig, Attack self, Humanoid character, Rigidbody body, ZSyncAnimation zanim, CharacterAnimEvent animEvent, VisEquipment visEquipment, ItemDrop.ItemData weapon, Attack previousAttack, float timeSinceLastAttack, float attackDrawPercentage)
        {
            var ret = orig(self, character, body, zanim, animEvent, visEquipment, weapon, previousAttack, timeSinceLastAttack, attackDrawPercentage);
            if (IsLocalPlayer(self.m_character)) States.ChainCount = self.m_currentAttackCainLevel;
            var skillTracker = character.gameObject.GetComponent<SkillTracker>();
            if (skillTracker) skillTracker.LastAttackStart = Time.time;
            return ret;
        }

        private static bool Humanoid_StartAttack(On.Humanoid.orig_StartAttack orig, Humanoid self, Character target, bool secondaryAttack)
        {
            if (IsLocalPlayer(self))
            {
                States.PrimaryAttack = !secondaryAttack;
                States.SecondaryAttack = secondaryAttack;
            }
            return orig(self, target, secondaryAttack);
        }

        private static void Attack_DoMeleeAttack(On.Attack.orig_DoMeleeAttack orig, Attack self)
        {
            orig(self);
            if (IsLocalPlayer(self.m_character))
            {
                
                States.PrimaryAttack = false;
                States.SecondaryAttack = false;
                //Main.log.LogDebug($"Returning states to false.");
            }
        }

        private static void Character_RPC_Damage(On.Character.orig_RPC_Damage orig, Character self, long sender, HitData hit)
        {
            var skillTracker = self.gameObject.GetComponent<SkillTracker>();
            if (skillTracker) skillTracker.LastHitTaken = Time.time;
            
            bool alerted = false;
            if(self.m_baseAI) alerted = self.m_baseAI.m_alerted;
            var isBackstab = hit.m_backstabBonus > 1f && Time.time - self.m_backstabTime > 300f;
            orig(self, sender, hit);
            var attacker = hit.GetAttacker() as Player;
            float XP = 0;
            if (attacker)
            {
                //Main.log.LogDebug($"Primary: {States.PrimaryAttack}, Secondary: {States.SecondaryAttack} with {hit.m_skill}. Attackers {(attacker as Player).GetCurrentWeapon()} is a {(attacker as Player).GetCurrentWeapon().m_shared.m_skillType}. alerted?: {alerted}");
                switch (hit.m_skill)
                {
                    case Skills.SkillType.Knives:
                        //Main.log.LogDebug($"{self.m_baseAI},{!alerted}{hit.m_backstabBonus > 1f}{time - backStabTime > 300f}, Evaluates to: {(self.m_baseAI != null && !alerted && hit.m_backstabBonus > 1f && Time.time - self.m_backstabTime > 300f)}");
                        if (self.m_baseAI != null && !alerted && isBackstab)
                        {
                            XP = DamageToXP(self, hit);
                            if (self.IsDead()) XP *= 2;
                            NotificationHandler.Instance.AddNotification($"Knives: <color={highlightColour}>+{XP:0.00}</color>XP");
                            attacker.RaiseSkill(hit.m_skill, XP);
                        }
                        break;
                    case Skills.SkillType.Swords:
                    case Skills.SkillType.Clubs:
                        if (States.SecondaryAttack)
                        {
                            XP = DamageToXP(self, hit);
                            NotificationHandler.Instance.AddNotification($"Clubs: <color={highlightColour}>+{XP:0.00}</color>XP");
                            attacker.RaiseSkill(hit.m_skill, XP);
                        }
                        break;
                    case Skills.SkillType.Axes:
                        var wName = attacker.GetCurrentWeapon().m_shared.m_name;
                        if (wName.Equals("$item_battleaxe") && States.PrimaryAttack && States.ChainCount == 0)
                        {
                            var attackerSkillTracker = attacker.gameObject.GetComponent<SkillTracker>();
                            if (attackerSkillTracker)
                            {
                                var attackerHitDuration = Time.time - attackerSkillTracker.LastAttackStart;
                                Main.log.LogDebug(
                                    $"Attack from {hit.GetAttacker().GetHoverName()} took {attackerHitDuration}s");
                                if (attackerSkillTracker.LastHitTaken <= attackerSkillTracker.LastAttackStart)
                                {
                                    if (IsLocalPlayer(hit.GetAttacker()))
                                    {
                                        XP = DamageToXP(self, hit);
                                        NotificationHandler.Instance.AddNotification($"Axes: <color={highlightColour}>+{XP:0.00}</color>XP");
                                        attacker.RaiseSkill(hit.m_skill, XP);
                                        Main.log.LogDebug(
                                            $"{hit.GetAttacker().GetHoverName()} hit {self.GetHoverName()} during window with chaincount: {States.ChainCount}");
                                    }
                                }
                            }
                        }
                        break;
                }
                Main.log.LogDebug($"{hit.GetAttacker().GetHoverName()} performed {hit.m_skill} against {self.GetHoverName()} hitting for {hit.GetTotalDamage()} yielding {XP}.");
            }
        }

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
                                NotificationHandler.Instance.AddNotification($"{hitData.m_skill}: <color={highlightColour}>+{XP:0.00}</color>XP");
                                NotificationHandler.Instance.AddNotification($"Distance: <color={highlightColour}>{distance:0.0}</color>m");

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
                NotificationHandler.Instance.AddNotification($"Shields: <color={highlightColour}>+{num}</color>XP");
                chara.RaiseSkill(Skills.SkillType.Blocking, num);
            });
        }

        private static float DamageToXP(Character victim, HitData hit, float damageOverride = 0)
        {
            Main.log.LogDebug($"override: {damageOverride} vs dmg: {hit.GetTotalDamage()}");
            var dmg = Mathf.Clamp(damageOverride==0 ? hit.GetTotalDamage() : damageOverride, 1, victim.GetMaxHealth()); //range: 5-100+
            var num = dmg / 20; //0.25-20
            if (num < 1) num = 1;
            Main.log.LogDebug($"override: {damageOverride} vs dmg: {hit.GetTotalDamage()}, MaxHP: {victim.GetMaxHealth()}, post-Clamp: {dmg}");
            return num;
        }

        public static bool IsLocalPlayer(Character self)
        {
            if (self.IsPlayer() && self.m_nview.IsOwner()) return true;
            return false;
        }
    }
}