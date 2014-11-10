using SunDofus.World.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Conditions
{
    class NPCConditions
    {
        public int CondiID { get; set; }
        public string Args { get; set; }

        public bool HasCondition(Character character)
        {
            return true;
        }
    }
}
