using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Maps.Zaapis
{
    class Zaapis
    {
        public int MapID { get; set; }
        public int CellID { get; set; }
        public int Faction { get; set; }

        public Map Map { get; set; }
    }
}
