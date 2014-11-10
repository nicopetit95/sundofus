using SunDofus.World.Characters;
using SunDofus.World.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.Entities.Models.NPC
{
    class NPCsAnswer
    {
        public int AnswerID { get; set; }
        public string Effects { get; set; }

        public List<SunDofus.World.Conditions.NPCConditions> Conditions { get; set; }

        public NPCsAnswer()
        {
            Conditions = new List<SunDofus.World.Conditions.NPCConditions>();
        }

        public void ApplyEffects(Character character)
        {
            try
            {
                foreach (var effect in Effects.Split('|'))
                {
                    var infos = effect.Split(';');
                    EffectAction.ParseEffect(character, int.Parse(infos[0]), infos[1]);
                }
            }
            catch { }
        }

        public bool HasConditions(Character character)
        {
            foreach (var condi in Conditions)
            {
                if (condi.HasCondition(character))
                    continue;
                else
                    return false;
            }

            return true;
        }
    }
}
