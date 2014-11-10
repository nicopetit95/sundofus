using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Characters.Stats
{
    class AbstractStats
    {
        public int Bases { get; set; }
        public int Items { get; set; }
        public int Dons { get; set; }
        public int Boosts { get; set; }

        public AbstractStats()
        {
            Bases = 0;
            Items = 0;
            Dons = 0;
            Boosts = 0;
        }

        public int Total()
        {
            return (Bases + Items + Dons + Boosts);
        }

        public int Base()
        {
            return Bases;
        }

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3}", Bases, Items, Dons, Boosts);
        }
    }
}
