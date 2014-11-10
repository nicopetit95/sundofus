using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.Entities.Models.Monsters
{
    class MonsterModel
    {
        public int ID { get; set; }
        public int GfxID { get; set; }
        public int Align { get; set; }
        public int Color { get; set; }
        public int Color2 { get; set; }
        public int Color3 { get; set; }
        public int IA { get; set; }

        public int Max_kamas { get; set; }
        public int Min_kamas { get; set; }

        public string Name { get; set; }

        public List<MonsterLevelModel> Levels { get; set; }
        public List<MonsterItem> Items { get; set; }

        public MonsterModel()
        {
            Levels = new List<MonsterLevelModel>();
            Items = new List<MonsterItem>();
        }

        public class MonsterItem
        {
            public int ID { get; set; }
            public double Chance { get; set; }
            public int Max { get; set; }

            public MonsterItem(int newID, double newChance, int newMax)
            {
                ID = newID;
                Chance = newChance;
                Max = newMax;
            }
        }
    }
}
