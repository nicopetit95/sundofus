using SunDofus.World.Characters.Spells;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Maps.Fights
{
    class FighterSpell
    {
        private Dictionary<int, SpellCooldown> myCooldown = new Dictionary<int, SpellCooldown>();
        private Dictionary<int, List<SpellTarget>> myTargets = new Dictionary<int, List<SpellTarget>>();

        public bool CanLaunchSpell(CharacterSpell spell, long target)
        {
            if (spell.LevelModel.TurnNumber > 0)
            {
                if (myCooldown.ContainsKey(spell.ID) && myCooldown[spell.ID].Cooldown > 0)
                    return false;
            }

            if (spell.LevelModel.MaxPerTurn > 0)
            {
                if (myTargets.ContainsKey(spell.ID) && myTargets[spell.ID].Count >= spell.LevelModel.MaxPerTurn)
                    return false;
            }

            if (spell.LevelModel.MaxPerPlayer > 0)
            {
                if (myTargets.ContainsKey(spell.ID) && myTargets[spell.ID].Count(x => x.Target == target) >= spell.LevelModel.MaxPerPlayer)
                    return false;
            }

            return true;
        }

        public void OnLaunchSpell(CharacterSpell spell, long target)
        {
            if (spell.LevelModel.TurnNumber > 0)
            {
                if (myCooldown.ContainsKey(spell.ID))
                    myCooldown[spell.ID].Cooldown = spell.LevelModel.TurnNumber;
                else
                    myCooldown.Add(spell.ID, new SpellCooldown(spell.LevelModel.TurnNumber));
            }

            if (spell.LevelModel.MaxPerTurn > 0 || spell.LevelModel.MaxPerPlayer > 0)
            {
                if (myTargets.ContainsKey(spell.ID))
                    myTargets[spell.ID].Add(new SpellTarget(target));
                else
                    myTargets.Add(spell.ID, new List<SpellTarget> { new SpellTarget(target) });
            }
        }

        public void OnTurnEnd()
        {
            foreach (List<SpellTarget> targets in myTargets.Values)
                targets.Clear();

            foreach (SpellCooldown cooldown in myCooldown.Values)
                cooldown.Decrement();
        }

        private class SpellCooldown
        {
            public SpellCooldown(int cooldown)
            {
                this.Cooldown = cooldown;
            }

            public int Cooldown
            {
                get;
                set;
            }

            public void Decrement()
            {
                this.Cooldown--;
            }
        }

        private class SpellTarget
        {
            public SpellTarget(long target)
            {
                this.Target = target;
            }

            public long Target
            {
                get;
                set;
            }
        }
    }
}