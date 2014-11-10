using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SunDofus.World.Characters.Spells;

namespace SunDofus.World.Effects
{
    class EffectSpell
    {
        public int ID { get; set; }
        public int Value { get; set; }
        public int Value2 { get; set; }
        public int Value3 { get; set; }
        public int Round { get; set; }
        public int Chance { get; set; }
        public string Effect { get; set; }

        public EffectSpellTarget Target { get; set; }

        public EffectSpell()
        {
            Value = 0;
            Value2 = 0;
            Value3 = 0;

            Round = 0;
            Chance = 0;

            Effect = "1d5+0";
        }
    }
}
