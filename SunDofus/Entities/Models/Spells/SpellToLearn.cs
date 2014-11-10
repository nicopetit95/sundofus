using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.Entities.Models.Spells
{
    class SpellToLearnModel
    {
        public int Race { get; set; }
        public int Level { get; set; }
        public int SpellID { get; set; }
        public int Pos { get; set; }

        public SpellToLearnModel()
        {
            Race = 0;
            Level = 0;
            SpellID = 0;
            Pos = 0;
        }
    }
}
