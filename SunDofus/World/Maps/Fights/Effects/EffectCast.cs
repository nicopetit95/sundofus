using SunDofus.World.Characters.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Maps.Fights.Effects
{
    class EffectCast
    {
        private int myCachedRandomJet = -1;

        public EffectCast(EffectEnum type, int spellID, int cellID, int value1, int value2, int value3, int chance, int duration, bool atCAC, Fighter caster, List<Fighter> targets, Fighter target = null)
        {
            Type = type;
            SpellID = spellID;
            CellID = cellID;
            Value1 = value1;
            Value2 = value2;
            Value3 = value3;
            Chance = chance;
            Duration = duration;
            AtCac = atCAC;
            Caster = caster;
            Targets = targets;
            Target = target;
        }

        public EffectEnum Type
        {
            get;
            set;
        }

        public int SpellID
        {
            get;
            set;
        }

        public int CellID
        {
            get;
            set;
        }

        public int Value1
        {
            get;
            set;
        }

        public int Value2
        {
            get;
            set;
        }

        public int Value3
        {
            get;
            set;
        }

        public int RandomJet
        {
            get
            {
                if (myCachedRandomJet == -1)
                    myCachedRandomJet = (Value2 == -1 ? Value1 : Utilities.Basic.Rand(Value1, Value2));

                return myCachedRandomJet;
            }
            set
            {
                myCachedRandomJet = value;
            }
        }

        public int NewRandomJet
        {
            get { return (Value2 == -1 ? Value1 : Utilities.Basic.Rand(Value1, Value2)); }
        }

        public int Chance
        {
            get;
            set;
        }

        public int Duration
        {
            get;
            set;
        }

        public bool AtCac
        {
            get;
            set;
        }

        public Fighter Caster
        {
            get;
            set;
        }

        public Fighter Target
        {
            get;
            set;
        }

        public List<Fighter> Targets
        {
            get;
            set;
        }

        public int Jet
        {
            get;
            set;
        }

        public EffectCast CopyToBuff()
        {
            EffectCast cast = new EffectCast(Type, SpellID, CellID, Value1, Value2, Value3, Chance, Duration, false, Caster, null, Target);
            cast.Jet = Jet;

            return cast;
        }
    }
}
