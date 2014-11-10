using SunDofus.World.Characters.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Maps.Fights.Effects
{
    class BuffProcessor
    {
        private delegate void Buffs(EffectCast cast, EffectCast actualCast);

        private static Dictionary<EffectEnum, Buffs[]> RegisteredBuffs = new Dictionary<EffectEnum, Buffs[]>()
        {
            // Dommages
            { EffectEnum.DamageNeutre, new Buffs[] { ApplyBuffDamage, null } },
            { EffectEnum.DamageTerre, new Buffs[] { ApplyBuffDamage, null } },
            { EffectEnum.DamageFeu, new Buffs[] { ApplyBuffDamage, null } },
            { EffectEnum.DamageEau, new Buffs[] { ApplyBuffDamage, null } },
            { EffectEnum.DamageAir, new Buffs[] { ApplyBuffDamage, null } },

            // Dommages par rapport à la vie
            { EffectEnum.DamageLifeNeutre, new Buffs[] { ApplyBuffDamage, null } },

            // Dommages par rapport aux PA
            { EffectEnum.DamagePerPA, new Buffs[] { ApplyBuffDamage, null } },

            // Armures
            { EffectEnum.AddArmor , new Buffs[] { null, RemoveBuffArmor } },
            { EffectEnum.AddArmorBis , new Buffs[] { null, RemoveBuffArmor } },

            // Augmentations/réductions de PA/PM/PO
            { EffectEnum.AddPA , new Buffs[] { null, RemoveBuffStats } },
            { EffectEnum.AddPABis , new Buffs[] { null, RemoveBuffStats } },
            { EffectEnum.AddPM , new Buffs[] { null, RemoveBuffStats } },
            { EffectEnum.AddEsquivePA , new Buffs[] { null, RemoveBuffStats } },
            { EffectEnum.AddEsquivePM , new Buffs[] { null, RemoveBuffStats } },
            { EffectEnum.SubPA , new Buffs[] { null, RemoveBuffStats } },
            { EffectEnum.SubPM , new Buffs[] { null, RemoveBuffStats } },
            { EffectEnum.SubPAEsquive , new Buffs[] { null, RemoveBuffStats } },
            { EffectEnum.SubPMEsquive , new Buffs[] { null, RemoveBuffStats } },
            { EffectEnum.SubEsquivePA , new Buffs[] { null, RemoveBuffStats } },
            { EffectEnum.SubEsquivePM , new Buffs[] { null, RemoveBuffStats } },
            { EffectEnum.AddPO , new Buffs[] { null, RemoveBuffStats } },
            { EffectEnum.SubPO , new Buffs[] { null, RemoveBuffStats } },

            // Augmentations/réductions des caractéristiques
            { EffectEnum.AddVie , new Buffs[] { null, RemoveBuffStats } },
            { EffectEnum.AddVitalite , new Buffs[] { null, RemoveBuffStats } },
            { EffectEnum.AddSagesse , new Buffs[] { null, RemoveBuffStats } },
            { EffectEnum.AddForce , new Buffs[] { null, RemoveBuffStats } },
            { EffectEnum.AddIntelligence , new Buffs[] { null, RemoveBuffStats } },
            { EffectEnum.AddChance , new Buffs[] { null, RemoveBuffStats } },
            { EffectEnum.AddAgilite , new Buffs[] { null, RemoveBuffStats } },
            { EffectEnum.SubVitalite , new Buffs[] { null, RemoveBuffStats } },  
            { EffectEnum.SubSagesse , new Buffs[] { null, RemoveBuffStats } },
            { EffectEnum.SubForce , new Buffs[] { null, RemoveBuffStats } },
            { EffectEnum.SubIntelligence , new Buffs[] { null, RemoveBuffStats } },
            { EffectEnum.SubChance , new Buffs[] { null, RemoveBuffStats } },
            { EffectEnum.SubAgilite , new Buffs[] { null, RemoveBuffStats } },  

            // Augmentation/réduction des soins
            { EffectEnum.AddSoins , new Buffs[] { null, RemoveBuffStats } },
            { EffectEnum.SubSoins , new Buffs[] { null, RemoveBuffStats } },

            // Augmentation du nombre d'invocations maximum
            { EffectEnum.AddInvocationMax , new Buffs[] { null, RemoveBuffStats } },

            // Augmentations/réductions des résistances
            { EffectEnum.AddReduceDamagePhysic  , new Buffs[] { null, RemoveBuffStats } },
            { EffectEnum.AddReduceDamageMagic , new Buffs[] { null, RemoveBuffStats } },
            { EffectEnum.AddReduceDamageNeutre  , new Buffs[] { null, RemoveBuffStats } },
            { EffectEnum.AddReduceDamageTerre , new Buffs[] { null, RemoveBuffStats } },
            { EffectEnum.AddReduceDamageFeu , new Buffs[] { null, RemoveBuffStats } },
            { EffectEnum.AddReduceDamageEau , new Buffs[] { null, RemoveBuffStats } },
            { EffectEnum.AddReduceDamageAir , new Buffs[] { null, RemoveBuffStats } },
            { EffectEnum.SubReduceDamageNeutre , new Buffs[] { null, RemoveBuffStats } },
            { EffectEnum.SubReduceDamageTerre , new Buffs[] { null, RemoveBuffStats } },
            { EffectEnum.SubReduceDamageFeu , new Buffs[] { null, RemoveBuffStats } },
            { EffectEnum.SubReduceDamageEau , new Buffs[] { null, RemoveBuffStats } },
            { EffectEnum.SubReduceDamageAir , new Buffs[] { null, RemoveBuffStats } },
            { EffectEnum.AddReduceDamagePourcentNeutre , new Buffs[] { null, RemoveBuffStats } },
            { EffectEnum.AddReduceDamagePourcentTerre , new Buffs[] { null, RemoveBuffStats } },
            { EffectEnum.AddReduceDamagePourcentFeu , new Buffs[] { null, RemoveBuffStats } },
            { EffectEnum.AddReduceDamagePourcentEau , new Buffs[] { null, RemoveBuffStats } },
            { EffectEnum.AddReduceDamagePourcentAir , new Buffs[] { null, RemoveBuffStats } },
            { EffectEnum.SubReduceDamagePourcentNeutre , new Buffs[] { null, RemoveBuffStats } },
            { EffectEnum.SubReduceDamagePourcentTerre , new Buffs[] { null, RemoveBuffStats } },
            { EffectEnum.SubReduceDamagePourcentFeu , new Buffs[] { null, RemoveBuffStats } },
            { EffectEnum.SubReduceDamagePourcentEau , new Buffs[] { null, RemoveBuffStats } },
            { EffectEnum.SubReduceDamagePourcentAir , new Buffs[] { null, RemoveBuffStats } },

            // Augmentations/réductions des dommages
            { EffectEnum.AddDamage , new Buffs[] { null, RemoveBuffStats } },
            { EffectEnum.AddDamageMagic , new Buffs[] { null, RemoveBuffStats } },
            { EffectEnum.AddDamagePhysic , new Buffs[] { null, RemoveBuffStats } },
            { EffectEnum.AddDamagePercent , new Buffs[] { null, RemoveBuffStats } },
            { EffectEnum.AddRenvoiDamage , new Buffs[] { null, RemoveBuffStats } },
            { EffectEnum.AddDamageCritic , new Buffs[] { null, RemoveBuffStats } },
            { EffectEnum.AddEchecCritic , new Buffs[] { null, RemoveBuffStats } },
            { EffectEnum.SubDamage , new Buffs[] { null, RemoveBuffStats } },
            { EffectEnum.SubDamageBis , new Buffs[] { null, RemoveBuffStats } },
            { EffectEnum.SubDamageMagic , new Buffs[] { null, RemoveBuffStats } },
            { EffectEnum.SubDamagePhysic , new Buffs[] { null, RemoveBuffStats } },
            { EffectEnum.SubDamagePercent , new Buffs[] { null, RemoveBuffStats } },
            { EffectEnum.SubDamageCritic , new Buffs[] { null, RemoveBuffStats } },

            // Augmentation des dommages d'un sort
            { EffectEnum.IncreaseSpellDamage , new Buffs[] { ApplyBuffIncreaseSpellDamage, null } },

            // Change d'apparence
            { EffectEnum.ChangeSkin ,new Buffs[] { null, RemoveBuffChangeSkin } },

            // Sacrieurs
            { EffectEnum.Sacrifice , new Buffs[] { ApplyBuffSacrifice, null } },
            { EffectEnum.MissBack , new Buffs[] { ApplyBuffMissBack, null } },
            { EffectEnum.AddChatiment , new Buffs[] { ApplyBuffChatiment, null } },
            { EffectEnum.ChatimentDamage , new Buffs[] { ApplyBuffChatimentDamage, null } },

            // Autres
            { EffectEnum.ChanceEcaflip , new Buffs[] { ApplyBuffChanceEcaflip, null } },
        };

        public static void ApplyBuff(EffectCast cast, EffectCast actualCast = null)
        {
            if (!RegisteredBuffs.ContainsKey(cast.Type))
                return;

            if (cast.Target == null)
                return;

            if (RegisteredBuffs[cast.Type][0] != null)
                RegisteredBuffs[cast.Type][0](cast, actualCast);
        }

        public static void RemoveBuff(EffectCast cast)
        {
            if (!RegisteredBuffs.ContainsKey(cast.Type))
                return;

            if (cast.Target == null)
                return;

            if (RegisteredBuffs[cast.Type][1] != null)
                RegisteredBuffs[cast.Type][1](cast, null);
        }

        public static void ApplyBuffDamage(EffectCast cast, EffectCast actualCast)
        {
            EffectProcessor.ApplyEffect(cast);
        }

        public static void ApplyBuffIncreaseSpellDamage(EffectCast cast, EffectCast actualCast)
        {
            if (cast.SpellID == actualCast.SpellID)
                actualCast.Jet += cast.Value3;
        }

        public static void ApplyBuffSacrifice(EffectCast cast, EffectCast actualCast)
        {
            EffectProcessor.ApplyEffect(new EffectCast(EffectEnum.Transpose, cast.SpellID, -1, -1, -1, 0, 0, 0, false, cast.Caster, null, cast.Target));

            actualCast.Target = cast.Caster;
        }

        public static void ApplyBuffMissBack(EffectCast cast, EffectCast actualCast)
        {
            if (!actualCast.AtCac)
                return;

            int chance = cast.Value1;
            int push = cast.Value2;

            if (Utilities.Basic.Rand(0, 99) < chance)
            {
                EffectProcessor.ApplyEffect(new EffectCast(EffectEnum.PushBack, cast.SpellID, actualCast.CellID, push, -1, -1, 0, 0, false, actualCast.Caster, null, cast.Target));

                actualCast.Jet = 0;
            }
        }

        public static void ApplyBuffChatiment(EffectCast cast, EffectCast actualCast)
        {
            int statsValue = actualCast.Jet / 2;
            EffectEnum statsType = (EffectEnum)cast.Value1 == EffectEnum.Heal ? EffectEnum.AddVitalite : (EffectEnum)cast.Value1;
            int maxValue = cast.Value2;
            int duration = cast.Value3;

            if (statsValue > maxValue)
                statsValue = maxValue;

            EffectProcessor.ApplyEffect(new EffectCast(statsType, cast.SpellID, 0, statsValue, -1, -1, 0, duration, false, cast.Caster, null, cast.Target));
        }

        public static void ApplyBuffChatimentDamage(EffectCast cast, EffectCast actualCast)
        {
            actualCast.Jet = (int)((1 + (double)cast.Value1 / 100) * actualCast.Jet);
        }

        public static void ApplyBuffChanceEcaflip(EffectCast cast, EffectCast actualCast)
        {
            int coefDamages = cast.Value1;
            int coefHeal = cast.Value2;
            int chance = cast.Value3;

            if (Utilities.Basic.Rand(0, 99) < chance)
            {
                EffectProcessor.ApplyEffect(new EffectCast(EffectEnum.Heal, cast.SpellID, actualCast.Jet * coefHeal, -1, -1, 0, 0, 0, false, cast.Caster, null, cast.Target));

                actualCast.Jet = 0;
            }
            else
            {
                actualCast.Jet *= coefDamages;
            }
        }

        public static void RemoveBuffArmor(EffectCast cast, EffectCast actualCast)
        {
            Fighter target = cast.Target;

            switch (cast.SpellID)
            {
                case 1: target.Stats.ModifyStatBonus(EffectEnum.SubArmorFeu, cast.Jet); break;
                case 6: target.Stats.ModifyStatBonus(EffectEnum.SubArmorTerre, cast.Jet); break;
                case 14: target.Stats.ModifyStatBonus(EffectEnum.SubArmorAir, cast.Jet); break;
                case 18: target.Stats.ModifyStatBonus(EffectEnum.SubArmorEau, cast.Jet); break;
                default: target.Stats.ModifyStatBonus(EffectEnum.SubArmor, cast.Jet); break;
            }
        }

        public static void RemoveBuffStats(EffectCast cast, EffectCast actualCast)
        {
            cast.Target.Stats.ModifyStatBonus(cast.Type, cast.Jet, true);
        }

        public static void RemoveBuffChangeSkin(EffectCast cast, EffectCast actualCast)
        {
            cast.Target.Skin = cast.Value2;

            cast.Caster.Fight.Send("GA;149;" + cast.Caster.ID + ';' + cast.Target.ID + ',' + cast.Value3 + ',' + cast.Value2);
        }
    }
}
