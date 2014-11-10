using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SunDofus.Utilities;
using SunDofus.World.Maps.Fights;

namespace SunDofus.World.Effects
{
    class EffectSpellTarget
    {
        public bool Enemies;
        public bool Allies;
        public bool Launcher;
        public bool AddLauncher;
        public bool Invocations;

        public EffectSpellTarget(int value)
        {
            Enemies = BooleanByte.GetFlag(value, 0);
            Allies = BooleanByte.GetFlag(value, 1);
            Launcher = BooleanByte.GetFlag(value, 2);
            AddLauncher = BooleanByte.GetFlag(value, 3);
            Invocations = BooleanByte.GetFlag(value, 4);
        }

        public List<Fighter> RemixTargets(Fighter caster, List<Fighter> targets)
        {
            List<Fighter> remixedTargets = new List<Fighter>();

            foreach (Fighter target in targets)
            {
                if (Enemies && caster.Team != target.Team)
                {
                    remixedTargets.Add(target);
                    continue;
                }

                if (Allies && caster.ID != target.ID && caster.Team == target.Team)
                {
                    remixedTargets.Add(target);
                    continue;
                }

                if (Launcher && caster.ID == target.ID)
                {
                    remixedTargets.Add(target);
                    continue;
                }
            }

            if (AddLauncher && !remixedTargets.Contains(caster))
                remixedTargets.Add(caster);

            return remixedTargets;
        }
    }
}
