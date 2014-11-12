using SunDofus.World.Characters.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Maps.Fights.Effects
{
    class EffectProcessor
    {
        private delegate void Effects(EffectCast cast);

        private static Dictionary<EffectEnum, Effects> RegisteredEffects = new Dictionary<EffectEnum, Effects>()
        {
            //USEGLYPH (401)
            //USETRAP (400)
            //INVOCATION (181)
            //INVOCDOUBLE (180)
            //ADDSTATE (950)
            //INVISIBLE (150)
            //PERCEPTION (202)
            //REFLECTSPELL (106)
            //PORTER (50)
            //LANCER (51)

            // Dommages
            { EffectEnum.DamageNeutre, EffectDamage },
            { EffectEnum.DamageTerre, EffectDamage },
            { EffectEnum.DamageFeu, EffectDamage },
            { EffectEnum.DamageEau, EffectDamage },
            { EffectEnum.DamageAir, EffectDamage },

            // Dommages par rapport à la vie
            { EffectEnum.DamageLifeNeutre, EffectDamageLife },

            // Dommages par rapport aux PA
            { EffectEnum.DamagePerPA, EffectDamagePerPA },

            // Dommages en partageant sa vie
            { EffectEnum.DamageDropLife, EffectDamageDropLife },

            // Soin
            { EffectEnum.Heal, EffectHeal },

            // Vols de vie
            { EffectEnum.VolNeutre, EffectStealLife },
            { EffectEnum.VolTerre, EffectStealLife },
            { EffectEnum.VolFeu, EffectStealLife },
            { EffectEnum.VolEau, EffectStealLife },
            { EffectEnum.VolAir, EffectStealLife },

            // Vol de vie fix
            { EffectEnum.VolVie , EffectStealLifeFix },

            // Téléportations
            { EffectEnum.Teleport, EffectTeleport },
            { EffectEnum.Transpose, EffectTranspose },

            // Armures
            { EffectEnum.AddArmor, EffectArmor },
            { EffectEnum.AddArmorBis, EffectArmor },

            // Augmentations/réductions de PA/PM/PO
            { EffectEnum.AddPA, EffectStats },
            { EffectEnum.AddPABis, EffectStats },
            { EffectEnum.AddPM, EffectStats },
            { EffectEnum.AddEsquivePA, EffectStats },
            { EffectEnum.AddEsquivePM, EffectStats },
            { EffectEnum.SubPA, EffectStats },
            { EffectEnum.SubPM, EffectStats },
            { EffectEnum.SubPAEsquive, EffectSubAPEsquive },
            { EffectEnum.SubPMEsquive, EffectSubMPEsquive },
            { EffectEnum.SubEsquivePA, EffectStats },
            { EffectEnum.SubEsquivePM, EffectStats },
            { EffectEnum.AddPO, EffectStats },
            { EffectEnum.SubPO, EffectStats },

            // Augmentations/réductions des caractéristiques
            { EffectEnum.AddVie, EffectStats },
            { EffectEnum.AddVitalite, EffectStats },
            { EffectEnum.AddSagesse, EffectStats },
            { EffectEnum.AddForce, EffectStats },
            { EffectEnum.AddIntelligence, EffectStats },
            { EffectEnum.AddChance, EffectStats },
            { EffectEnum.AddAgilite, EffectStats },
            { EffectEnum.SubVitalite, EffectStats },  
            { EffectEnum.SubSagesse, EffectStats },
            { EffectEnum.SubForce, EffectStats },
            { EffectEnum.SubIntelligence, EffectStats },
            { EffectEnum.SubChance, EffectStats },
            { EffectEnum.SubAgilite, EffectStats },  

            // Augmentation/réduction des soins
            { EffectEnum.AddSoins, EffectStats },
            { EffectEnum.SubSoins, EffectStats },

            // Augmentation du nombre d'invocations maximum
            { EffectEnum.AddInvocationMax, EffectStats },

            // Augmentations/réductions des résistances
            { EffectEnum.AddReduceDamagePhysic  , EffectStats },
            { EffectEnum.AddReduceDamageMagic , EffectStats },
            { EffectEnum.AddReduceDamageNeutre  , EffectStats },
            { EffectEnum.AddReduceDamageTerre , EffectStats },
            { EffectEnum.AddReduceDamageFeu , EffectStats },
            { EffectEnum.AddReduceDamageEau , EffectStats },
            { EffectEnum.AddReduceDamageAir , EffectStats },
            { EffectEnum.SubReduceDamageNeutre , EffectStats },
            { EffectEnum.SubReduceDamageTerre , EffectStats },
            { EffectEnum.SubReduceDamageFeu , EffectStats },
            { EffectEnum.SubReduceDamageEau , EffectStats },
            { EffectEnum.SubReduceDamageAir , EffectStats },
            { EffectEnum.AddReduceDamagePourcentNeutre , EffectStats },
            { EffectEnum.AddReduceDamagePourcentTerre , EffectStats },
            { EffectEnum.AddReduceDamagePourcentFeu , EffectStats },
            { EffectEnum.AddReduceDamagePourcentEau , EffectStats },
            { EffectEnum.AddReduceDamagePourcentAir , EffectStats },
            { EffectEnum.SubReduceDamagePourcentNeutre , EffectStats },
            { EffectEnum.SubReduceDamagePourcentTerre , EffectStats },
            { EffectEnum.SubReduceDamagePourcentFeu , EffectStats },
            { EffectEnum.SubReduceDamagePourcentEau , EffectStats },
            { EffectEnum.SubReduceDamagePourcentAir , EffectStats },

            // Augmentations/réductions des dommages
            { EffectEnum.AddDamage , EffectStats },
            { EffectEnum.AddDamageMagic , EffectStats },
            { EffectEnum.AddDamagePhysic , EffectStats },
            { EffectEnum.AddDamagePercent , EffectStats },
            { EffectEnum.AddRenvoiDamage , EffectStats },
            { EffectEnum.AddDamageCritic , EffectStats },
            { EffectEnum.AddEchecCritic , EffectStats },
            { EffectEnum.SubDamage , EffectStats },
            { EffectEnum.SubDamageBis , EffectStats },
            { EffectEnum.SubDamageMagic , EffectStats },
            { EffectEnum.SubDamagePhysic , EffectStats },
            { EffectEnum.SubDamagePercent , EffectStats },
            { EffectEnum.SubDamageCritic , EffectStats },

            // Vols de statistique
            { EffectEnum.VolSagesse , EffectStealStats },
            { EffectEnum.VolForce , EffectStealStats },
            { EffectEnum.VolIntell , EffectStealStats },
            { EffectEnum.VolChance , EffectStealStats },
            { EffectEnum.VolAgi , EffectStealStats },
            { EffectEnum.VolPA , EffectStealStats },
            { EffectEnum.VolPM , EffectStealStats },
            { EffectEnum.VolPO , EffectStealStats },

            // Poussés
            { EffectEnum.PushBack , EffectPush },
            { EffectEnum.PushFront , EffectPush },
            { EffectEnum.PushFear , EffectPush },

            // Debuff
            { EffectEnum.DeleteAllBonus , EffectDebuff },

            // Augmentation des dommages d'un sort
            { EffectEnum.IncreaseSpellDamage , EffectIncreaseSpellDamage },

            // Change l'apparence
            { EffectEnum.ChangeSkin , EffectChangeSkin },

            // Passe le tour
            { EffectEnum.TurnPass , EffectTurnPass },

            // Sacrieurs
            { EffectEnum.Sacrifice , EffectSacrifice },
            { EffectEnum.MissBack , EffectMissBack },
            { EffectEnum.Punition , EffectPunition },
            { EffectEnum.AddChatiment , EffectChatiment },
            { EffectEnum.ChatimentDamage , EffectChatimentDamage },

            // Autres
            { EffectEnum.ChanceEcaflip , EffectChanceEcaflip },
            { EffectEnum.Arnaque , EffectArnaque },
        };

        public static void ApplyEffect(EffectCast cast)
        {
            if (!RegisteredEffects.ContainsKey(cast.Type))
                return;

            if (cast.Targets == null)
                cast.Targets = new List<Fighter>();

            if (cast.Targets.Count > 0)
            {
                foreach (Fighter target in cast.Targets)
                {
                    cast.Target = target;
                    RegisteredEffects[cast.Type](cast);
                }
            }
            else
            {
                RegisteredEffects[cast.Type](cast);
            }
        }

        public static void EffectDamage(EffectCast cast)
        {
            if (cast.Target == null)
                return;

            if (cast.Duration > 0)
            {
                cast.Target.Buffs.AddBuff(cast, BuffActiveType.ACTIVE_ENDTURN, BuffDecrementType.TYPE_ENDTURN);
            }
            else
            {
                cast.Jet = 0;

                cast.Caster.Buffs.OnAttackPostJet(cast);
                cast.Target.Buffs.OnAttackedPostJet(cast);

                cast.Jet += cast.RandomJet;
                cast.Jet = cast.Caster.CalculDamages(cast.Type, cast.Jet);
                cast.Jet = cast.Target.CalculReduceDamages(cast.Type, cast.Jet);

                if (cast.Jet > 0)
                {
                    int armor = cast.Target.CalculArmor(cast.Type);

                    if (armor > 0)
                    {
                        cast.Jet -= armor;

                        cast.Caster.Fight.Send("GA;105;" + cast.Caster.ID + ';' + cast.Target.ID + ',' + armor);
                    }
                }

                cast.Caster.Buffs.OnAttackAfterJet(cast);
                cast.Target.Buffs.OnAttackedAfterJet(cast);

                if (cast.Jet > 0)
                {
                    int reflectDamages = cast.Target.Stats.GetStat(StatEnum.ReflectDamage).Total;

                    if (reflectDamages > 0)
                    {
                        if (reflectDamages > cast.Jet)
                            reflectDamages = cast.Jet;

                        cast.Jet -= reflectDamages;
                        cast.Caster.Life -= reflectDamages;

                        cast.Caster.Fight.Send("GA;107;" + cast.Target.ID + ';' + cast.Target.ID + ',' + reflectDamages);
                        cast.Caster.Fight.Send("GA;100;" + cast.Target.ID + ';' + cast.Caster.ID + ",-" + reflectDamages);
                    }
                }

                if (cast.Jet < 0)
                    cast.Jet = 0;

                if (cast.Jet > cast.Target.Life)
                    cast.Jet = cast.Target.Life;

                cast.Target.Life -= cast.Jet;

                if (cast.Target.Life == 0)
                    cast.Caster.Fight.Send("GA;103;" + cast.Caster.ID + ";" + cast.Target.ID);

                cast.Caster.Fight.Send("GA;100;" + cast.Caster.ID + ';' + cast.Target.ID + ",-" + cast.Jet);
            }
        }

        public static void EffectDamageLife(EffectCast cast)
        {
            if (cast.Target == null)
                return;

            if (cast.Duration > 0)
            {
                cast.Target.Buffs.AddBuff(cast, BuffActiveType.ACTIVE_ATTACKED_POST_JET, BuffDecrementType.TYPE_ENDTURN);
            }
            else
            {
                cast.Jet = (int)((double)cast.RandomJet / 100 * cast.Target.Life);
                cast.Target.Life -= cast.Jet;

                cast.Caster.Fight.Send("GA;100;" + cast.Caster.ID + ';' + cast.Target.ID + ",-" + cast.Jet);
            }
        }

        public static void EffectDamagePerPA(EffectCast cast)
        {
            if (cast.Target == null)
                return;

            if (cast.Duration > 0)
            {
                cast.Target.Buffs.AddBuff(cast, BuffActiveType.ACTIVE_ENDTURN, BuffDecrementType.TYPE_ENDTURN);
            }
            else
            {
                cast.Jet = (cast.Target.Stats.GetStat(StatEnum.MaxPA).Total - cast.Target.AP) * cast.Value2;

                if (cast.Jet > cast.Target.Life)
                    cast.Jet = cast.Target.Life;

                cast.Target.Life -= cast.Jet;

                cast.Caster.Fight.Send("GA;100;" + cast.Caster.ID + ';' + cast.Target.ID + ",-" + cast.Jet);
            }
        }

        public static void EffectDamageDropLife(EffectCast cast)
        {
            if (cast.Target == null)
                return;

            if (cast.Caster == cast.Target)
            {
                cast.Jet = (int)((double)cast.RandomJet / 100 * cast.Caster.Life);
                cast.Caster.Life -= cast.Jet;

                cast.Caster.Fight.Send("GA;100;" + cast.Caster.ID + ';' + cast.Caster.ID + ",-" + cast.Jet);
            }
            else
            {
                int givenLife = cast.Jet;

                if (cast.Target.Life + givenLife > cast.Target.Stats.GetStat(StatEnum.MaxLife).Total)
                    givenLife = cast.Target.Stats.GetStat(StatEnum.MaxLife).Total - cast.Target.Life;

                cast.Target.Life += givenLife;

                cast.Caster.Fight.Send("GA;100;" + cast.Caster.ID + ';' + cast.Target.ID + ',' + givenLife);
            }
        }

        public static void EffectHeal(EffectCast cast)
        {
            if (cast.Target == null)
                return;

            cast.Jet = cast.RandomJet;
            cast.Jet = cast.Target.CalculHeal(cast.Jet);

            if (cast.Target.Life + cast.Jet > cast.Target.Stats.GetStat(StatEnum.MaxLife).Total)
                cast.Jet = cast.Target.Stats.GetStat(StatEnum.MaxLife).Total - cast.Target.Life;

            cast.Target.Life += cast.Jet;

            cast.Caster.Fight.Send("GA;100;" + cast.Caster.ID + ';' + cast.Target.ID + ',' + cast.Jet);
        }

        public static void EffectStealLife(EffectCast cast)
        {
            if (cast.Target == null)
                return;

            EffectDamage(cast);

            cast.RandomJet /= 2;
            cast.Target = cast.Caster;

            EffectHeal(cast);
        }

        public static void EffectStealLifeFix(EffectCast cast)
        {
            if (cast.Target == null)
                return;

            cast.Jet = cast.RandomJet;

            if (cast.Jet > cast.Target.Life)
                cast.Jet = cast.Target.Life;

            cast.Target.Life -= cast.Jet;

            cast.Caster.Fight.Send("GA;100;" + cast.Caster.ID + ';' + cast.Target.ID + ",-" + cast.Jet);

            if (cast.Caster.Life + cast.Jet > cast.Caster.Stats.GetStat(StatEnum.MaxLife).Total)
                cast.Jet = cast.Caster.Stats.GetStat(StatEnum.MaxLife).Total - cast.Caster.Life;

            cast.Caster.Life += cast.Jet;

            cast.Caster.Fight.Send("GA;100;" + cast.Caster.ID + ';' + cast.Caster.ID + ',' + cast.Jet);
        }

        public static void EffectTeleport(EffectCast cast)
        {
            if (!cast.Caster.Fight.Map.IsRushableCell(cast.CellID))
                return;

            cast.Caster.Cell = cast.CellID;

            cast.Caster.Fight.Send("GA0;4;" + cast.Caster.ID + ';' + cast.Caster.ID + ',' + cast.CellID);
        }

        public static void EffectTranspose(EffectCast cast)
        {
            if (cast.Target == null)
                return;

            int casterCell = cast.Caster.Cell;

            EffectTeleport(new EffectCast(EffectEnum.Teleport, cast.SpellID, casterCell, -1, -1, -1, 0, 0, false, cast.Caster, null));
            EffectTeleport(new EffectCast(EffectEnum.Teleport, cast.SpellID, casterCell, -1, -1, -1, 0, 0, false, cast.Target, null));
        }

        public static void EffectArmor(EffectCast cast)
        {
            if (cast.Target == null)
                return;

            if (cast.Duration == 0)
                return;

            cast.Jet = cast.RandomJet;

            switch (cast.SpellID)
            {
                case 1: cast.Target.Stats.ModifyStatBonus(EffectEnum.AddArmorFeu, cast.Jet); break;
                case 6: cast.Target.Stats.ModifyStatBonus(EffectEnum.AddArmorTerre, cast.Jet); break;
                case 14: cast.Target.Stats.ModifyStatBonus(EffectEnum.AddArmorAir, cast.Jet); break;
                case 18: cast.Target.Stats.ModifyStatBonus(EffectEnum.AddArmorEau, cast.Jet); break;
                default: cast.Target.Stats.ModifyStatBonus(EffectEnum.AddArmor, cast.Jet); break;
            }

            cast.Target.Buffs.AddBuff(cast, BuffActiveType.ACTIVE_ATTACKED_AFTER_JET, BuffDecrementType.TYPE_ENDTURN);
        }

        public static void EffectStats(EffectCast cast)
        {
            if (cast.Target == null)
                return;

            if (cast.Duration == 0)
                return;

            cast.Jet = cast.RandomJet;

            cast.Target.Stats.ModifyStatBonus(cast.Type, cast.Jet);

            bool negative = false;

            if (cast.Type == EffectEnum.SubPA || cast.Type == EffectEnum.SubPM || cast.Type == EffectEnum.SubPAEsquive || cast.Type == EffectEnum.SubPMEsquive)
                negative = true;

            cast.Caster.Fight.Send("GA;" + (int)cast.Type + ';' + cast.Caster.ID + ';' + cast.Target.ID + ',' + (negative ? "-" : string.Empty) + cast.Jet + ',' + cast.Duration);

            cast.Target.Buffs.AddBuff(cast, BuffActiveType.ACTIVE_STATS, BuffDecrementType.TYPE_ENDTURN);
        }

        public static void EffectSubAPEsquive(EffectCast cast)
        {
            if (cast.Target == null)
                return;

            if (cast.Duration == 0)
                return;

            cast.Jet = cast.RandomJet;

            if (cast.Jet > cast.Target.Stats.GetStat(StatEnum.MaxPA).Total)
                cast.Jet = cast.Target.Stats.GetStat(StatEnum.MaxPA).Total;

            cast.Jet = cast.Target.CalculDodgeAPMP(cast.Caster, cast.Jet);

            if (cast.Jet < cast.RandomJet)
                cast.Caster.Fight.Send("GA;308;" + cast.Caster.ID + ';' + cast.Target.ID + ',' + (cast.RandomJet - cast.Jet));

            if (cast.Jet > 0)
            {
                cast.RandomJet = cast.Jet;
                EffectStats(cast);
            }
        }

        public static void EffectSubMPEsquive(EffectCast cast)
        {
            if (cast.Target == null)
                return;

            if (cast.Duration == 0)
                return;

            cast.Jet = cast.RandomJet;

            if (cast.Jet > cast.Target.Stats.GetStat(StatEnum.MaxPM).Total)
                cast.Jet = cast.Target.Stats.GetStat(StatEnum.MaxPM).Total;

            cast.Jet = cast.Target.CalculDodgeAPMP(cast.Caster, cast.Jet, true);

            if (cast.Jet < cast.RandomJet)
                cast.Caster.Fight.Send("GA;309;" + cast.Caster.ID + ';' + cast.Target.ID + ',' + (cast.RandomJet - cast.Jet));

            if (cast.Jet > 0)
            {
                cast.RandomJet = cast.Jet;
                EffectStats(cast);
            }
        }

        public static void EffectStealStats(EffectCast cast)
        {
            if (cast.Target == null)
                return;

            if (cast.Duration == 0)
                return;

            EffectEnum malusEffect = EffectEnum.None;
            EffectEnum bonusEffect = EffectEnum.None;

            switch (cast.Type)
            {
                case EffectEnum.VolSagesse:
                    malusEffect = EffectEnum.SubSagesse;
                    bonusEffect = EffectEnum.AddSagesse;
                    break;

                case EffectEnum.VolForce:
                    malusEffect = EffectEnum.SubForce;
                    bonusEffect = EffectEnum.AddForce;
                    break;

                case EffectEnum.VolIntell:
                    malusEffect = EffectEnum.SubIntelligence;
                    bonusEffect = EffectEnum.AddIntelligence;
                    break;

                case EffectEnum.VolChance:
                    malusEffect = EffectEnum.SubChance;
                    bonusEffect = EffectEnum.AddChance;
                    break;

                case EffectEnum.VolAgi:
                    malusEffect = EffectEnum.SubAgilite;
                    bonusEffect = EffectEnum.AddAgilite;
                    break;

                case EffectEnum.VolPA:
                    malusEffect = EffectEnum.SubPA;
                    bonusEffect = EffectEnum.AddPA;
                    break;

                case EffectEnum.VolPM:
                    malusEffect = EffectEnum.SubPM;
                    bonusEffect = EffectEnum.AddPM;
                    break;

                case EffectEnum.VolPO:
                    malusEffect = EffectEnum.SubPO;
                    bonusEffect = EffectEnum.AddPO;
                    break;
            }

            if (malusEffect == EffectEnum.None || bonusEffect == EffectEnum.None)
                return;

            cast.Type = malusEffect;

            EffectStats(cast);

            cast.Type = bonusEffect;
            cast.Target = cast.Caster;

            EffectStats(cast);
        }

        public static void EffectPush(EffectCast cast)
        {
            int direction = -1;

            switch (cast.Type)
            {
                case EffectEnum.PushBack:
                    if (Pathfinding.InLine(cast.Caster.Fight.Map, cast.CellID, cast.Target.Cell) && cast.CellID != cast.Target.Cell)
                        direction = Pathfinding.GetDirection(cast.Caster.Fight.Map, cast.CellID, cast.Target.Cell);
                    else if (Pathfinding.InLine(cast.Caster.Fight.Map, cast.Caster.Cell, cast.Target.Cell))
                        direction = Pathfinding.GetDirection(cast.Caster.Fight.Map, cast.Caster.Cell, cast.Target.Cell);
                    else
                        return;
                    break;

                case EffectEnum.PushFront:
                    if (Pathfinding.InLine(cast.Caster.Fight.Map, cast.Caster.Cell, cast.Target.Cell))
                        direction = Pathfinding.GetDirection(cast.Caster.Fight.Map, cast.Target.Cell, cast.Caster.Cell);
                    else
                        return;
                    break;

                case EffectEnum.PushFear:
                    if (Pathfinding.InLine(cast.Caster.Fight.Map, cast.Caster.Cell, cast.CellID))
                    {
                        direction = Pathfinding.GetDirection(cast.Caster.Fight.Map, cast.Caster.Cell, cast.CellID);
                        cast.Target = cast.Caster.Fight.GetAliveFighter(Pathfinding.NextCell(cast.Caster.Fight.Map, cast.Caster.Cell, direction));
                    }
                    else
                    {
                        return;
                    }
                    break;
            }

            if (cast.Target == null)
                return;

            int lastCell = cast.Target.Cell;
            int length = cast.Type != EffectEnum.PushFear ? cast.RandomJet : Pathfinding.GetDistanceBetween(cast.Caster.Fight.Map, cast.Target.Cell, cast.CellID);

            for (int i = 0; i < length; i++)
            {
                int nextCell = Pathfinding.NextCell(cast.Caster.Fight.Map, lastCell, direction);

                if (cast.Caster.Fight.Map.IsRushableCell(nextCell))
                {
                    if (!cast.Caster.Fight.IsFreeCell(nextCell))
                    {
                        if (i > 0)
                        {
                            cast.Target.Cell = lastCell;
                            cast.Caster.Fight.Send("GA0;5;" + cast.Caster.ID + ';' + cast.Target.ID + ',' + lastCell);
                        }

                        if (cast.Type == EffectEnum.PushBack)
                            PushBackDamages(cast, length, i);

                        return;
                    }
                }
                else
                {
                    if (i > 0)
                    {
                        cast.Target.Cell = lastCell;
                        cast.Caster.Fight.Send("GA0;5;" + cast.Caster.ID + ';' + cast.Target.ID + ',' + lastCell);
                    }

                    if (cast.Type == EffectEnum.PushBack)
                        PushBackDamages(cast, length, i);

                    return;
                }

                lastCell = nextCell;
            }

            cast.Target.Cell = lastCell;
            cast.Caster.Fight.Send("GA0;5;" + cast.Caster.ID + ';' + cast.Target.ID + ',' + lastCell);
        }

        public static void PushBackDamages(EffectCast cast, int length, int currentLength)
        {
            int damageCoef = Utilities.Basic.Rand(8, 17);
            double levelCoef = cast.Caster.Level / 50;

            if (levelCoef < 0.1)
                levelCoef = 0.1;

            int damages = (int)Math.Floor(damageCoef * levelCoef) * (length - currentLength + 1);

            EffectDamage(new EffectCast(EffectEnum.DamageNeutre, cast.SpellID, damages, -1, -1, 0, 0, 0, false, cast.Caster, null, cast.Target));
        }

        public static void EffectDebuff(EffectCast cast)
        {
            if (cast.Target == null)
                return;

            cast.Target.Buffs.Debuff();

            cast.Caster.Fight.Send("GA;132;" + cast.Caster.ID + ';' + cast.Target.ID);
        }

        public static void EffectIncreaseSpellDamage(EffectCast cast)
        {
            if (cast.Duration == 0)
                return;

            cast.Target = cast.Caster;

            cast.Caster.Buffs.AddBuff(cast, BuffActiveType.ACTIVE_ATTACK_POST_JET, BuffDecrementType.TYPE_ENDTURN);
        }

        public static void EffectChangeSkin(EffectCast cast)
        {
            if (cast.Target == null)
                return;

            if (cast.Duration == 0)
                return;

            if (cast.Value3 == -1)
                return;

            cast.Value2 = cast.Target.Skin;
            cast.Target.Skin = cast.Value3;

            cast.Target.Buffs.AddBuff(cast, BuffActiveType.ACTIVE_STATS, BuffDecrementType.TYPE_ENDTURN);
            cast.Caster.Fight.Send("GA;149;" + cast.Caster.ID + ';' + cast.Target.ID + ',' + cast.Value2 + ',' + cast.Value3 + ',' + cast.Duration);
        }

        public static void EffectTurnPass(EffectCast cast)
        {
            cast.Caster.Fight.TurnEnd();
        }

        public static void EffectSacrifice(EffectCast cast)
        {
            if (cast.Target == null)
                return;

            if (cast.Duration == 0)
                return;

            cast.Target.Buffs.AddBuff(cast, BuffActiveType.ACTIVE_ATTACKED_POST_JET, BuffDecrementType.TYPE_ENDTURN);
        }

        public static void EffectMissBack(EffectCast cast)
        {
            if (cast.Target == null)
                return;

            if (cast.Duration == 0)
                return;

            cast.Target.Buffs.AddBuff(cast, BuffActiveType.ACTIVE_ATTACKED_AFTER_JET, BuffDecrementType.TYPE_ENDTURN);
        }

        public static void EffectPunition(EffectCast cast)
        {
            if (cast.Target == null)
                return;

            cast.Jet = (int)((double)cast.RandomJet / 100 * Math.Pow(Math.Cos(2 * Math.PI * ((double)cast.Caster.Life / cast.Caster.Stats.GetStat(StatEnum.MaxLife).Total - 0.5)) + 1, 2) / 4 * cast.Caster.Stats.GetStat(StatEnum.MaxLife).Total);

            if (cast.Jet > cast.Target.Life)
                cast.Jet = cast.Target.Life;

            cast.Target.Life -= cast.Jet;

            cast.Caster.Fight.Send("GA;100;" + cast.Caster.ID + ';' + cast.Target.ID + ",-" + cast.Jet);
        }

        public static void EffectChatiment(EffectCast cast)
        {
            if (cast.Target == null)
                return;

            if (cast.Duration == 0)
                return;

            cast.Target.Buffs.AddBuff(cast, BuffActiveType.ACTIVE_ATTACKED_AFTER_JET, BuffDecrementType.TYPE_ENDTURN);
        }

        public static void EffectChatimentDamage(EffectCast cast)
        {
            if (cast.Target == null)
                return;

            if (cast.Duration == 0)
                return;

            cast.Target.Buffs.AddBuff(cast, BuffActiveType.ACTIVE_ATTACKED_AFTER_JET, BuffDecrementType.TYPE_ENDTURN);
        }

        public static void EffectChanceEcaflip(EffectCast cast)
        {
            if (cast.Target == null)
                return;

            if (cast.Duration == 0)
                return;

            cast.Target.Buffs.AddBuff(cast, BuffActiveType.ACTIVE_ATTACKED_AFTER_JET, BuffDecrementType.TYPE_ENDTURN);
        }

        public static void EffectArnaque(EffectCast cast)
        {
            if (cast.Target == null || cast.Caster.Character == null || cast.Target.Character == null)
                return;

            cast.Jet = cast.RandomJet;

            if (cast.Jet > cast.Target.Character.Kamas)
                cast.Jet = (int)cast.Target.Character.Kamas;

            cast.Caster.Character.Kamas += cast.Jet;
            cast.Target.Character.Kamas -= cast.Jet;

            cast.Caster.Fight.Send("GA;130;" + cast.Caster.ID + ';' + cast.Jet);
        }

        public static void EffectDoom(EffectCast cast)
        {
            if (cast.Target != null || cast.Caster.Character == null || cast.Target.Character == null)
                return;


        }
    }
}
