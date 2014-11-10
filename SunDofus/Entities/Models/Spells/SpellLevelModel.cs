using SunDofus.World.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.Entities.Models.Spells
{
    class SpellLevelModel
    {
        public int Level { get; set; }
        public int Cost { get; set; }
        public int MinRP { get; set; }
        public int MaxRP { get; set; }
        public int CC { get; set; }
        public int EC { get; set; }
        public int MaxPerTurn { get; set; }
        public int MaxPerPlayer { get; set; }
        public int TurnNumber { get; set; }

        public bool isOnlyViewLine { get; set; }
        public bool isOnlyLine { get; set; }
        public bool isAlterablePO { get; set; }
        public bool isECEndTurn { get; set; }
        public bool isEmptyCell { get; set; }

        public string Type { get; set; }

        public List<EffectSpell> Effects { get; set; }
        public List<EffectSpell> CriticalEffects { get; set; }

        public SpellLevelModel()
        {
            CriticalEffects = new List<EffectSpell>();
            Effects = new List<EffectSpell>();

            Level = -1;
            CC = 0;
            Cost = 0;
            MinRP = -1;
            MaxRP = 1;
            EC = 0;
            MaxPerPlayer = 0;
            MaxPerTurn = 0;
            TurnNumber = 0;
            isOnlyLine = false;
            isOnlyViewLine = false;
            isAlterablePO = false;
            isECEndTurn = false;
            isEmptyCell = false;
        }

        public void ParseEffect(string datas, bool CC)
        {
            var List = datas.Split('|');

            foreach (var actualEffect in List)
            {
                if (actualEffect == "-1" | actualEffect == "") 
                    continue;

                var effect = new EffectSpell();
                var infos = actualEffect.Split(';');

                effect.ID = int.Parse(infos[0]);
                effect.Value = int.Parse(infos[1]);
                effect.Value2 = int.Parse(infos[2]);
                effect.Value3 = int.Parse(infos[3]);

                if (infos.Length >= 8)
                {
                    effect.Round = int.Parse(infos[4]);
                    effect.Chance = int.Parse(infos[5]);
                    effect.Effect = infos[6];
                    effect.Target = new EffectSpellTarget(int.Parse(infos[7]));
                }
                else if (infos.Length >= 7)
                {
                    effect.Round = int.Parse(infos[4]);
                    effect.Chance = int.Parse(infos[5]);
                    effect.Effect = infos[6];
                    effect.Target = new EffectSpellTarget(23);
                }
                else if (infos.Length >= 6)
                {
                    effect.Round = int.Parse(infos[4]);
                    effect.Chance = int.Parse(infos[5]);
                    effect.Effect = "0d0+0";
                    effect.Target = new EffectSpellTarget(23);
                }
                else if (infos.Length >= 5)
                {
                    effect.Round = int.Parse(infos[4]);
                    effect.Chance = -1;
                    effect.Effect = "0d0+0";
                    effect.Target = new EffectSpellTarget(23);
                }
                else
                {
                    effect.Round = 0;
                    effect.Chance = -1;
                    effect.Effect = "0d0+0";
                    effect.Target = new EffectSpellTarget(23);
                }

                if (CC == true)
                {
                    lock(CriticalEffects)
                        CriticalEffects.Add(effect);
                }
                else
                {
                    lock(Effects)
                        Effects.Add(effect);
                }
            }
        }
    }
}
