using SunDofus.World.Characters;
using SunDofus.World.Characters.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Maps.Fights
{
    abstract class Fighter
    {
        private FighterType myType;
        private Fight myFight;

        public Fighter(FighterType type, Fight fight)
        {
            myType = type;
            myFight = fight;

            Buffs = new FighterBuff();
            SpellController = new FighterSpell();

            Left = false;
            FightReady = false;
            TurnReady = false;
        }

        public abstract int ID
        {
            get;
        }

        public abstract string Name
        {
            get;
        }

        public abstract int Level
        {
            get;
        }

        public abstract int Life
        {
            get;
            set;
        }

        public abstract int Skin
        {
            get;
            set;
        }

        public abstract GenericStats Stats
        {
            get;
        }

        public abstract Character Character
        {
            get;
        }

        public abstract string GetPattern();

        public FighterType Type
        {
            get { return myType; }
        }

        public Fight Fight
        {
            get { return myFight; }
        }

        public FightTeam Team
        {
            get;
            set;
        }

        public FighterBuff Buffs
        {
            get;
            set;
        }

        public FighterSpell SpellController
        {
            get;
            set;
        }

        public bool Left
        {
            get;
            set;
        }

        public bool FightReady
        {
            get;
            set;
        }

        public bool TurnReady
        {
            get;
            set;
        }

        public int Cell
        {
            get;
            set;
        }

        public bool IsLeader
        {
            get { return this == Team.Leader; }
        }

        public int AP
        {
            get;
            set;
        }

        public int MP
        {
            get;
            set;
        }

        public bool Dead
        {
            get { return Life <= 0 || Left; }
        }

        public int CalculDamages(EffectEnum effect, int jet)
        {
            switch (effect)
            {
                case EffectEnum.DamageTerre:
                case EffectEnum.VolTerre:
                case EffectEnum.DamageNeutre:
                case EffectEnum.VolNeutre:
                    return (int)Math.Floor((double)jet * (100 + Stats.GetStat(StatEnum.Force).Total + Stats.GetStat(StatEnum.DamagePercent).Total) / 100 +
                                                  Stats.GetStat(StatEnum.DamagePhysic).Total + Stats.GetStat(StatEnum.Damage).Total);

                case EffectEnum.DamageFeu:
                case EffectEnum.VolFeu:
                    return (int)Math.Floor((double)jet * (100 + Stats.GetStat(StatEnum.Intelligence).Total + Stats.GetStat(StatEnum.DamagePercent).Total) / 100 +
                                                  Stats.GetStat(StatEnum.DamageMagic).Total + Stats.GetStat(StatEnum.Damage).Total);

                case EffectEnum.DamageAir:
                case EffectEnum.VolAir:
                    return (int)Math.Floor((double)jet * (100 + Stats.GetStat(StatEnum.Agilite).Total + Stats.GetStat(StatEnum.DamagePercent).Total) / 100 +
                                                  Stats.GetStat(StatEnum.DamageMagic).Total + Stats.GetStat(StatEnum.Damage).Total);

                case EffectEnum.DamageEau:
                case EffectEnum.VolEau:
                    return (int)Math.Floor((double)jet * (100 + Stats.GetStat(StatEnum.Chance).Total + Stats.GetStat(StatEnum.DamagePercent).Total) / 100 +
                                                  Stats.GetStat(StatEnum.DamageMagic).Total + Stats.GetStat(StatEnum.Damage).Total);
            }

            return 0;
        }

        public int CalculReduceDamages(EffectEnum effect, int damages)
        {
            switch (effect)
            {
                case EffectEnum.DamageNeutre:
                case EffectEnum.VolNeutre:
                    return damages * (100 - Stats.GetStat(StatEnum.ReduceDamagePercentNeutre).Total - Stats.GetStat(StatEnum.ReduceDamagePercentPvPNeutre).Total) / 100
                                             - Stats.GetStat(StatEnum.ReduceDamageNeutre).Total - Stats.GetStat(StatEnum.ReduceDamagePvPNeutre).Total - Stats.GetStat(StatEnum.ReduceDamagePhysic).Total;

                case EffectEnum.DamageTerre:
                case EffectEnum.VolTerre:
                    return damages * (100 - Stats.GetStat(StatEnum.ReduceDamagePercentTerre).Total - Stats.GetStat(StatEnum.ReduceDamagePercentPvPTerre).Total) / 100
                                             - Stats.GetStat(StatEnum.ReduceDamageTerre).Total - Stats.GetStat(StatEnum.ReduceDamagePvPTerre).Total - Stats.GetStat(StatEnum.ReduceDamagePhysic).Total;

                case EffectEnum.DamageFeu:
                case EffectEnum.VolFeu:
                    return damages * (100 - Stats.GetStat(StatEnum.ReduceDamagePercentFeu).Total - Stats.GetStat(StatEnum.ReduceDamagePercentPvPFeu).Total) / 100
                                             - Stats.GetStat(StatEnum.ReduceDamageFeu).Total - Stats.GetStat(StatEnum.ReduceDamagePvPFeu).Total - Stats.GetStat(StatEnum.ReduceDamageMagic).Total;

                case EffectEnum.DamageAir:
                case EffectEnum.VolAir:
                    return damages * (100 - Stats.GetStat(StatEnum.ReduceDamagePercentAir).Total - Stats.GetStat(StatEnum.ReduceDamagePercentPvPAir).Total) / 100
                                             - Stats.GetStat(StatEnum.ReduceDamageAir).Total - Stats.GetStat(StatEnum.ReduceDamagePvPAir).Total - Stats.GetStat(StatEnum.ReduceDamageMagic).Total;

                case EffectEnum.DamageEau:
                case EffectEnum.VolEau:
                    return damages * (100 - Stats.GetStat(StatEnum.ReduceDamagePercentEau).Total - Stats.GetStat(StatEnum.ReduceDamagePercentPvPEau).Total) / 100
                                             - Stats.GetStat(StatEnum.ReduceDamageEau).Total - Stats.GetStat(StatEnum.ReduceDamagePvPEau).Total - Stats.GetStat(StatEnum.ReduceDamageMagic).Total;
            }

            return 0;
        }

        public int CalculHeal(int heal)
        {
            heal = (int)Math.Floor((double)heal * (100 + Stats.GetStat(StatEnum.Intelligence).Total) / 100 + Stats.GetStat(StatEnum.Soins).Total);
            return heal;
        }

        public int CalculArmor(EffectEnum effect)
        {
            switch (effect)
            {
                case EffectEnum.DamageTerre:
                case EffectEnum.VolTerre:
                case EffectEnum.DamageNeutre:
                case EffectEnum.VolNeutre:
                    return (Stats.GetStat(StatEnum.ArmorTerre).Total * Math.Max(1 + Stats.GetStat(StatEnum.Force).Total / 100, 1 + Stats.GetStat(StatEnum.Force).Total / 200 + Stats.GetStat(StatEnum.Intelligence).Total / 200)) +
                           (Stats.GetStat(StatEnum.Armor).Total * Math.Max(1 + Stats.GetStat(StatEnum.Force).Total / 100, 1 + Stats.GetStat(StatEnum.Force).Total / 200 + Stats.GetStat(StatEnum.Intelligence).Total / 200));

                case EffectEnum.DamageFeu:
                case EffectEnum.VolFeu:
                    return (Stats.GetStat(StatEnum.ArmorFeu).Total * Math.Max(1 + Stats.GetStat(StatEnum.Intelligence).Total / 100, 1 + Stats.GetStat(StatEnum.Intelligence).Total / 200 + Stats.GetStat(StatEnum.Intelligence).Total / 200)) +
                           (Stats.GetStat(StatEnum.Armor).Total * Math.Max(1 + Stats.GetStat(StatEnum.Intelligence).Total / 100, 1 + Stats.GetStat(StatEnum.Intelligence).Total / 200 + Stats.GetStat(StatEnum.Intelligence).Total / 200));

                case EffectEnum.DamageAir:
                case EffectEnum.VolAir:
                    return (Stats.GetStat(StatEnum.ArmorAir).Total * Math.Max(1 + Stats.GetStat(StatEnum.Agilite).Total / 100, 1 + Stats.GetStat(StatEnum.Agilite).Total / 200 + Stats.GetStat(StatEnum.Intelligence).Total / 200)) +
                           (Stats.GetStat(StatEnum.Armor).Total * Math.Max(1 + Stats.GetStat(StatEnum.Agilite).Total / 100, 1 + Stats.GetStat(StatEnum.Agilite).Total / 200 + Stats.GetStat(StatEnum.Intelligence).Total / 200));

                case EffectEnum.DamageEau:
                case EffectEnum.VolEau:
                    return (Stats.GetStat(StatEnum.ArmorEau).Total * Math.Max(1 + Stats.GetStat(StatEnum.Chance).Total / 100, 1 + Stats.GetStat(StatEnum.Chance).Total / 200 + Stats.GetStat(StatEnum.Intelligence).Total / 200)) +
                           (Stats.GetStat(StatEnum.Armor).Total * Math.Max(1 + Stats.GetStat(StatEnum.Chance).Total / 100, 1 + Stats.GetStat(StatEnum.Chance).Total / 200 + Stats.GetStat(StatEnum.Intelligence).Total / 200));
            }

            return 0;
        }

        public int CalculDodgeAPMP(Fighter caster, int lostPoint, bool IsMP = false)
        {
            int realLostPoint = 0;
            int dodgeCaster;
            int dodgeTarget;

            if (!IsMP)
            {
                dodgeCaster = caster.Stats.GetStat(StatEnum.EsquivePA).Total + 1;
                dodgeTarget = Stats.GetStat(StatEnum.EsquivePA).Total + 1;
            }
            else
            {
                dodgeCaster = caster.Stats.GetStat(StatEnum.EsquivePM).Total;
                dodgeTarget = Stats.GetStat(StatEnum.EsquivePM).Total;
            }

            if (dodgeTarget <= 0)
                dodgeTarget = 1;

            for (int i = 0; i < lostPoint; i++)
            {
                int actualPoint = (!IsMP ? AP : MP) - realLostPoint;
                int percentLastPoint = actualPoint / (!IsMP ? AP : MP);
                double chance = 0.5 * (dodgeCaster / dodgeTarget) * percentLastPoint;
                double percentChance = chance * 100;

                if (percentChance > 100) percentChance = 90;
                if (percentChance < 10) percentChance = 10;

                if (Utilities.Basic.Rand(0, 99) < percentChance) realLostPoint++;
            }

            return realLostPoint;
        }
    }
}