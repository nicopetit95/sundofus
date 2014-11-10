using SunDofus.World.Characters.Spells;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.Entities.Models.Monsters
{
    class MonsterLevelModel
    {
        public int ID { get; set; }
        public int CreatureID { get; set; }
        public int GradeID { get; set; }

        public int Level { get; set; }
        public int AP { get; set; }
        public int MP { get; set; }
        public int Life { get; set; }

        public int RNeutral { get; set; }
        public int RStrenght { get; set; }
        public int RIntel { get; set; }
        public int RLuck { get; set; }
        public int RAgility { get; set; }

        public int RPa { get; set; }
        public int RPm { get; set; }

        public int Wisdom { get; set; }
        public int Strenght { get; set; }
        public int Intel { get; set; }
        public int Luck { get; set; }
        public int Agility { get; set; }
        public int Exp { get; set; }

        public List<CharacterSpell> Spells { get; set; }

        public MonsterLevelModel()
        {
            Spells = new List<CharacterSpell>();
        }
    }
}
