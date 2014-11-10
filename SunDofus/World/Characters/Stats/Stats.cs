using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Characters.Stats
{
    class Stats
    {
        private Character character;

        public AbstractStats life { get; set; }
        public AbstractStats wisdom { get; set; }
        public AbstractStats strenght { get; set; }
        public AbstractStats intelligence { get; set; }
        public AbstractStats luck { get; set; }
        public AbstractStats agility { get; set; }

        public AbstractStats PA { get; set; }
        public AbstractStats PM  { get; set; }
        public AbstractStats PO  { get; set; }

        public AbstractStats maxPods  { get; set; }

        public AbstractStats maxMonsters  { get; set; }

        public AbstractStats bonusDamage  { get; set; }
        public AbstractStats bonusDamagePhysic  { get; set; }
        public AbstractStats bonusDamageMagic  { get; set; }
        public AbstractStats bonusDamagePercent  { get; set; }
        public AbstractStats bonusHeal  { get; set; }
        public AbstractStats bonusDamageTrap  { get; set; }
        public AbstractStats bonusDamageTrapPercent  { get; set; }
        public AbstractStats bonusCritical  { get; set; }
        public AbstractStats bonusFail  { get; set; }

        public AbstractStats returnDamage  { get; set; }

        public AbstractStats initiative  { get; set; }
        public AbstractStats prospection  { get; set; }
        public AbstractStats dodgePA  { get; set; }
        public AbstractStats dodgePM  { get; set; }

        public AbstractStats armorNeutral  { get; set; }
        public AbstractStats armorPercentNeutral  { get; set; }
        public AbstractStats armorPvpNeutral  { get; set; }
        public AbstractStats armorPvpPercentNeutral  { get; set; }

        public AbstractStats armorStrenght  { get; set; }
        public AbstractStats armorPercentStrenght  { get; set; }
        public AbstractStats armorPvpStrenght  { get; set; }
        public AbstractStats armorPvpPercentStrenght  { get; set; }

        public AbstractStats armorLuck  { get; set; }
        public AbstractStats armorPercentLuck  { get; set; }
        public AbstractStats armorPvpLuck  { get; set; }
        public AbstractStats armorPvpPercentLuck  { get; set; }

        public AbstractStats armorAgility  { get; set; }
        public AbstractStats armorPercentAgility  { get; set; }
        public AbstractStats armorPvpAgility  { get; set; }
        public AbstractStats armorPvpPercentAgility  { get; set; }

        public AbstractStats armorIntelligence  { get; set; }
        public AbstractStats armorPercentIntelligence  { get; set; }
        public AbstractStats armorPvpIntelligence  { get; set; }
        public AbstractStats armorPvpPercentIntelligence  { get; set; }

        public Stats(Character character)
        {
            this.character = character;

            life = new AbstractStats();
            wisdom = new AbstractStats();
            strenght = new AbstractStats();
            intelligence = new AbstractStats();
            luck = new AbstractStats();
            agility = new AbstractStats();

            PA = new AbstractStats();
            PM = new AbstractStats();
            PO = new AbstractStats();

            maxPods = new AbstractStats();

            maxMonsters = new AbstractStats();

            bonusDamage = new AbstractStats();
            bonusDamagePhysic = new AbstractStats();
            bonusDamageMagic = new AbstractStats();
            bonusDamagePercent = new AbstractStats();
            bonusHeal = new AbstractStats();
            bonusDamageTrap = new AbstractStats();
            bonusDamageTrapPercent = new AbstractStats();
            bonusCritical = new AbstractStats();
            bonusFail = new AbstractStats();

            returnDamage = new AbstractStats();

            initiative = new AbstractStats();
            prospection = new AbstractStats();
            dodgePA = new AbstractStats();
            dodgePM = new AbstractStats();

            armorNeutral = new AbstractStats();
            armorPercentNeutral = new AbstractStats();
            armorPvpNeutral = new AbstractStats();
            armorPvpPercentNeutral = new AbstractStats();

            armorStrenght = new AbstractStats();
            armorPercentStrenght = new AbstractStats();
            armorPvpStrenght = new AbstractStats();
            armorPvpPercentStrenght = new AbstractStats();

            armorLuck = new AbstractStats();
            armorPercentLuck = new AbstractStats();
            armorPvpLuck = new AbstractStats();
            armorPvpPercentLuck = new AbstractStats();

            armorAgility = new AbstractStats();
            armorPercentAgility = new AbstractStats();
            armorPvpAgility = new AbstractStats();
            armorPvpPercentAgility = new AbstractStats();

            armorIntelligence = new AbstractStats();
            armorPercentIntelligence = new AbstractStats();
            armorPvpIntelligence = new AbstractStats();
            armorPvpPercentIntelligence = new AbstractStats();
        }
    }
}
