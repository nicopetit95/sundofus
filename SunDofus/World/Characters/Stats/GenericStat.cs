using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Characters.Stats
{
    class GenericStat
    {
        public GenericStat(StatEnum type, EffectEnum[] addEffects, EffectEnum[] subEffects, int baseValue = 0, StatTotalFormule formule = null)
        {
            Type = type;
            AddEffects = addEffects;
            SubEffects = subEffects;

            Formule = (formule == null ? GenericStats.FormuleTotal : formule);

            Base = baseValue;
            Equipped = 0;
            Given = 0;
            Bonus = 0;
        }

        public StatEnum Type;
        public EffectEnum[] AddEffects;
        public EffectEnum[] SubEffects;

        public StatTotalFormule Formule;

        public int Base;
        public int Equipped;
        public int Given;
        public int Bonus;

        public int Total
        {
            get { return Formule(Base, Equipped, Given, Bonus); }
        }

        public override string ToString()
        {
            return string.Concat(Base, ',', Equipped, ',', Given, ',', Bonus);
        }
    }
}
